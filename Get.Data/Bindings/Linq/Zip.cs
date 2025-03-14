using Get.Data.Helpers;

namespace Get.Data.Bindings.Linq;
public delegate TOut ZipForwardConverter<TIn1, TIn2, TOut>(TIn1 input1, TIn2 input2);
public delegate void ZipAdvancedBackwardConverter<TIn1, TIn2, TOut>(TOut output, ref TIn1 input1, ref TIn2 input2);
public delegate void ZipPartialAdvancedBackwardConverter<TIn1, TIn2, TOut>(TOut output, ref TIn1 input1, TIn2 input2);
partial class Zip<TIn1, TIn2, TOut>
{
    public static IBinding<TOut> Create(IBinding<TIn1> inBinding1, IBinding<TIn2> inBinding2, ZipForwardConverter<TIn1, TIn2, TOut> converter, ZipAdvancedBackwardConverter<TIn1, TIn2, TOut> backwardConverter)
        => new OutputBinding<TOut>(
            new Zip<TIn1, TIn2, TOut>(inBinding1, inBinding2, converter).AssignTo(out var impl),
            () => impl._value,
            value =>
            {
                var val1 = impl.owner1;
                var val2 = impl.owner2;
                backwardConverter(value, ref val1, ref val2);
                if (!EqualityComparer<TIn1>.Default.Equals(val1, inBinding1.CurrentValue))
                    inBinding1.CurrentValue = val1;
                if (!EqualityComparer<TIn2>.Default.Equals(val2, inBinding2.CurrentValue))
                    inBinding2.CurrentValue = val2;
            }
        );
    public static IBinding<TOut> Create(IBinding<TIn1> inBinding1, IReadOnlyBinding<TIn2> inBinding2, ZipForwardConverter<TIn1, TIn2, TOut> converter, ZipPartialAdvancedBackwardConverter<TIn1, TIn2, TOut> backwardConverter)
        => new OutputBinding<TOut>(
            new Zip<TIn1, TIn2, TOut>(inBinding1, inBinding2, converter).AssignTo(out var impl),
            () => impl._value,
            value =>
            {
                var val1 = impl.owner1;
                var val2 = impl.owner2;
                backwardConverter(value, ref val1, val2);
                if (!EqualityComparer<TIn1>.Default.Equals(val1, inBinding1.CurrentValue))
                    inBinding1.CurrentValue = val1;
            }
        );
    public static IReadOnlyBinding<TOut> Create(IReadOnlyBinding<TIn1> inBinding1, IReadOnlyBinding<TIn2> inBinding2, ZipForwardConverter<TIn1, TIn2, TOut> converter)
        => new OutputReadOnlyBinding<TOut>(
            new Zip<TIn1, TIn2, TOut>(inBinding1, inBinding2, converter).AssignTo(out var impl),
            () => impl._value
        );
}
partial class Zip<TIn1, TIn2, TOut>(IReadOnlyBinding<TIn1> inBinding1, IReadOnlyBinding<TIn2> inBinding2, ZipForwardConverter<TIn1, TIn2, TOut> converter) : BindingNotifyBase<TOut>
{
    protected TIn1 owner1 = inBinding1.CurrentValue;
    protected TIn2 owner2 = inBinding2.CurrentValue;
    void SetData(TIn1 value1, TIn2 value2)
    {
        if (EqualityComparer<TIn1>.Default.Equals(owner1, value1) && EqualityComparer<TIn2>.Default.Equals(owner2, value2))
            return;
        UnregisterValueChangingEvents();
        UnregisterValueChangedEvents();
        owner1 = value1;
        owner2 = value2;
        var oldValue = _value;
        var newValue = converter(value1, value2);
        InvokeValueChanging(oldValue, newValue);
        _value = newValue;
        InvokeValueChanged(oldValue, newValue);
        RegisterValueChangingEventsIfNeeded();
        RegisterValueChangedEventsIfNeeded();
    }
    protected TOut _value = converter(inBinding1.CurrentValue, inBinding2.CurrentValue);
    
    protected override void RegisterValueChangedEvents()
    {
        inBinding1.ValueChanged += InitialOwner_ValueChanged1;
        inBinding2.ValueChanged += InitialOwner_ValueChanged2;
        SetData(inBinding1.CurrentValue, inBinding2.CurrentValue);
    }

    private void InitialOwner_ValueChanged1(TIn1 oldValue, TIn1 newValue)
    {
        SetData(newValue, owner2);
    }
    private void InitialOwner_ValueChanged2(TIn2 oldValue, TIn2 newValue)
    {
        SetData(owner1, newValue);
    }

    protected override void UnregisterValueChangedEvents()
    {
        inBinding1.ValueChanged -= InitialOwner_ValueChanged1;
        inBinding2.ValueChanged -= InitialOwner_ValueChanged2;
    }
    protected override void RegisterValueChangingEvents()
    {
    }
    protected override void UnregisterValueChangingEvents()
    {
    }
    protected override void RegisterRootChangedEvents()
    {
        inBinding1.RootChanged += InvokeRootChanged;
        inBinding2.RootChanged += InvokeRootChanged;
    }

    protected override void UnregisterRootChangedEvents()
    {
        inBinding1.RootChanged -= InvokeRootChanged;
        inBinding2.RootChanged -= InvokeRootChanged;
    }
#if DEBUG
    public override string ToString()
    {
        return $"{inBinding1} > Zip({inBinding2})";
    }
#endif
}