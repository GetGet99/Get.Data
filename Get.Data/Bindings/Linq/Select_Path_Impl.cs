namespace Get.Data.Bindings.Linq;
abstract class SelectPathBase<TOwner, TOut, TBinding>(IReadOnlyBinding<TOwner> bindingOwner, Func<TOwner, TBinding> pDef) : BindingNotifyBase<TOut>
    where TBinding : INotifyBinding<TOut>, IReadOnlyBinding<TOut>
{
    TOwner owner = bindingOwner.CurrentValue;
    void SetData(TOwner value)
    {
        if (EqualityComparer<TOwner>.Default.Equals(owner, value))
            return;
        UnregisterValueChangingEvents();
        UnregisterValueChangedEvents();
        owner = value;
        var oldVal = default(TOut);
        if (currentBinding is not null) oldVal = currentBinding.CurrentValue;
        TBinding? newBinding;
        if (value is null)
            newBinding = default;
        else
            newBinding = pDef(value);

        var newVal = default(TOut);
        if (newBinding is not null) newVal = newBinding.CurrentValue;
        InvokeValueChanging(oldVal!, newVal!);
        currentBinding = newBinding;
        InvokeValueChanged(oldVal!, newVal!);
        RegisterValueChangingEventsIfNeeded();
        RegisterValueChangedEventsIfNeeded();
    }
    protected TBinding? currentBinding = bindingOwner.CurrentValue is null ? default : pDef(bindingOwner.CurrentValue);
    protected override void RegisterValueChangedEvents()
    {
        if (currentBinding is not null)
            currentBinding.ValueChanged += InvokeValueChanged;
        bindingOwner.ValueChanged += InitialOwner_ValueChanged;
        SetData(bindingOwner.CurrentValue);
    }

    private void InitialOwner_ValueChanged(TOwner oldValue, TOwner newValue)
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