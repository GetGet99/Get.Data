using System;
using System.Collections.Generic;
using System.Drawing;
using Get.EasyCSharp.GeneratorTools;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Get.EasyCSharp.GeneratorTools.SyntaxCreator;
using Get.EasyCSharp.GeneratorTools.SyntaxCreator.Members;
using Get.EasyCSharp.GeneratorTools.SyntaxCreator.Attributes;
using Get.EasyCSharp.GeneratorTools.SyntaxCreator.Lines;
using Get.EasyCSharp.GeneratorTools.SyntaxCreator.Expression;
using Get.Data.Properties;

namespace Get.EasyCSharp.Generator.PropertyGenerator;

[Generator]
[AddAttributeConverter(typeof(IgnorePropertyAttribute))]
[AddAttributeConverter(typeof(AutoPropertyAttribute))]
[AddAttributeConverter(typeof(PropertyNameOverrideAttribute), ParametersAsString = "\"\"")]
[AddAttributeConverter(typeof(PropertySetEquivalentCheckAttribute), ParametersAsString = "default")]
partial class AutoPropertyGenerator : AttributeBaseGenerator<
    AutoPropertyAttribute,
    AutoPropertyGenerator.AutoPropertyAttributeWarpper,
    TypeDeclarationSyntax,
    INamedTypeSymbol
