using Get.Data.Collections;
using Get.Data.Collections.Update;
using Get.Data.DataTemplates;
using Get.Data.Properties;

namespace Get.Data.Bundles;

public abstract class ItemsBundleBase<TIn, TOut> : IItemsBundle<TOut>
{
    protected abstract IUpdateItemsBundleOutputCollection<TIn> ItemsSourceBase { get; }
    public Property<IDataTemplate<TIn, TOut>?> ItemTemplateProperty { get; } = new(default);
    public IDataTemplate<TIn, TOut>? ItemTemplate
    {
        get => ItemTemplateProperty.Value;
        set => ItemTemplateProperty.Value = value;
    }
    IUpdateReadOnlyCollection<TOut> IReadOnlyItemsBundle<TOut>.OutputContent => dest;
    public IUpdateItemsBundleOutputCollection<TOut> OutputContent => new Wrapper<TIn, TOut>(ItemsSourceBase, dest);
    readonly UpdateCollection<TOut> dest = new();
    IDataTemplateGeneratedValue<TIn, TOut>? _generatedValue;
    public ItemsBundleBase()
    {
        ItemTemplateProperty.ValueChanged += (old, @new) => RefreshTemplate(@new);
    }
    IDisposable? _collectionBinder;
    void RefreshTemplate(IDataTemplate<TIn, TOut>? @new)
    {
        _collectionBinder?.Dispose();
        dest.Clear();
        if (@new is not null)
            _collectionBinder = ItemsSourceBase.Bind(dest, @new);
    }
}
public class ItemsBundle<TIn, TOut> : ItemsBundleBase<TIn, TOut>
{
    public TwoWayUpdateCollectionProperty<TIn> ItemsSourceProperty { get; } = [];
    public IUpdateCollection<TIn> ItemsSource
    {
        get => ItemsSourceProperty.Value;
        set => ItemsSourceProperty.Value = value;
    }
    protected override IUpdateItemsBundleOutputCollection<TIn> ItemsSourceBase => new Wrapper<TIn>(ItemsSourceProperty);
}
readonly struct Wrapper<TIn, TOut>(IUpdateItemsBundleOutputCollection<TIn> ItemsSourceBase, IUpdateReadOnlyCollection<TOut> dest) : IUpdateItemsBundleOutputCollection<TOut>
{
    public TOut this[int index] { get => dest[index]; }

    public int Count => dest.Count;

    public event UpdateCollectionItemsChanged<TOut> ItemsChanged
    {
        add => dest.ItemsChanged += value;
        remove => dest.ItemsChanged -= value;
    }

    public void Move(int index1, int index2)
        => ItemsSourceBase.Move(index1, index2);

    public bool Remove(TOut item)
    {
        var idx = dest.IndexOf(item);
        var isValid = idx >= 0;
        if (isValid)
            ItemsSourceBase.RemoveAt(idx);
        return isValid;
    }

    public void RemoveAt(int index)
        => ItemsSourceBase.RemoveAt(index);
}
readonly struct Wrapper<TIn>(IUpdateCollection<TIn> c) : IUpdateItemsBundleOutputCollection<TIn>
{
    public TIn this[int index] => c[index];

    public int Count => c.Count;

    public event UpdateCollectionItemsChanged<TIn> ItemsChanged
    {
        add => c.ItemsChanged += value;
        remove => c.ItemsChanged -= value;
    }

    public void Move(int index1, int index2)
        => c.Move(index1, index2);

    public bool Remove(TIn item)
        => c.Remove(item);

    public void RemoveAt(int index)
        => c.RemoveAt(index);
}