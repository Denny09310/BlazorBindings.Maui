﻿using BlazorBindings.Maui.ComponentGenerator.Extensions;
using Microsoft.CodeAnalysis;

namespace BlazorBindings.Maui.ComponentGenerator;

public partial class GeneratedPropertyInfo
{
    private readonly IPropertySymbol _propertyInfo;
    private readonly ITypeSymbol _propertyType;
    private Lazy<string> _componentPropertyNameLazy;
    private Lazy<string> _componentTypeLazy;

    public GeneratedTypeInfo ContainingType { get; set; }
    public GeneratedPropertyKind Kind { get; }
    public string MauiPropertyName { get; set; }
    public string MauiContainingTypeName { get; }
    public string ComponentName { get; }
    public Compilation Compilation { get; }
    public bool IsGeneric { get; }
    public INamedTypeSymbol GenericTypeArgument { get; }
    public string ComponentPropertyName
    {
        get => _componentPropertyNameLazy.Value;
        set => _componentPropertyNameLazy = new Lazy<string>(value);
    }

    public string ComponentType
    {
        get => _componentTypeLazy.Value;
        set => _componentTypeLazy = new Lazy<string>(value);
    }

    private GeneratedPropertyInfo(GeneratedTypeInfo typeInfo,
                                  ITypeSymbol mauiPropertyType,
                                  string mauiPropertyName,
                                  string mauiContainingTypeName,
                                  string componentPropertyName,
                                  string componentType,
                                  GeneratedPropertyKind kind)
    {
        ContainingType = typeInfo;
        Compilation = typeInfo.Compilation;
        ComponentName = typeInfo.TypeName;
        Kind = kind;
        MauiPropertyName = mauiPropertyName;
        MauiContainingTypeName = mauiContainingTypeName;
        _propertyType = mauiPropertyType;
        _componentTypeLazy = new Lazy<string>(componentType);
        _componentPropertyNameLazy = new Lazy<string>(componentPropertyName);
    }

    private GeneratedPropertyInfo(GeneratedTypeInfo typeInfo, IPropertySymbol propertyInfo, GeneratedPropertyKind kind)
    {
        _propertyInfo = propertyInfo;
        _propertyType = propertyInfo.Type;
        Kind = kind;

        ContainingType = typeInfo;
        Compilation = typeInfo.Compilation;
        ComponentName = typeInfo.TypeName;
        MauiPropertyName = ComponentWrapperGenerator.GetIdentifierName(propertyInfo.Name);
        IsGeneric = typeInfo.Settings.GenericProperties.TryGetValue(propertyInfo.Name, out var genericTypeArgument);
        GenericTypeArgument = genericTypeArgument;
        MauiContainingTypeName = GetTypeNameAndAddNamespace(propertyInfo.ContainingType);

        ComponentName = ComponentWrapperGenerator.GetIdentifierName(_propertyInfo.ContainingType.Name);
        _componentPropertyNameLazy = new Lazy<string>(GetComponentPropertyName);
        _componentTypeLazy = new Lazy<string>(() => GetComponentPropertyTypeName(_propertyInfo, typeInfo, IsRenderFragmentProperty, makeNullable: true));

        string GetComponentPropertyName()
        {
            if (ContainingType.Settings.Aliases.TryGetValue(MauiPropertyName, out var aliasName))
                return aliasName;

            if (IsRenderFragmentProperty && _propertyInfo.ContainingType.GetAttributes().Any(a
                => a.AttributeClass.Name == "ContentPropertyAttribute" && Equals(a.ConstructorArguments.FirstOrDefault().Value, _propertyInfo.Name)))
            {
                return "ChildContent";
            }

            return ComponentWrapperGenerator.GetIdentifierName(_propertyInfo.Name);
        }
    }

    public string GetPropertyDeclaration()
    {
        // razor compiler doesn't allow 'new' properties, it considers them as duplicates.
        if (_propertyInfo is not null && _propertyInfo.IsHidingMember())
        {
            return "";
        }

        const string indent = "        ";

        var xmlDocContents = _propertyInfo is null ? "" : ComponentWrapperGenerator.GetXmlDocContents(_propertyInfo, indent);

        return $@"{xmlDocContents}{indent}[Parameter] public {ComponentType} {ComponentPropertyName} {{ get; set; }}
";
    }

