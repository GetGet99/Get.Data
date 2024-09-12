using Get.Data.Collections.Update;
using Get.Data.Helpers;

namespace Get.Data.Bindings.Linq;
partial class Select<TIn, TOut>
{
    public static IBinding<TOut> Create(IBinding<TIn> src, ForwardConverter<TIn, TOut> forwardConverter, BackwardConverter<TIn, TOut> backwardConverter)
        => new OutputBinding<TOut>(
            new Select<TIn, TOut>(src, forwardConverter).AssignTo(out var impl),
            () => impl._value,
            x => src.CurrentValue = backwardConverter(x)
        );
    public static IBinding<TOut> Create(
        IBinding<TIn> src,
        ForwardConverter<TIn, TOut> forwardConverter,
        AdvancedBackwardConverter<TIn, TOut> backwardConverter)
        => new OutputBinding<TOut>(
            new Select<TIn, TOut>(src, forwardConverter).AssignTo(out var impl),
            () => impl._value,
            x => src.CurrentValue = backwardConverter(x, impl.owner)
        );
    public static IReadOnlyBinding<TOut> Create(IReadOnlyBinding<TIn> src, ForwardConverter<TIn, TOut> forwardConverter)
        => new OutputReadOnlyBinding<TOut>(
            new Select<TIn, TOut>(src, forwardConverter).AssignTo(out var impl),
            () => impl._value
        );
}
sealed partial class Select<TIn, TOut>(IReadOnlyBinding<TIn> inBinding, ForwardConverter<TIn, TOut> converter)
    : BindingNotifyBase<TOut>
{
    TIn owner = inBinding.CurrentValue;
    void SetData(TIn value)
    {
        if (EqualityComparer<TIn>.Default.Equals(owner, value))
            return;
        UnregisterValueChangingEvents();
        UnregisterValueChangedEvents();
        owner = value;
        var oldValue = _value;
        var newValue = converter(value);
        InvokeValueChanging(oldValue, newValue);
        _value = newValue;
        InvokeValueChanged(oldValue, newValue);
        RegisterValueChangingEventsIfNeeded();
        RegisterValueChangedEventsIfNeeded();
    }
    TOut _value = converter(inBinding.CurrentValue);
    protected override void RegisterValueChangedEvents()
    {
        inBinding.ValueChanged += InitialOwner_ValueChanged;
        SetData(inBinding.CurrentValue);
    }

    private void InitialOwner_ValueChanged(TIn oldValue, TIn newValue)
    {
        SetData(newValue);
    }

    protected override void UnregisterValueChangedEvents()
    {
        inBinding.ValueChanged -= InitialOwner_ValueChanged;
    }
    protected override void RegisterValueChangingEvents()
    {
    }
    protected override void UnregisterValueChangingEvents()
    {
    }
    protected override void RegisterRootChangedEvents()
    {
        inBinding.RootChanged += InvokeRootChanged;
    }

    protected override void UnregisterRootChangedEvents()
    {
        inBinding.RootChanged -= InvokeRootChanged;
    }
#if DEBUG
    public override string ToString()
    {
        return $"{inBinding} > Select";
    }
#endif
}