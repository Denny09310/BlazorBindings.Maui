// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using MC = Microsoft.Maui.Controls;
using Microsoft.AspNetCore.Components;
using BlazorBindings.Core;
using BlazorBindings.Maui.Elements.Handlers;
using System.Threading.Tasks;

namespace BlazorBindings.Maui.Elements
{
    public partial class Tab : ShellSection
    {
        static Tab()
        {
            ElementHandlerRegistry.RegisterElementHandler<Tab>(
                renderer => new TabHandler(renderer, new MC.Tab()));

            RegisterAdditionalHandlers();
        }

        public new MC.Tab NativeControl => ((TabHandler)ElementHandler).TabControl;

        protected override void RenderAttributes(AttributesBuilder builder)
        {
            base.RenderAttributes(builder);


            RenderAdditionalAttributes(builder);
        }

        partial void RenderAdditionalAttributes(AttributesBuilder builder);

        static partial void RegisterAdditionalHandlers();
    }
}