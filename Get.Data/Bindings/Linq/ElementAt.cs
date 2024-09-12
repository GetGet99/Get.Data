using Get.Data.Collections.Update;
using Get.Data.Helpers;

namespace Get.Data.Bindings.Linq;
partial class ElementAt<T>
{
    public static IReadOnlyBinding<T?> Create(IUpdateReadOnlyCollection<T> collection, IReadOnlyBinding<int> index)
        => new OutputReadOnlyBinding<T?>(
            new ElementAt<T>(collection, index).AssignTo(out var impl),
            () => impl.value
        );
    public static IBinding<T?> Create(IUpdateFixedSizeCollection<T> collection, IReadOnlyBinding<int> index)
        => new OutputBinding<T?>(
            new ElementAt<T>(collection, index).AssignTo(out var impl),
            () => impl.value,
            value =>
            {
                var curIdx = index.CurrentValue;
                if (curIdx < 0 || curIdx >= collection.Count)
                    throw new InvalidOperationException("Invalid Index");
                collection[index.CurrentValue] = value;
            }
        );
}
partial class ElementAt<T>(IUpdateReadOnlyCollection<T> collection, IReadOnlyBinding<int> index) : BindingNotifyBase<T?>
{
    protected T? value = index.CurrentValue >= 0 ? collection[index.CurrentValue] : default;
    //public override TOut? CurrentValue { get => value; set => throw new InvalidOperationException("Cannot go backwards"); }
    protected override void RegisterValueChangedEvents()
    {
        collection.ItemsChanged += Collection_ItemsChanged;
        index.ValueChanged += InvokeIndexChanged;
    }

    private void Collection_ItemsChanged(IEnumerable<IUpdateAction<T>> actions)
    {
        UpdateValue();
    }
    void UpdateValue()
    {
        var newValue = Get(index.CurrentValue);
        if (!EqualityComparer<T>.Default.Equals(newValue, value))
        {
            var oldValue = value;
            InvokeValueChanging(oldValue, newValue);
            value = newValue;
            InvokeValueChanged(oldValue, newValue);
        }
    }

    protected override void UnregisterValueChangedEvents()
    {
        index.ValueChanged -= InvokeIndexChanged;
    }
    protected override void RegisterValueChangingEvents()
    {
        index.ValueChanging += InvokeIndexChanging;
    }
    protected override void UnregisterValueChangingEvents()
    {
        index.ValueChanging -= InvokeIndexChanging;
    }
    T? Get(int index) => index >= collection.Count || index < 0 ? default : collection[index];
    protected void InvokeIndexChanging(int oldIndex, int newIndex) => InvokeValueChanging(value, Get(newIndex));
    protected void InvokeIndexChanged(int oldIndex, int newIndex)
    {
        var oldValue = value;
        value = Get(newIndex);
        InvokeValueChanged(oldValue, value);
    }

    protected override void RegisterRootChangedEvents() { }

    protected override void UnregisterRootChangedEvents() { }
#if DEBUG
    public override string ToString()
    {
        return $"{collection} > [Binding] ElementAt";
    }
#endif

}