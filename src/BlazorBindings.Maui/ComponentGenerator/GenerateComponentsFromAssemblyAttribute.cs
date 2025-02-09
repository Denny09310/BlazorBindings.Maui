﻿namespace BlazorBindings.Maui.ComponentGenerator;

#pragma warning disable CS9113 // Parameter is unread. Type is used by Component Generator.

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class GenerateComponentsFromAssemblyAttribute(Type containingType) : Attribute
{
    /// <summary>
    /// Add prefix to generated component types names. E.g. if you have TypeNamePrefix="EX", component type for
    /// control 'Button' will be named 'EXButton',
    /// </summary>
    public string TypeNamePrefix { get; set; }

    /// <summary>
    /// Exclude specific types from generation.
    /// </summary>
    public Type[] Exclude { get; set; }

    /// <summary>
    /// By default, only types inherited from <see cref="Microsoft.Maui.Controls.Element"/> are included. 
    /// Set to true to include everything inherited from <see cref="Microsoft.Maui.Controls.BindableObject"/>.
    /// </summary>
    public bool IncludeNonElements { get; set; }
}