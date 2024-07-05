using Get.Data.DataTemplates;
using Get.Data.Properties;

namespace Get.Data.Bundles;

public class ContentBundle<TIn, TOut> : IContentBundle<TOut>
{
    public Property<TIn> ContentProperty { get; }
    public Property<IDataTemplate<TIn,TOut>?> ContentTemplateProperty { get; } = new(default);
    Property<TOut?> OutElement { get; } = new(default);

    public IReadOnlyProperty<TOut?> OutputContent => OutElement;

    IDataTemplateGeneratedValue<TIn, TOut>? _generatedValue;
    public ContentBundle(TIn defaultContent)
    {
        ContentProperty = new(defaultContent);
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