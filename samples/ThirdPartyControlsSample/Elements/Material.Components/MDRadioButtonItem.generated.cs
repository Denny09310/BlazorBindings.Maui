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
using Microsoft.Maui.Graphics;
using System.Threading.Tasks;

#pragma warning disable MBB001

namespace BlazorBindings.Maui.Elements.Material.Components
{
    public partial class MDRadioButtonItem : SKTouchCanvasView
    {
        static MDRadioButtonItem()
        {
            RegisterAdditionalHandlers();
        }

        [Parameter] public string FontFamily { get; set; }
        [Parameter] public bool? FontItalic { get; set; }
        [Parameter] public float? FontSize { get; set; }
        [Parameter] public int? FontWeight { get; set; }
        [Parameter] public Color ForegroundColor { get; set; }
        [Parameter] public bool? IsSelected { get; set; }
        [Parameter] public Color OnColor { get; set; }
        [Parameter] public Color RippleColor { get; set; }
        [Parameter] public Color StateLayerColor { get; set; }
        [Parameter] public string Text { get; set; }
        [Parameter] public EventCallback<bool> IsSelectedChanged { get; set; }

        public new MCM.RadioButtonItem NativeControl => (MCM.RadioButtonItem)((BindableObject)this).NativeControl;

        protected override MCM.RadioButtonItem CreateNativeElement() => new();

        protected override void HandleParameter(string name, object value)
        {
            switch (name)
            {
                case nameof(FontFamily):
                    if (!Equals(FontFamily, value))
                    {
                        FontFamily = (string)value;
                        NativeControl.FontFamily = FontFamily;
                    }
                    break;
                case nameof(FontItalic):
                    if (!Equals(FontItalic, value))
                    {
                        FontItalic = (bool?)value;
                        NativeControl.FontItalic = FontItalic ?? (bool)MCM.RadioButtonItem.FontItalicProperty.DefaultValue;
                    }
                    break;
                case nameof(FontSize):
                    if (!Equals(FontSize, value))
                    {
                        FontSize = (float?)value;
                        NativeControl.FontSize = FontSize ?? (float)MCM.RadioButtonItem.FontSizeProperty.DefaultValue;
                    }
                    break;
                case nameof(FontWeight):
                    if (!Equals(FontWeight, value))
                    {
                        FontWeight = (int?)value;
                        NativeControl.FontWeight = FontWeight ?? (int)MCM.RadioButtonItem.FontWeightProperty.DefaultValue;
                    }
                    break;
                case nameof(ForegroundColor):
                    if (!Equals(ForegroundColor, value))
                    {
                        ForegroundColor = (Color)value;
                        NativeControl.ForegroundColor = ForegroundColor;
                    }
                    break;
                case nameof(IsSelected):
                    if (!Equals(IsSelected, value))
                    {
                        IsSelected = (bool?)value;
                        NativeControl.IsSelected = IsSelected ?? (bool)MCM.RadioButtonItem.IsSelectedProperty.DefaultValue;
                    }
                    break;
                case nameof(OnColor):
                    if (!Equals(OnColor, value))
                    {
                        OnColor = (Color)value;
                        NativeControl.OnColor = OnColor;
                    }
                    break;
                case nameof(RippleColor):
                    if (!Equals(RippleColor, value))
                    {
                        RippleColor = (Color)value;
                        NativeControl.RippleColor = RippleColor;
                    }
                    break;
                case nameof(StateLayerColor):
                    if (!Equals(StateLayerColor, value))
                    {
                        StateLayerColor = (Color)value;
                        NativeControl.StateLayerColor = StateLayerColor;
                    }
                    break;
                case nameof(Text):
                    if (!Equals(Text, value))
                    {
                        Text = (string)value;
                        NativeControl.Text = Text;
                    }
                    break;
                case nameof(IsSelectedChanged):
                    if (!Equals(IsSelectedChanged, value))
                    {
                        void NativeControlSelectedChanged(object sender, MCM.Core.SelectedChangedEventArgs e)
                        {
                            var value = NativeControl.IsSelected;
                            IsSelected = value;
                            InvokeEventCallback(IsSelectedChanged, value);
                        }

                        IsSelectedChanged = (EventCallback<bool>)value;
                        NativeControl.SelectedChanged -= NativeControlSelectedChanged;
                        NativeControl.SelectedChanged += NativeControlSelectedChanged;
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
