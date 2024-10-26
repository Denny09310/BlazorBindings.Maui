using BlazorBindings.Maui;
using System.Reflection;

namespace NewApp;

internal partial class App(IServiceProvider services) : BlazorBindingsApplication<AppShell>(services)
{
    protected override void Configure()
    {
        var resources = typeof(App).Assembly.GetCustomAttributes<XamlResourceIdAttribute>()
            .Where(attribute => Path.GetDirectoryName(attribute.Path) == "Resources/Styles" && Path.GetExtension(attribute.Path) == ".xaml")
            .Select(attribute => Activator.CreateInstance(attribute.Type))
            .OfType<ResourceDictionary>();

        foreach (var resource in resources)
        {
            Resources.Add(resource);
        }
    }
}
