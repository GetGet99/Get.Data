using Get.Data.DataTemplates;
using Get.Data.Properties;

namespace Get.Data.Bundles;
[AutoProperty]
public partial class ContentBundle<TIn, TOut> : IContentBundle<TOut>
{
    public IProperty<TIn> ContentProperty { get; }
    public IProperty<IDataTemplate<TIn,TOut>?> ContentTemplateProperty { get; } = AutoTyper.Auto<IDataTemplate<TIn, TOut>?>(default);
    Property<TOut?> OutElement { get; } = new(default);

    public IReadOnlyProperty<TOut?> OutputContent => OutElement;

    IDataTemplateGeneratedValue<TIn, TOut>? _generatedValue;
    public ContentBundle(TIn defaultContent)
    {
        ContentProperty = AutoTyper.Auto(defaultContent);
        ContentTemplateProperty.ValueChanged += (old, @new) =>
        {
            if (old is not null && _generatedValue is not null)
                old.NotifyRecycle(_generatedValue);
            if (@new is not null && OutElement is not null)
                OutElement.Value =
                    (_generatedValue = @new.Generate(ContentProperty))
                    .GeneratedValue;
        };
    }
}
