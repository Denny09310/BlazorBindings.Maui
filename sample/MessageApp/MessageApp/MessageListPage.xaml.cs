﻿using BlazorDesktop;
using MessageApp.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MessageApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageListPage : ContentPage
    {
        readonly ContentPage subPage;
        readonly AppState appState;

        public MessageListPage()
        {
            InitializeComponent();
            appState = BlazorDesktopDefaultServices.Instance.GetRequiredService<AppState>();

            var subPageItems = new StackLayout();
            subPageItems.Children.Add(new Label { Text = "This was generated programmatically" });
            subPage = new ContentPage { Content = subPageItems };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            appState.CurrentMessageChanged += NavigateToMessage;

            // Workaround for "master" icon not appearing if you:
            //  1. Use portrait mode on tablet
            //  2. Navigate into a subpage
            //  3. Rotate to landscape
            //  4. Click 'back'
            // This property assignment results in a call to UpdateTitleArea on NavigationRenderer
            var master = ((MasterDetailPage)Parent.Parent).Master;
            var originalValue = master.IconImageSource;
            master.IconImageSource = "temp.png";
            master.IconImageSource = originalValue;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            appState.CurrentMessageChanged -= NavigateToMessage;
        }

        private void NavigateToMessage(object sender, Message e)
        {
            subPage.Title = e.Subject;
            _ = Navigation.PushAsync(subPage);
        }
    }
}
