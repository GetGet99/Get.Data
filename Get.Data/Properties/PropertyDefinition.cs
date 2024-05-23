namespace Get.Data.Properties;


public sealed class PropertyDefinition<TOwnerType, TPropertyType>(Func<TOwnerType, IProperty<TPropertyType>> getProperty) : IPropertyDefinition<TOwnerType, TPropertyType>
{
    public IProperty<TPropertyType> GetProperty(TOwnerType owner)
        => getProperty(owner);
    IReadOnlyProperty<TPropertyType>
        IReadOnlyPropertyDefinition<TOwnerType, TPropertyType>.GetProperty(TOwnerType owner)
        => GetProperty(owner);
}
public sealed class ReadOnlyPropertyDefinition<TOwnerType, TPropertyType>(Func<TOwnerType, IReadOnlyProperty<TPropertyType>> getProperty) : IReadOnlyPropertyDefinition<TOwnerType, TPropertyType>
{
    public IReadOnlyProperty<TPropertyType> GetProperty(TOwnerType owner)
        => getProperty(owner);
}
public static class PropertyDefinition
{
    public static PropertyDefinition<TOwnerType, TPropertyType> Create<TOwnerType, TPropertyType>(Func<TOwnerType, PropertyBase<TPropertyType>> getProperty)
        => new(getProperty);
    public static PropertyDefinition<TOwnerType, TPropertyType> CreateExpr<TOwnerType, TPropertyType>(Func<TOwnerType, TPropertyType> getter, Action<TOwnerType, TPropertyType> setter, bool automaticNotifyOnSetValue = true)
        => Create<TOwnerType, TPropertyType>(owner => new ExprProperty<TPropertyType>(() => getter(owner), (x => setter(owner, x)), automaticNotifyOnSetValue));
    public static PropertyDefinition<TOwnerType, TPropertyType> CreateExpr<TOwnerType, TPropertyType>(Func<TOwnerType, TPropertyType> getter, Action<TOwnerType, TPropertyType> setter, Func<bool> writebackCondition, bool automaticNotifyOnSetValue = true)
        => CreateExpr(getter, (x, y) =>
        {
            if (writebackCondition()) setter(x, y);
        }, automaticNotifyOnSetValue: automaticNotifyOnSetValue);
}
public static class PropertyDefinitionExtension
{
    public static IPropertyDefinition<TNewOwnerType, TPropertyType> As<TOwnerType, TPropertyType, TNewOwnerType>(this IPropertyDefinition<TOwnerType, TPropertyType> pDef) where TNewOwnerType : TOwnerType
        => new PropertyDefinition<TNewOwnerType, TPropertyType>(x => pDef.GetProperty(x));
    public static IPropertyDefinition<TNewOwnerType, TPropertyType> As<TOwnerType, TPropertyType, TNewOwnerType>(this IPropertyDefinition<TOwnerType, TPropertyType> pDef, Func<TNewOwnerType, TOwnerType> caster)
        => new PropertyDefinition<TNewOwnerType, TPropertyType>(x => pDef.GetProperty(caster(x)));
    public static IReadOnlyPropertyDefinition<TNewOwnerType, TPropertyType> As<TOwnerType, TPropertyType, TNewOwnerType>(this IReadOnlyPropertyDefinition<TOwnerType, TPropertyType> pDef) where TNewOwnerType : TOwnerType
        => new ReadOnlyPropertyDefinition<TNewOwnerType, TPropertyType>(x => pDef.GetProperty(x));
    public static IReadOnlyPropertyDefinition<TNewOwnerType, TPropertyType> As<TOwnerType, TPropertyType, TNewOwnerType>(this IReadOnlyPropertyDefinition<TOwnerType, TPropertyType> pDef, Func<TNewOwnerType, TOwnerType> caster)
        => new ReadOnlyPropertyDefinition<TNewOwnerType, TPropertyType>(x => pDef.GetProperty(caster(x)));

    //public PropertyDefinition<TOwnerType, TNewPropertyType> WithConverter<TNewPropertyType>(Func<TPropertyType, TNewPropertyType> forwardConvert)
    //    => new(
    //        x => new PropertyWithConverter<TPropertyType, TNewPropertyType>(GetProperty(x), forwardConvert)
    //    );
    //public PropertyDefinition<TOwnerType, TNewPropertyType> WithConverter<TNewPropertyType>(Func<TPropertyType, TNewPropertyType> forwardConvert, Func<TNewPropertyType, TPropertyType> backwardConvert)
    //    => new(
    //        x => new PropertyWithConverter<TPropertyType, TNewPropertyType>(GetProperty(x), forwardConvert, backwardConvert)
    //    );
}