// <auto-generated>
//     This code was generated by a BlazorBindings.Maui component generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using BlazorBindings.Core;
using MC = Microsoft.Maui.Controls;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

#pragma warning disable CA2252

namespace BlazorBindings.Maui.Elements
{
    /// <summary>
    /// Base class for generalized user-defined behaviors that can respond to arbitrary conditions and events.
    /// </summary>
    public abstract partial class Behavior : BindableObject
    {
        static Behavior()
        {
            RegisterAdditionalHandlers();
        }

        public new MC.Behavior NativeControl => (MC.Behavior)((BindableObject)this).NativeControl;



        static partial void RegisterAdditionalHandlers();
    }
}