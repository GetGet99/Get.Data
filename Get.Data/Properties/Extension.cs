using Get.Data.Bindings;

namespace Get.Data.Properties;

public static class Extension
{
    public static void BindOneWay<T>(this IProperty<T> property, IReadOnlyBinding<T> readOnlyBinding)
        => property.Bind(readOnlyBinding, ReadOnlyBindingModes.OneWay);
    public static void BindOneWayToTarget<T>(this IProperty<T> property, IReadOnlyBinding<T> readOnlyBinding)
        => property.Bind(readOnlyBinding, ReadOnlyBindingModes.OneWayToTarget);
    public static void BindOneWayToSource<T>(this IReadOnlyProperty<T> property, IBinding<T> readOnlyBinding)
        => property.BindOneWayToSource(readOnlyBinding);
}
