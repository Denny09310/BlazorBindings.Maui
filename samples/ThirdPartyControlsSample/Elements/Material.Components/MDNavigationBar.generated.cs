// <auto-generated>
//     This code was generated by a BlazorBindings.Maui component generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using BlazorBindings.Core;
using BlazorBindings.Maui.Elements;
using MC = Microsoft.Maui.Controls;
using MCM = Material.Components.Maui;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Threading.Tasks;

#pragma warning disable CA2252

namespace BlazorBindings.Maui.Elements.Material.Components
{
    public partial class MDNavigationBar : BlazorBindings.Maui.Elements.TemplatedView
    {
        static MDNavigationBar()
        {
            RegisterAdditionalHandlers();
        }

        [Parameter] public bool? HasLabel { get; set; }
        [Parameter] public int? SelectedIndex { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        public new MCM.NavigationBar NativeControl => (MCM.NavigationBar)((BindableObject)this).NativeControl;

        protected override MCM.NavigationBar CreateNativeElement() => new();

        protected override void HandleParameter(string name, object value)
        {
            switch (name)
            {
                case nameof(HasLabel):
                    if (!Equals(HasLabel, value))
                    {
                        HasLabel = (bool?)value;
                        NativeControl.HasLabel = HasLabel ?? (bool)MCM.NavigationBar.HasLabelProperty.DefaultValue;
                    }
                    break;
                case nameof(SelectedIndex):
                    if (!Equals(SelectedIndex, value))
                    {
                        SelectedIndex = (int?)value;
                        NativeControl.SelectedIndex = SelectedIndex ?? (int)MCM.NavigationBar.SelectedIndexProperty.DefaultValue;
                    }
                    break;
                case nameof(ChildContent):
                    ChildContent = (RenderFragment)value;
                    break;

                default:
                    base.HandleParameter(name, value);
                    break;
            }
        }

        protected override void RenderAdditionalElementContent(RenderTreeBuilder builder, ref int sequence)
        {
            base.RenderAdditionalElementContent(builder, ref sequence);
            RenderTreeBuilderHelper.AddListContentProperty<MCM.NavigationBar, MCM.NavigationBarItem>(builder, sequence++, ChildContent, x => x.Items);
        }

        static partial void RegisterAdditionalHandlers();
    }
}