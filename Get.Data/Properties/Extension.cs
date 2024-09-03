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
    /// <summary>
    /// Adds <paramref name="valueChangedHandler"/> to <see cref="INotifyBinding{T}.ValueChanged"/>.
    /// Then, calls <paramref name="valueChangedHandler"/> with property's current value (on both old and new parameter)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property"></param>
    /// <param name="valueChangedHandler"></param>
    public static void ApplyAndRegisterForNewValue<T>(this IReadOnlyBinding<T> property, ValueChangedHandler<T> valueChangedHandler)
    {
        property.ValueChanged += valueChangedHandler;
        valueChangedHandler(property.CurrentValue, property.CurrentValue);
    }
}
