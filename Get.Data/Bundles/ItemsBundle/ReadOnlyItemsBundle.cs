using Get.Data.Bindings;
using Get.Data.Bindings.Linq;
using Get.Data.Collections;
using Get.Data.Collections.Update;
using Get.Data.DataTemplates;
using Get.Data.Properties;

namespace Get.Data.Bundles;

public abstract class ReadOnlyItemsBundleBase<TIn, TOut> : IReadOnlyItemsBundle<TOut>
{
    protected abstract IUpdateReadOnlyCollection<TIn> ItemsSourceBase { get; }
    public Property<IDataTemplate<TIn,TOut>?> ItemTemplateProperty { get; } = new(default);
    public IDataTemplate<TIn, TOut>? ItemTemplate
    {
        get => ItemTemplateProperty.Value;
        set => ItemTemplateProperty.Value = value;
    }
    public IUpdateReadOnlyCollection<TOut> OutputContent => dest;
    readonly UpdateCollection<TOut> dest = new();
    IDataTemplateGeneratedValue<TIn, TOut>? _generatedValue;
    public ReadOnlyItemsBundleBase()
    {
        ItemTemplateProperty.ValueChanged += (old, @new) => RefreshTemplate(@new);
    }
    IDisposable? _collectionBinder;
    void RefreshTemplate(IDataTemplate<TIn, TOut>? @new)
    {
        _collectionBinder?.Dispose();
        dest.Clear();
        if (ItemsSourceBase is not null && @new is not null)
            _collectionBinder = ItemsSourceBase.Bind(dest, @new);
    }
}
public class ReadOnlyItemsBundle<TIn, TOut> : ReadOnlyItemsBundleBase<TIn, TOut>
{
    public OneWayUpdateCollectionProperty<TIn> ItemsSourceProperty { get; } = new();
    protected override IUpdateReadOnlyCollection<TIn> ItemsSourceBase => ItemsSourceProperty;
    public IUpdateReadOnlyCollection<TIn> ItemsSource
    {
        get => ItemsSourceProperty.Value;
        set => ItemsSourceProperty.Value = value;
    }
}
public class ReadOnlyItemsBundle<TIn1, TIn2, TOut>
{
    public Property<IDataTemplate<TIn2, TOut>?> ItemTemplateProperty { get; } = new(default);
    public IDataTemplate<TIn2, TOut>? ItemTemplate
    {
        get => ItemTemplateProperty.Value;
        set => ItemTemplateProperty.Value = value;
    }

    public Property<IUpdateReadOnlyCollection<TIn1>?> ItemsSourceProperty { get; } = new(default);
    public IUpdateReadOnlyCollection<TIn1>? ItemsSource
    {
        get => ItemsSourceProperty.Value;
        set => ItemsSourceProperty.Value = value;
    }
}
public class ReadOnlyItemsBundleProcessor<TIn1, TIn2, TOut>
{
    ReadOnlyItemsBundle<TIn1, TIn2> ItemsBundle2 { get; }
    public ReadOnlyItemsBundleProcessor(IReadOnlyBinding<ReadOnlyItemsBundle<TIn1, TIn2, TOut>> prop, DataTemplateDefinition<TIn1, TIn2> innerTemplate)
    {
        ItemsBundle2 = new()
        {
            ItemTemplate = new DataTemplate<TIn1, TIn2>(innerTemplate),
            ItemsSource = SourceItemsProperty
        };
        FinalTemplate = new()
        {
            ItemsSource = ItemsBundle2.OutputContent
        };
        FinalTemplate.ItemTemplateProperty.Bind(ItemTemplateProperty, ReadOnlyBindingModes.OneWay);
        SourceItemsProperty.Bind(prop.SelectPath(x => x.ItemsSourceProperty), ReadOnlyBindingModes.OneWay);
    }
    Property<IDataTemplate<TIn2, TOut>?> ItemTemplateProperty { get; } = new(default);
    IDataTemplate<TIn2, TOut>? ItemTemplate
    {
        get => ItemTemplateProperty.Value;
        set => ItemTemplateProperty.Value = value;
    }

    public OneWayUpdateCollectionProperty<TIn1> SourceItemsProperty { get; } = new();
    public OneWayUpdateCollectionProperty<TIn2> ConvertedItemsProperty => FinalTemplate.ItemsSourceProperty;
    internal ReadOnlyItemsBundle<TIn2, TOut> FinalTemplate { get; }
}