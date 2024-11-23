using Get.Data.Bindings;
using Get.Data.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace Get.Data.XACL;
public readonly struct QuickBinding<T>
{
    public QuickBinding(QuickReadOnlyBindingModes<T> aa) => a = aa;
    public QuickBinding(QuickBindingOneWayToSource<T> bb) => b = bb;
    public QuickBinding(QuickBindingModes<T> cc) => c = cc;
    readonly QuickReadOnlyBindingModes<T> a;
    readonly QuickBindingOneWayToSource<T> b;
    readonly QuickBindingModes<T> c;
    public void Bind(IProperty<T> property)
    {
        if (a != default) a.Bind(property);
        else if (b != default) b.Bind(property);
        else if (c != default) c.Bind(property);
        else property.RemoveBinding();
    }
    public static implicit operator QuickBinding<T>(QuickReadOnlyBindingModes<T> b) => new(b);
    public static implicit operator QuickBinding<T>(QuickBindingOneWayToSource<T> b) => new(b);
    public static implicit operator QuickBinding<T>(QuickBindingModes<T> b) => new(b);
}
public readonly record struct QuickBindingOneWayToSource<T>(IBinding<T> Binding)
{
    public void Bind(IReadOnlyProperty<T> property)
        => property.BindOneWayToSource(Binding);
}
public readonly record struct QuickBindingModes<T>(IBinding<T> Binding, BindingModes BindingModes)
{
    public void Bind(IProperty<T> property)
        => property.Bind(Binding, BindingModes);
}
public readonly record struct QuickReadOnlyBindingModes<T>(IReadOnlyBinding<T> Binding, ReadOnlyBindingModes ReadOnlyBindingMode)
{
    public void Bind(IProperty<T> property)
        => property.Bind(Binding, ReadOnlyBindingMode);
}
public static class QuickBindingExtension
{
    public static QuickReadOnlyBindingModes<T> OneWay<T>(IReadOnlyBinding<T> Binding)
        => new(Binding, ReadOnlyBindingModes.OneWay);
    public static QuickReadOnlyBindingModes<T> OneTime<T>(IReadOnlyBinding<T> Binding)
        => new(Binding, ReadOnlyBindingModes.OneTime);
    public static QuickReadOnlyBindingModes<T> OneWayToTarget<T>(IReadOnlyBinding<T> Binding)
        => new(Binding, ReadOnlyBindingModes.OneWayToTarget);
    public static QuickBindingOneWayToSource<T> OneWayToSource<T>(IBinding<T> Binding)
        => new(Binding);
    public static QuickBindingModes<T> TwoWay<T>(IBinding<T> Binding)
        => new(Binding, BindingModes.TwoWay);
    public static QuickBindingModes<T> TwoWayUpdateSourceImmediete<T>(IBinding<T> Binding)
        => new(Binding, BindingModes.TwoWayUpdateSourceImmediete);
    public static QuickBindingModes<T> TwoWayUpdateTargetImmediete<T>(IBinding<T> Binding)
            => new(Binding, BindingModes.TwoWayUpdateTargetImmediete);

}