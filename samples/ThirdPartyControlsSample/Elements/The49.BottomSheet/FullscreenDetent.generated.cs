// <auto-generated>
//     This code was generated by a BlazorBindings.Maui component generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using BlazorBindings.Core;
using BlazorBindings.Maui.Elements;
using MC = Microsoft.Maui.Controls;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using TMB = The49.Maui.BottomSheet;

#pragma warning disable MBB001

namespace BlazorBindings.Maui.Elements.The49.BottomSheet
{
    public partial class FullscreenDetent : Detent
    {
        static FullscreenDetent()
        {
            RegisterAdditionalHandlers();
        }

        public new TMB.FullscreenDetent NativeControl => (TMB.FullscreenDetent)((BindableObject)this).NativeControl;

        protected override TMB.FullscreenDetent CreateNativeElement() => new();


        static partial void RegisterAdditionalHandlers();
    }
}