using Get.Data.Bindings;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Get.Data.Properties;

public abstract class PropertyBase<T> : IProperty<T>
{
#if DEBUG
    public string DebugName { get; set; } = $"Property<{typeof(T).Name}>";
    public bool BreakOnSet { get; set; } = false;
    public override string ToString() => DebugName;
#endif
    public abstract T Value { get; set; }
    T IDataBinding<T>.CurrentValue { get => Value; set => Value = value; }

    T IReadOnlyDataBinding<T>.CurrentValue => Value;

    public abstract event ValueChangingHandler<T>? ValueChanging;
    public abstract event ValueChangedHandler<T>? ValueChanged;
    IReadOnlyBinding<T>? currentBinding;

    // RootChanged event will never be sent
    event Action INotifyBinding<T>.RootChanged
    {
        add { }
        remove { }
    }

    public void Bind(IReadOnlyBinding<T> binding, ReadOnlyBindingModes bindingMode)
    {
        RemoveBinding();
        currentBinding = binding;
        switch (bindingMode)
        {
            case ReadOnlyBindingModes.OneTime:
                Value = currentBinding.CurrentValue;
                currentBinding.RootChanged += BindingRootChanged;
                break;
            case ReadOnlyBindingModes.OneWayToTarget:
                Value = currentBinding.CurrentValue;
                currentBinding.ValueChanged += SourceBindingValueChanged;
                break;
        }
    }
    public void Bind(IBinding<T> binding, BindingModes bindingMode)
    {
        RemoveBinding();
        currentBinding = binding;
        switch (bindingMode)
        {
            case BindingModes.OneWayToSource:
                binding.CurrentValue = Value;
                ValueChanged += ValueChangedToSourceBinding;
                break;
            case BindingModes.TwoWayUpdateSourceImmediete:
                binding.CurrentValue = Value;
                ValueChanged += ValueChangedToSourceBinding;
                currentBinding.ValueChanged += SourceBindingValueChanged;
                break;
            case BindingModes.TwoWayUpdateTargetImmediete:
                Value = currentBinding.CurrentValue;
                ValueChanged += ValueChangedToSourceBinding;
                currentBinding.ValueChanged += SourceBindingValueChanged;
                break;
        }
    }
    void IReadOnlyProperty<T>.BindOneWayToSource(IBinding<T> binding) => Bind(binding, BindingModes.OneWayToSource);
    private void BindingRootChanged()
    {
        if (currentBinding != null)
            Value = currentBinding.CurrentValue;
    }

    void SourceBindingValueChanged(T oldVal, T newVal)
    {
        Value = newVal;
    }
    void ValueChangedToSourceBinding(T oldVal, T newVal)
    {
        if (currentBinding != null && currentBinding is IBinding<T> readWriteBinding)
            readWriteBinding.CurrentValue = newVal;
    }

    public void RemoveBinding()
    {
        if (currentBinding is not null)
        {
            ValueChanged -= ValueChangedToSourceBinding;
            currentBinding.ValueChanged -= SourceBindingValueChanged;
        }
    }
}
public class Property<T>(T defaultValue) : PropertyBase<T>
{
    T val = defaultValue;
    public override T Value
    {
        get => val;
        set
        {
#if DEBUG
            if (BreakOnSet) Debugger.Break();
#endif
            if (EqualityComparer<T>.Default.Equals(val, value))
                return;
            var oldValue = val;
            ValueChanging?.Invoke(oldValue, value);
            val = value;
            ValueChanged?.Invoke(oldValue, value);
        }
    }

    public override event ValueChangingHandler<T>? ValueChanging;
    public override event ValueChangedHandler<T>? ValueChanged;
}
public class ExprProperty<T> : PropertyBase<T>
{
    readonly Func<T> _onGetValue;
    readonly Action<T> _onSetValue;
    public ExprProperty(Func<T> onGetValue, Action<T> onSetValue, bool automaticNotifyOnSetValue = true)
    {
        _onGetValue = onGetValue;
        if (automaticNotifyOnSetValue)
            _onSetValue = (value) =>
            {
                onSetValue(value);
            };
        else
            _onSetValue = onSetValue;
    }

    public override T Value { get => _onGetValue(); set => _onSetValue(value); }

