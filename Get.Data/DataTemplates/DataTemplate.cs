using Get.Data.Bindings;
namespace Get.Data.DataTemplates;

public class DataTemplate<TSrc, TOut>(DataTemplateDefinition<TSrc, TOut> TemplateDefinition)
{
    readonly internal DataTemplateDefinition<TSrc, TOut> TemplateDefinition = TemplateDefinition;
    readonly Queue<DataTemplateGeneratedValue<TSrc, TOut>> recycledQueue = new();
    public DataTemplateGeneratedValue<TSrc, TOut> Generate(IReadOnlyBinding<TSrc> source)
    {
        if (recycledQueue.Count > 0)
        {
            var item = recycledQueue.Dequeue();
            item.DataRoot.ParentBinding = source;
            return item;
        }
        return new(this, source);
    }
    public DataTemplateGeneratedValue<TSrc, TOut> Generate(TSrc source)
        => Generate(new ValueBinding<TSrc>(source));
    public void NotifyRecycle(DataTemplateGeneratedValue<TSrc, TOut> recycledItem)
    {
        recycledQueue.Enqueue(recycledItem);
    }
}
