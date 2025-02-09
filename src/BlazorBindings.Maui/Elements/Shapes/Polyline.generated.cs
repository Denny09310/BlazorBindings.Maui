// <auto-generated>
//     This code was generated by a BlazorBindings.Maui component generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using BlazorBindings.Core;
using BlazorBindings.Maui.Elements;
using MC = Microsoft.Maui.Controls;
using MCS = Microsoft.Maui.Controls.Shapes;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

#pragma warning disable MBB001

namespace BlazorBindings.Maui.Elements.Shapes
{
    public partial class Polyline : Shape
    {
        static Polyline()
        {
            RegisterAdditionalHandlers();
        }

        [Parameter] public MCS.FillRule? FillRule { get; set; }

        public new MCS.Polyline NativeControl => (MCS.Polyline)((BindableObject)this).NativeControl;

        protected override MCS.Polyline CreateNativeElement() => new();

        protected override void HandleParameter(string name, object value)
        {
            switch (name)
            {
                case nameof(FillRule):
                    if (!Equals(FillRule, value))
                    {
                        FillRule = (MCS.FillRule?)value;
                        NativeControl.FillRule = FillRule ?? (MCS.FillRule)MCS.Polyline.FillRuleProperty.DefaultValue;
                    }
                    break;

                default:
                    base.HandleParameter(name, value);
                    break;
            }
        }

        static partial void RegisterAdditionalHandlers();
    }
}