    public override event ValueChangingHandler<T>? ValueChanging;
    public override event ValueChangedHandler<T>? ValueChanged;

}
class PropertyWithConverter<TInput, TOutput>(PropertyBase<TInput> originalProperty, Func<TInput, TOutput> forwardConvert, Func<TOutput, TInput> backwardConvert) : PropertyBase<TOutput>
{
    public PropertyWithConverter(PropertyBase<TInput> originalProperty, Func<TInput, TOutput> forwardConvert)
        : this(originalProperty, forwardConvert, x => throw new InvalidOperationException("Backward Converter was not defined")) { }
    readonly PropertyBase<TInput> originalProperty = originalProperty;
    readonly Func<TInput, TOutput> forwardConvert = forwardConvert;
    readonly Func<TOutput, TInput> backwardConvert = backwardConvert;

    public override TOutput Value
    {
        get => forwardConvert(originalProperty.Value);
        set => originalProperty.Value = backwardConvert(value);
    }
    ValueChangingHandler<TOutput>? _ValueChanging;
    public override event ValueChangingHandler<TOutput>? ValueChanging
    {
        add
        {
            if (_ValueChanging is null)
            {
                originalProperty.ValueChanging += ParentValueChangingCallback;
            }
            _ValueChanging += value;
        }
        remove
        {
            _ValueChanging -= value;
            if (_ValueChanging is null)
            {
                originalProperty.ValueChanging -= ParentValueChangingCallback;
            }
        }
    }
    ValueChangedHandler<TOutput>? _ValueChanged;
    public override event ValueChangedHandler<TOutput>? ValueChanged
    {
        add
        {
            if (_ValueChanged is null)
            {
                originalProperty.ValueChanged += ParentValueChangedCallback;
            }
            _ValueChanged += value;
        }
        remove
        {
            _ValueChanged -= value;
            if (_ValueChanged is null)
            {
                originalProperty.ValueChanged -= ParentValueChangedCallback;
            }
        }
    }
    void ParentValueChangingCallback(TInput oldValue, TInput newValue)
        => _ValueChanging?.Invoke(forwardConvert(oldValue), forwardConvert(newValue));
    void ParentValueChangedCallback(TInput oldValue, TInput newValue)
        => _ValueChanged?.Invoke(forwardConvert(oldValue), forwardConvert(newValue));
}
public class AsyncWriteProperty<T>(T defaultValue) : IAsyncWriteProperty<T>
{
    public async Task SetValueAsync(T value)
    {
        var oldValue = CurrentValue;
        if (ValueChangingAsync != null)
            foreach (var handler in
                ValueChangingAsync.GetInvocationList()
                .Cast<AsyncValueChangingHandler<T>>()
            ) await handler(oldValue, value);
        _ValueChanging?.Invoke(oldValue, value);
        CurrentValue = value;
        if (ValueChangedAsync != null)
            foreach (var handler in
                ValueChangedAsync.GetInvocationList()
                .Cast<AsyncValueChangedHandler<T>>()
            ) await handler(oldValue, value);
        _ValueChanged?.Invoke(oldValue, value);
    }
    public T CurrentValue { get; private set; } = defaultValue;

    // RootChanged event will never be sent
    event Action INotifyBinding<T>.RootChanged
    {
        add { }
        remove { }
    }
    event Func<Task> IAsyncNotifyBinding<T>.RootChangedAsync
    {
        add { }
        remove { }
    }

    ValueChangingHandler<T>? _ValueChanging;
    ValueChangedHandler<T>? _ValueChanged;
    event ValueChangingHandler<T>? INotifyBinding<T>.ValueChanging
    {
        add => _ValueChanging += value;
        remove => _ValueChanging += value;
    }

    event ValueChangedHandler<T>? INotifyBinding<T>.ValueChanged
    {
        add => _ValueChanged += value;
        remove => _ValueChanged += value;
    }

    public event AsyncValueChangingHandler<T>? ValueChangingAsync;
    public event AsyncValueChangedHandler<T>? ValueChangedAsync;

    IReadOnlyBinding<T>? currentBinding;
    void ValueChangedToSourceBinding(T oldVal, T newVal)
    {
        if (currentBinding != null && currentBinding is IBinding<T> readWriteBinding)
            readWriteBinding.CurrentValue = newVal;
    }
    public void BindOneWayToSource(IBinding<T> binding)
    {
        currentBinding = binding;
        binding.CurrentValue = CurrentValue;
        _ValueChanged += ValueChangedToSourceBinding;
    }

    public void RemoveBinding()
    {
        currentBinding = null;
        _ValueChanged -= ValueChangedToSourceBinding;
    }

}