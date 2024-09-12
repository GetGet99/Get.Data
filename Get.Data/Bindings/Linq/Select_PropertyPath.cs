using Get.Data.Collections.Update;
using Get.Data.Helpers;
using Get.Data.Properties;

namespace Get.Data.Bindings.Linq;

partial class SelectPropertyPath<TSrc, TDest>
{
    public static IReadOnlyBinding<TDest> Create(IReadOnlyBinding<TSrc> src, IReadOnlyPropertyDefinition<TSrc, TDest> pDef)
        => new OutputReadOnlyBinding<TDest>(
            new SelectPropertyPath<TSrc, TDest>(src, owner => pDef.GetProperty(owner)).AssignTo(out var impl),
            () => impl.currentProperty.CurrentValue
        );
    public static IBinding<TDest> Create(IReadOnlyBinding<TSrc> src, IPropertyDefinition<TSrc, TDest> pDef)
        => new OutputBinding<TDest>(
            new SelectPropertyPath<TSrc, TDest>(src, owner => pDef.GetProperty(owner)).AssignTo(out var impl),
            () => impl.currentProperty.CurrentValue,
            value => ((IProperty<TDest>)impl.currentProperty).CurrentValue = value
        );
}
partial class SelectPropertyPath<TSrc, TDest>(
    IReadOnlyBinding<TSrc> bindingOwner,
    Func<TSrc, IReadOnlyProperty<TDest>> GetProperty
) : BindingNotifyBase<TDest>
{
    TSrc owner = bindingOwner.CurrentValue;
    void SetData(TSrc newOwner)
    {
        if (EqualityComparer<TSrc>.Default.Equals(owner, newOwner))
            return;
        UnregisterValueChangingEvents();
        UnregisterValueChangedEvents();
        var oldValue = currentProperty.CurrentValue;
        var newProperty = GetProperty(newOwner);
        var newValue = newProperty.CurrentValue;
        InvokeValueChanging(oldValue, newValue);
        owner = newOwner;
        currentProperty = newProperty;
        InvokeValueChanged(oldValue, newValue);
        RegisterValueChangingEventsIfNeeded();
        RegisterValueChangedEventsIfNeeded();
    }
    // Initialize on OnInitialize()
    protected IReadOnlyProperty<TDest> currentProperty = null!;
    protected override void OnInitialize()
    {
        base.OnInitialize();
        currentProperty = GetProperty(bindingOwner.CurrentValue);
    }
    protected override void RegisterValueChangedEvents()
    {
        currentProperty.ValueChanged += InvokeValueChanged;
        bindingOwner.ValueChanged += InitialOwner_ValueChanged;
        SetData(bindingOwner.CurrentValue);
    }

    private void InitialOwner_ValueChanged(TSrc oldValue, TSrc newValue)
    {
        SetData(newValue);
    }

    protected override void UnregisterValueChangedEvents()
    {
        currentProperty.ValueChanged -= InvokeValueChanged;
        bindingOwner.ValueChanged -= InitialOwner_ValueChanged;
    }
    protected override void RegisterValueChangingEvents()
    {
        currentProperty.ValueChanging += InvokeValueChanging;
    }
    protected override void UnregisterValueChangingEvents()
    {
        currentProperty.ValueChanging -= InvokeValueChanging;
    }
    protected override void RegisterRootChangedEvents()
    {
        bindingOwner.RootChanged += InvokeRootChanged;
    }

    protected override void UnregisterRootChangedEvents()
    {
        bindingOwner.RootChanged -= InvokeRootChanged;
    }
#if DEBUG
    public override string ToString()
    {
        return $"{bindingOwner} > SelectPropertyPath";
    }
#endif
}