    public string GetHandleValueProperty()
    {
        var propName = ComponentPropertyName;

        return $@"                case nameof({propName}):
                    if (!Equals({propName}, value))
                    {{
                        {propName} = ({ComponentType})value;
                        NativeControl.{MauiPropertyName} = {GetConvertedProperty(_propertyType, propName)};
                    }}
                    break;
";

        string GetConvertedProperty(ITypeSymbol propertyType, string propName)
        {
            if (propertyType is INamedTypeSymbol namedType)
            {
                if (namedType.IsGenericType
                    && namedType.ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IList_T
                    && namedType.TypeArguments[0].SpecialType == SpecialType.System_String)
                {
                    return $"AttributeHelper.GetStringList({propName})";
                }

                if (namedType.IsValueType && !namedType.IsNullableStruct())
                {
                    var hasBindingProperty = !_propertyInfo.ContainingType.GetMembers($"{propName}Property").IsEmpty;
                    var defaultValue = hasBindingProperty
                        ? $"({GetTypeNameAndAddNamespace(propertyType)}){MauiContainingTypeName}.{propName}Property.DefaultValue"
                        : "default";

                    return $"{propName} ?? {defaultValue}";
                }

                if (IsGeneric && namedType.GetFullName() == "Microsoft.Maui.Controls.BindingBase")
                {
                    return $"AttributeHelper.GetBinding({propName})";
                }

                if (IsGeneric && namedType.GetFullName() == "System.Collections.IList")
                {
                    return $"AttributeHelper.GetIList({propName})";
                }
            }

            return propName;
        }
    }

    internal static GeneratedPropertyInfo[] GetValueProperties(GeneratedTypeInfo generatedType)
    {
        var props = GetMembers<IPropertySymbol>(generatedType.Settings.TypeSymbol, generatedType.Settings.Include)
            .Where(p => IsValueProperty(p, generatedType))
            .OrderBy(prop => prop.Name, StringComparer.OrdinalIgnoreCase);

        return props.Select(prop =>
            {
                if (prop.Type.GetFullName() == "Microsoft.Maui.Controls.Brush")
                {
                    var propName = prop.Name.Replace("Brush", "") + "Color";

                    if (prop.ContainingType.GetProperty(propName, includeBaseTypes: true) != null)
                    {
                        return null;
                    }

                    return new GeneratedPropertyInfo(generatedType, prop, GeneratedPropertyKind.Value)
                    {
                        ComponentPropertyName = propName,
                        ComponentType = generatedType.GetTypeNameAndAddNamespace("Microsoft.Maui.Graphics", "Color")
                    };
                }
                else
                {
                    return new GeneratedPropertyInfo(generatedType, prop, GeneratedPropertyKind.Value);
                }
            })
            .Where(p => p != null)
            .ToArray();
    }

    private static bool IsValueProperty(IPropertySymbol prop, GeneratedTypeInfo generatedType)
    {
        if (generatedType.Settings.Exclude.Contains(prop.Name))
            return false;

        if (generatedType.Settings.ContentProperties.Contains(prop.Name))
            return false;

        return IsPublicProperty(prop)
            && HasPublicSetter(prop)
            && (IsExplicitlyAllowed(prop, generatedType) || (!IsDisallowed(prop) && !IsCommandParameter(prop)))
            && (prop.Type.GetFullName() == "Microsoft.Maui.Controls.Brush" || !IsRenderFragmentPropertySymbol(generatedType, prop));

        static bool IsDisallowed(IPropertySymbol prop) => DisallowedComponentTypes.Contains(prop.Type.GetFullName());
        static bool IsCommandParameter(IPropertySymbol prop) => prop.Type.SpecialType == SpecialType.System_Object && prop.Name.EndsWith("CommandParameter");
    }

