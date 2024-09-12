using Get.Data.Bindings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Get.Data.Properties;

public readonly struct ReadOnlyPropertyConstant<T>(T val) : IReadOnlyProperty<T>
{
    public T CurrentValue { get => val; }

    public event Action RootChanged { add { } remove { } }
    public event ValueChangingHandler<T>? ValueChanging { add { } remove { } }
    public event ValueChangedHandler<T>? ValueChanged { add { } remove { } }

    public void BindOneWayToSource(IBinding<T> binding)
        => binding.CurrentValue = val;

    public void RemoveBinding() {
        
    }
}
/// <summary>
/// Wraps around <see cref="IReadOnlyProperty{T}"/> to ensure that it can't be casted back.
/// </summary>
/// <typeparam name="T">The type parameter</typeparam>
/// <param name="property">The property to be wrapped</param>
public readonly struct ReadOnlyProperty<T>(IReadOnlyProperty<T> property) : IReadOnlyProperty<T>
{
    // Backward Compatable Reasons
    [Obsolete("Please use `new ReadOnlyPropertyConstant<T>(T Val)` or `AutoProperty.AutoReadOnly<T>(T Val)` for beter performance")]
    public ReadOnlyProperty(T val) : this(new ReadOnlyPropertyConstant<T>(val)) { }
    public T CurrentValue => property.CurrentValue;

    public event Action RootChanged
    {
        add => property.RootChanged += value;
        remove => property.RootChanged -= value;
    }
    public event ValueChangingHandler<T>? ValueChanging
    {
        add => property.ValueChanging += value;
        remove => property.ValueChanging -= value;
    }
    public event ValueChangedHandler<T>? ValueChanged
    {
        add => property.ValueChanged += value;
        remove => property.ValueChanged -= value;
    }

    public void BindOneWayToSource(IBinding<T> binding)
        => property.BindOneWayToSource(binding);

    public void RemoveBinding()
        => property.RemoveBinding();
}
