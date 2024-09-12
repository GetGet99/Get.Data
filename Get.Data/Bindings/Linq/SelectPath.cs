using Get.Data.Helpers;

namespace Get.Data.Bindings.Linq;
public delegate IReadOnlyBinding<TDest> SelectPathDelegate<TSrc, TDest>(TSrc item);
partial class SelectPath<TSrc, TDest>
{
    public static IBinding<TDest> Create(IReadOnlyBinding<TSrc> src, Func<TSrc, IBinding<TDest>> selector)
        => new OutputBinding<TDest>(
            new SelectPath<TSrc, TDest>(src, x => selector(x)).AssignTo(out var impl),
            () => impl.currentBinding is null ? default! : impl.currentBinding.CurrentValue,
            value => (impl.currentBinding as IBinding<TDest> ?? throw new NullReferenceException()).CurrentValue = value
        );
    public static IReadOnlyBinding<TDest> Create(IReadOnlyBinding<TSrc> src, SelectPathDelegate<TSrc, TDest> selector)
        => new OutputReadOnlyBinding<TDest>(
            new SelectPath<TSrc, TDest>(src, x => selector(x)).AssignTo(out var impl),
            () => impl.currentBinding is null ? default! : impl.currentBinding.CurrentValue
        );
}
partial class SelectPath<TSrc, TDest>(
    IReadOnlyBinding<TSrc> bindingOwner,
    Func<TSrc, IReadOnlyBinding<TDest>> pDef
) : BindingNotifyBase<TDest>
{
    TSrc owner = bindingOwner.CurrentValue;
    void SetData(TSrc value)
    {
        if (EqualityComparer<TSrc>.Default.Equals(owner, value))
            return;
        UnregisterValueChangingEvents();
        UnregisterValueChangedEvents();
        owner = value;
        var oldVal = default(TDest);
        if (currentBinding is not null) oldVal = currentBinding.CurrentValue;
        IReadOnlyBinding<TDest>? newBinding;
        if (value is null)
            newBinding = default;
        else
            newBinding = pDef(value);

        var newVal = default(TDest);
        if (newBinding is not null) newVal = newBinding.CurrentValue;
        InvokeValueChanging(oldVal!, newVal!);
        currentBinding = newBinding;
        InvokeValueChanged(oldVal!, newVal!);
        RegisterValueChangingEventsIfNeeded();
        RegisterValueChangedEventsIfNeeded();
    }
    IReadOnlyBinding<TDest?>? currentBinding = bindingOwner.CurrentValue is null ? default : pDef(bindingOwner.CurrentValue);
    protected override void RegisterValueChangedEvents()
    {
        if (currentBinding is not null)
            currentBinding.ValueChanged += InvokeValueChanged;
        bindingOwner.ValueChanged += InitialOwner_ValueChanged;
        SetData(bindingOwner.CurrentValue);
    }

    private void InitialOwner_ValueChanged(TSrc oldValue, TSrc newValue)
    {
        SetData(newValue);
    }

    protected override void UnregisterValueChangedEvents()
    {
        if (currentBinding is not null)
            currentBinding.ValueChanged -= InvokeValueChanged;
        bindingOwner.ValueChanged -= InitialOwner_ValueChanged;
    }
    protected override void RegisterValueChangingEvents()
    {
        if (currentBinding is not null)
            currentBinding.ValueChanging += InvokeValueChanging;
    }
    protected override void UnregisterValueChangingEvents()
    {
        if (currentBinding is not null)
            currentBinding.ValueChanging -= InvokeValueChanging;
    }
    protected override void RegisterRootChangedEvents()
    {
        bindingOwner.RootChanged += InvokeRootChanged;
    }

    protected override void UnregisterRootChangedEvents()
    {
        bindingOwner.RootChanged -= InvokeRootChanged;
    }
}