    private static string GetComponentPropertyTypeName(IPropertySymbol propertySymbol, GeneratedTypeInfo containingType, bool isRenderFragmentProperty = false, bool makeNullable = false)
    {
        var typeSymbol = propertySymbol.Type;
        var isGeneric = containingType.Settings.GenericProperties.TryGetValue(propertySymbol.Name, out var typeArgument);
        var typeArgumentName = typeArgument is null ? "T" : containingType.GetTypeNameAndAddNamespace(typeArgument);

        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return containingType.GetTypeNameAndAddNamespace(typeSymbol);
        }
        else if (namedTypeSymbol.IsGenericType
            && namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IList_T
            && namedTypeSymbol.TypeArguments[0].SpecialType == SpecialType.System_String)
        {
            // Lists of strings are special-cased because they are handled specially by the handlers as a comma-separated list
            return "string";
        }
        else if (isRenderFragmentProperty)
        {
            return isGeneric ? $"RenderFragment<{typeArgumentName}>" : "RenderFragment";
        }
        else if (makeNullable && namedTypeSymbol.IsValueType && !namedTypeSymbol.IsNullableStruct())
        {
            return containingType.GetTypeNameAndAddNamespace(typeSymbol) + "?";
        }
        else if (namedTypeSymbol.SpecialType == SpecialType.System_Collections_IEnumerable && isGeneric)
        {
            return containingType.GetTypeNameAndAddNamespace("System.Collections.Generic", $"IEnumerable<{typeArgumentName}>");
        }
        else if (namedTypeSymbol.GetFullName() == "System.Collections.IList" && isGeneric)
        {
            return containingType.GetTypeNameAndAddNamespace("System.Collections.Generic", $"IList<{typeArgumentName}>");
        }
        else if (namedTypeSymbol.GetFullName() == "Microsoft.Maui.Controls.BindingBase" && isGeneric)
        {
            return "Func<T, string>";
        }
        else if (namedTypeSymbol.SpecialType == SpecialType.System_Object && isGeneric)
        {
            return typeArgumentName;
        }
        else
        {
            return containingType.GetTypeNameAndAddNamespace(namedTypeSymbol);
        }
    }

    private static bool IsExplicitlyAllowed(IPropertySymbol propertyInfo, GeneratedTypeInfo generatedType)
    {
        return generatedType.Settings.Include.Contains(propertyInfo.Name)
            || propertyInfo.Type.SpecialType == SpecialType.System_Object && generatedType.Settings.GenericProperties.ContainsKey(propertyInfo.Name);
    }

    private static bool IsPublicProperty(IPropertySymbol propertyInfo)
    {
        return propertyInfo.GetMethod?.DeclaredAccessibility == Accessibility.Public
            && !propertyInfo.IsIndexer
            && propertyInfo.IsBrowsable()
            && !propertyInfo.IsObsolete();
    }

    private static bool HasPublicSetter(IPropertySymbol propertyInfo)
    {
        return propertyInfo.SetMethod?.DeclaredAccessibility == Accessibility.Public;
    }

    private static readonly List<string> DisallowedComponentTypes =
    [
        "Microsoft.Maui.Controls.Button.ButtonContentLayout", // TODO: This is temporary; should be possible to add support later
        "Microsoft.Maui.Controls.ColumnDefinitionCollection",
        "Microsoft.Maui.Controls.PointCollection",
        "Microsoft.Maui.Controls.DoubleCollection",
        "Microsoft.Maui.Controls.ControlTemplate",
        "Microsoft.Maui.Controls.DataTemplate",
        "Microsoft.Maui.Controls.Element",
        "Microsoft.Maui.Font", // TODO: This is temporary; should be possible to add support later
        "Microsoft.Maui.Graphics.Font", // TODO: This is temporary; should be possible to add support later
        "Microsoft.Maui.Controls.FormattedString",
        "Microsoft.Maui.Controls.Shapes.Geometry",
        "Microsoft.Maui.Controls.GradientStopCollection",
        "System.Windows.Input.ICommand",
        "Microsoft.Maui.Controls.Command",
        "Microsoft.Maui.Controls.Page",
        "Microsoft.Maui.Controls.RowDefinitionCollection",
        "Microsoft.Maui.Controls.Shadow",
        "Microsoft.Maui.Controls.ShellContent",
        "Microsoft.Maui.Controls.ShellItem",
        "Microsoft.Maui.Controls.ShellSection",
        "Microsoft.Maui.Controls.IVisual",
        "Microsoft.Maui.Controls.View",
        "Microsoft.Maui.Graphics.IShape",
        "Microsoft.Maui.IView",
        "Microsoft.Maui.IViewHandler"
    ];

    private string GetTypeNameAndAddNamespace(ITypeSymbol type)
    {
        return ContainingType.GetTypeNameAndAddNamespace(type);
    }

    private string GetTypeNameAndAddNamespace(string @namespace, string typeName)
    {
        return ContainingType.GetTypeNameAndAddNamespace(@namespace, typeName);
    }

    private static IEnumerable<T> GetMembers<T>(ITypeSymbol typeSymbol, IEnumerable<string> includeBaseMemberNames) where T : ISymbol
    {
        var baseMembers = includeBaseMemberNames
            .Select(member => typeSymbol.GetMember(member, true))
            .Where(member => member != null);

        IEnumerable<ISymbol> members = typeSymbol.GetMembers();

        // Since we don't generate components for generic types, we include them here.
        if (typeSymbol.BaseType != null && typeSymbol.BaseType.IsGenericType)
        {
            members = members.Concat(typeSymbol.BaseType.GetMembers());
        }

        return members.Union(baseMembers, SymbolEqualityComparer.Default)
            .Where(m => !m.IsStatic && m.DeclaredAccessibility == Accessibility.Public)
            .OfType<T>();
    }
}
