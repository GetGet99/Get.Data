using Get.Data.Bindings;

namespace Get.Data.Properties;

public delegate void AttachedPropertyValueChangingHandler<TBindable, TProperty>(TBindable parent, TProperty oldValue, TProperty newValue);
public delegate void AttachedPropertyValueChangedHandler<TBindable, TProperty>(TBindable parent, TProperty oldValue, TProperty newValue);
public class AttachedProperty<TBindable, TProperty>(TProperty defaultValue) : IPropertyDefinition<TBindable, TProperty> where TBindable : notnull
{
    public event AttachedPropertyValueChangingHandler<TBindable, TProperty>? ValueChanging;
    public event AttachedPropertyValueChangedHandler<TBindable, TProperty>? ValueChanged;
    readonly Dictionary<TBindable, Property<TProperty>> dict = [];
    public IProperty<TProperty> GetProperty(TBindable owner)
    {
        if (!dict.TryGetValue(owner, out var property))
        {
            dict[owner] = property = new Property<TProperty>(defaultValue);
            property.ValueChanging += (oldVal, newVal) => ValueChanging?.Invoke(owner, oldVal, newVal);
            property.ValueChanged += (oldVal, newVal) => ValueChanged?.Invoke(owner, oldVal, newVal);
        }
        return property;
    }

    /// <summary>
    /// When the property value for <paramref name="source"/> changes,
    /// the property value for <paramref name="target"/> will be updated to match.
    /// </summary>
    /// <param name="source">The source item</param>
    /// <param name="target">The target item</param>
    /// <remarks>
    /// If there were old binding for target, it will be replaced.
    /// </remarks>
    public void InheritValue(TBindable source, TBindable target)
    {
        GetProperty(target).BindOneWay(GetProperty(source));
    }
    /// <summary>
    /// When the property value for <paramref name="source"/> changes,
    /// the property value for <paramref name="target"/> will be updated to match.
    /// </summary>
    /// <param name="source">The source item</param>
    /// <param name="target">The target item</param>
    /// <remarks>
    /// If there were old binding for target, it will be replaced.
    /// </remarks>
    public void InheritProperty(IReadOnlyBinding<TProperty> source, TBindable target)
    {
        GetProperty(target).BindOneWay(source);
    }

    /// <summary>
    /// Shorthand for <c>GetProperty(owner).CurrentValue</c>
    /// </summary>
    /// <param name="owner">The type that the property is attached to</param>
    /// <returns>The value in the property. If it is never set before, it returns the default value.</returns>
    public TProperty GetValue(TBindable owner)
        => GetProperty(owner).CurrentValue;
    /// <summary>
    /// Shorthand for <c>GetProperty(owner).CurrentValue = value</c>
    /// </summary>
    /// <param name="owner">The type that the property is attached to</param>
    public void SetValue(TBindable owner, TProperty value)
        => GetProperty(owner).CurrentValue = value;

    IReadOnlyProperty<TProperty> IReadOnlyPropertyDefinition<TBindable, TProperty>.GetProperty(TBindable owner)
        => GetProperty(owner);
}
public class AttachedProperty<TPropertyType>(TPropertyType defaultValue) : AttachedProperty<object, TPropertyType>(defaultValue)
{

}