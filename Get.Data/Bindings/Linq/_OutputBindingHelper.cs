using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Get.Data.Bindings.Linq;

readonly struct OutputReadOnlyBinding<T>(
    INotifyBinding<T> impl,
    Func<T> getter
) : IReadOnlyBinding<T>
{
    public T CurrentValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => getter();
    }

    public event Action RootChanged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        add => impl.RootChanged += value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        remove => impl.RootChanged -= value;
    }
    public event ValueChangingHandler<T>? ValueChanging
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        add => impl.ValueChanging += value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        remove => impl.ValueChanging -= value;
    }
    public event ValueChangedHandler<T>? ValueChanged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        add => impl.ValueChanged += value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        remove => impl.ValueChanged -= value;
    }
}

readonly struct OutputBinding<T>(
    INotifyBinding<T> impl,
    Func<T> getter,
    Action<T> setter
) : IBinding<T>
{
    public T CurrentValue {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => getter();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => setter(value);
    }

    public event Action RootChanged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        add => impl.RootChanged += value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        remove => impl.RootChanged -= value;
    }
    public event ValueChangingHandler<T>? ValueChanging
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        add => impl.ValueChanging += value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        remove => impl.ValueChanging -= value;
    }
    public event ValueChangedHandler<T>? ValueChanged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        add => impl.ValueChanged += value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        remove => impl.ValueChanged -= value;
    }
}