>
{
    protected override bool CountAttributeSubclass => false;

    protected override AutoPropertyAttributeWarpper TransformAttribute(AttributeData attributeData, Compilation compilation)
    {
        return AttributeDataToAutoPropertyAttribute(attributeData, compilation);
    }

    protected override string? OnPointVisit(GeneratorSyntaxContext genContext, TypeDeclarationSyntax syntaxNode, INamedTypeSymbol symbol, (AttributeData Original, AutoPropertyAttributeWarpper Wrapper)[] attributeData)
    {
        return GetCode(genContext, symbol, attributeData, genContext.SemanticModel.Compilation).JoinNewLine();
    }
    IEnumerable<string> GetCode(GeneratorSyntaxContext genContext, INamedTypeSymbol cls, (AttributeData Original, AutoPropertyAttributeWarpper Wrapper)[] attributeDatas, Compilation compilation)
    {
        var IProperty = compilation.GetTypeByMetadataName("Get.Data.Properties.IProperty`1") ?? throw new NullReferenceException();
        var IReadOnlyProperty = compilation.GetTypeByMetadataName("Get.Data.Properties.IReadOnlyProperty`1") ?? throw new NullReferenceException();
        var IPropertyDefinition = compilation.GetTypeByMetadataName("Get.Data.Properties.IPropertyDefinition`2") ?? throw new NullReferenceException();
        var IReadOnlyPropertyDefinition = compilation.GetTypeByMetadataName("Get.Data.Properties.IReadOnlyPropertyDefinition`2") ?? throw new NullReferenceException();
        var PropertyDefinition = compilation.GetTypeByMetadataName("Get.Data.Properties.PropertyDefinition`2") ?? throw new NullReferenceException();
        var ReadOnlyPropertyDefinition = compilation.GetTypeByMetadataName("Get.Data.Properties.ReadOnlyPropertyDefinition`2") ?? throw new NullReferenceException();

        if (attributeDatas.Length is 0) yield break;
        bool isInterface = cls.TypeKind == TypeKind.Interface;
        foreach (var item in cls.GetMembers())
        {
            if (item is IPropertySymbol sym)
            {
                if (sym.Type is not INamedTypeSymbol outterType)
                    continue;
                byte propertyKind = outterType.IsImplementingNoGeneric(IReadOnlyProperty) ?
                    (
                        outterType.IsImplementingNoGeneric(IProperty) ? (byte)1 : (byte)2
                    ) : (byte)0;
                if (propertyKind is 0) continue;
                if (outterType.TypeArguments.Length is not 1)
                    continue;
                var ignorePropAttr = AttributeHelper.TryGetAttribute
                    <IgnorePropertyAttribute, IgnorePropertyAttributeWarpper>(
                    genContext, sym, AttributeDataToIgnorePropertyAttribute);
                if (ignorePropAttr is not null) continue;
                var innerType = outterType.TypeArguments[0];
                var providedNameAttr = AttributeHelper.TryGetAttribute
                    <PropertyNameOverrideAttribute, PropertyNameOverrideAttributeWarpper>(
                    genContext, sym, AttributeDataToPropertyNameOverrideAttribute)?.Serialized;
                var propertyName = ChooseName(sym.Name, providedNameAttr?.Name);
                var setEquivCheckAttr = AttributeHelper.TryGetAttribute
                    <PropertySetEquivalentCheckAttribute, PropertySetEquivalentCheckAttributeWarpper>(
                    genContext, sym, AttributeDataToPropertySetEquivalentCheckAttribute
                )?.Serialized ?? new(compilation) { CheckKinds = PropertySetEquivalentCheckKinds.Auto };
                var propName = new Variable(sym.Name);
                var propTypeImpl = new FullType($"{(propertyKind is 1 ? "global::Get.Data.Properties.IProperty" : "global::Get.Data.Properties.IReadOnlyProperty")}<{new FullType(innerType)}>");
                var propValueRef = propName.ForceCast(propTypeImpl).Dot("CurrentValue");
                yield return new Property(GetSyntaxVisiblity(sym.DeclaredAccessibility), new(innerType), propertyName)
                {
                    Documentation = new CustomDocumentation(
                    $"""
                        /// <summary>
                        /// <inheritdoc cref="{cls.Name}"/>
                        /// </summary>
                        """
                    ),
                    Get =
                    {
                        Shortened = isInterface,
                        Attributes =
                        {
                            () => new CustomAttribute("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
                        },
                        Code =
                        {
                            new Return(propValueRef)
                        }
                    },
                    Set =
                    {
                        Shortened = isInterface,
                        Visibility = propertyKind is 2 ? SyntaxVisibility.DoNotGenerate : SyntaxVisibility.Default,
                        Attributes =
                        {
                            () => new CustomAttribute("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
                        },
                        Code =
                        {
                            x => {

                                if (setEquivCheckAttr.CheckKinds is not
                                    (PropertySetEquivalentCheckKinds.ReferenceEquals or
                                    PropertySetEquivalentCheckKinds.DefaultEqualityComparer or
                                    PropertySetEquivalentCheckKinds.DoNotCheck or
                                    PropertySetEquivalentCheckKinds.Custom))
                                {
                                    setEquivCheckAttr.CheckKinds = innerType.IsValueType ?
                                       PropertySetEquivalentCheckKinds.DefaultEqualityComparer :
                                       PropertySetEquivalentCheckKinds.ReferenceEquals;
                                }
                                var methodCall = setEquivCheckAttr.CheckKinds switch
                                {
                                    PropertySetEquivalentCheckKinds.ReferenceEquals => $"object.ReferenceEquals({propValueRef}, value)",
                                    PropertySetEquivalentCheckKinds.DefaultEqualityComparer => $"global::System.Collections.Generic.EqualityComparer<{innerType.FullName()}>.Default.Equals({propValueRef}, value)",
                                    PropertySetEquivalentCheckKinds.Custom => setEquivCheckAttr.CustomBooleanExpression,
                                    PropertySetEquivalentCheckKinds.DoNotCheck => null,
                                    _ => throw new InvalidOperationException()
                                };
                                if (methodCall is not null)
                                    x.AddLast(new CustomLine(
                                        $"""
                                        if ({methodCall}) return;
                                        """
                                    ));
                            },
                            new Assign(propValueRef, new CustomExpression("value")).EndLine(),
                            //() => attr.OnChanged is not null ? new MethodCall(attr.OnChanged).EndLine() : null,
                            //list => OnSet(list, cls, propertyName, originalattr, compilation)
                        }
                    }
                }.StringRepresentaion;
                const string IPropertyDefinitionStr = "global::Get.Data.Properties.IPropertyDefinition";
                const string IReadOnlyPropertyDefinitionStr = "global::Get.Data.Properties.IReadOnlyPropertyDefinition";
                if (!isInterface)
                    yield return new Property(GetSyntaxVisiblity(sym.DeclaredAccessibility),
                        new($"{(propertyKind is 1 ? IPropertyDefinitionStr : IReadOnlyPropertyDefinitionStr)}<{new FullType(cls)}, {new FullType(innerType)}>"),
                        propertyName + "PropertyDefinition")
                    {
                        Static = true,
                        Documentation = new CustomDocumentation(
                        $"""
                        /// <summary>
                        /// <inheritdoc cref="{cls.Name}"/>
                        /// </summary>
                        """
                        ),
                        Get = { Shortened = true },
                        Set = { Visibility = SyntaxVisibility.DoNotGenerate },
                        CustomEqualExpression = new CustomExpression($"""
                            new {(propertyKind is 1 ? "global::Get.Data.Properties.PropertyDefinition" : "global::Get.Data.Properties.ReadOnlyPropertyDefinition")}<{new FullType(cls)}, {new FullType(innerType)}>
                            (x => x.{propName})
                            """)
                    }.StringRepresentaion;
                const string QuickBindingStr = "global::Get.Data.XACL.QuickBinding";
                const string QuickBindingOneWayToSourceStr = "global::Get.Data.XACL.QuickBindingOneWayToSource";
                yield return new Property(GetSyntaxVisiblity(sym.DeclaredAccessibility), new($"{(propertyKind is 1 ? QuickBindingStr : QuickBindingOneWayToSourceStr)}<{new FullType(innerType)}>"), propertyName + "Binding")
                {
                    Documentation = new CustomDocumentation(
                    $"""
                        /// <summary>
                        /// <inheritdoc cref="{cls.Name}"/>
                        /// </summary>
                        """
                    ),
                    Get = { Visibility = SyntaxVisibility.DoNotGenerate },
                    Set =
                    {
                        Shortened = isInterface,
                        Attributes =
                        {
                            () => new CustomAttribute("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
                        },
                        Code =
                        {
                            new CustomLine($"value.Bind({propName});"),
                            //() => attr.OnChanged is not null ? new MethodCall(attr.OnChanged).EndLine() : null,
                            //list => OnSet(list, cls, propertyName, originalattr, compilation)
                        }
                    }
                }.StringRepresentaion;
            }
        }
        
        yield break;
    }
    protected virtual void OnPropertyVisited(LinkedList<ILine> Lines, IFieldSymbol symbol, string PropertyName, AttributeData data, Compilation compilation) { }
    protected virtual void OnSet(LinkedList<ILine> Lines, IFieldSymbol symbol, string PropertyName, AttributeData data, Compilation compilation) { }
    static SyntaxVisibility GetSyntaxVisiblity(Accessibility propertyAccessibility)
        => propertyAccessibility switch
        {
            Accessibility.Public => SyntaxVisibility.Public,
            Accessibility.ProtectedOrFriend or Accessibility.ProtectedAndInternal => SyntaxVisibility.Protected,
            Accessibility.Private => SyntaxVisibility.Private,
            Accessibility.Internal => SyntaxVisibility.Internal,
            _ => SyntaxVisibility.Default
        };
    static string ChooseName(string fieldName, string? overridenNameOpt)
    {
        if (overridenNameOpt is not null) return overridenNameOpt;

        if (fieldName.EndsWith("Property"))
        {
            var cut = fieldName[..^"Property".Length];
            return cut;
        }
        if (fieldName.EndsWith("Prop"))
        {
            var cut = fieldName[..^"Prop".Length];
            return cut;
        }
        // not a great fallback name, but... yeah
        return fieldName + "Out";
        //fieldName = fieldName.TrimStart('_');
        //if (fieldName.Length == 0)
        //    return string.Empty;

        //if (fieldName.Length == 1)
        //    return fieldName.ToUpper();

        //return fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
    }
}
