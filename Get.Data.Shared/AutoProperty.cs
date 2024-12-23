#pragma warning disable CS9113 // Parameter is unread.
using System;

namespace Get.Data.Properties;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class AutoPropertyAttribute : Attribute;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class IgnorePropertyAttribute : Attribute;


[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class PropertyNameOverrideAttribute(string Name) : Attribute;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class PropertySetEquivalentCheckAttribute(PropertySetEquivalentCheckKinds CheckKinds) : Attribute
{
    public string CustomBooleanExpression { get; set; } = "false";
}

public enum PropertySetEquivalentCheckKinds : byte
{
    DoNotCheck = 0,
    Auto = 1,
    ReferenceEquals = 2,
    DefaultEqualityComparer = 3,
    Custom = 4
}