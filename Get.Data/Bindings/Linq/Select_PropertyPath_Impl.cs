using Get.Data.Collections.Update;
using Get.Data.Properties;

namespace Get.Data.Bindings.Linq;
abstract class SelectPropertyPathBase<TOwner, TOut, TProp, TPropDef>(IReadOnlyBinding<TOwner> bindingOwner, TPropDef pDef) : BindingNotifyBase<TOut> where TProp : IReadOnlyProperty<TOut>
{
    TOwner owner = bindingOwner.CurrentValue;
    void SetData(TOwner newOwner)
    {
        if (EqualityComparer<TOwner>.Default.Equals(owner, newOwner))
            return;
        UnregisterValueChangingEvents();
        UnregisterValueChangedEvents();
        var oldValue = currentProperty.CurrentValue;
        var newProperty = GetProperty(pDef, newOwner);
        var newValue = newProperty.CurrentValue;
        InvokeValueChanging(oldValue, newValue);
        owner = newOwner;
        currentProperty = newProperty;
        InvokeValueChanged(oldValue, newValue);
        RegisterValueChangingEventsIfNeeded();
        RegisterValueChangedEventsIfNeeded();
    }
    protected TProp currentProperty;
    protected override void OnInitialize()
    {
        base.OnInitialize();
        currentProperty = GetProperty(pDef, bindingOwner.CurrentValue);
    }
    protected override void RegisterValueChangedEvents()
    {
        currentProperty.ValueChanged += InvokeValueChanged;
        bindingOwner.ValueChanged += InitialOwner_ValueChanged;
        SetData(bindingOwner.CurrentValue);
    }
    protected abstract TProp GetProperty(TPropDef pdef, TOwner owner);

    private void InitialOwner_ValueChanged(TOwner oldValue, TOwner newValue)
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