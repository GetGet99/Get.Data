using Get.Data.Bindings;
using System.Runtime.CompilerServices;
namespace Get.Data.DataTemplates;

public class DataTemplate<TSrc, TOut>(DataTemplateDefinition<TSrc, TOut> TemplateDefinition) : IDataTemplate<TSrc, TOut>
{
    readonly internal DataTemplateDefinition<TSrc, TOut> TemplateDefinition = TemplateDefinition;
    readonly Queue<DataTemplateGeneratedValue<TSrc, TOut>> recycledQueue = new();
    public IDataTemplateGeneratedValue<TSrc, TOut> Generate(IReadOnlyBinding<TSrc> source)
    {
        if (recycledQueue.Count > 0)
        {
            var item = recycledQueue.Dequeue();
            item.DataRoot.ParentBinding = source;
            return item;
        }
        return new DataTemplateGeneratedValue<TSrc, TOut>(this, source);
    }
    public IDataTemplateGeneratedValue<TSrc, TOut> Generate(TSrc source)
        => Generate(new ValueBinding<TSrc>(source));
    public void NotifyRecycle(IDataTemplateGeneratedValue<TSrc, TOut> recycledItem)
    {
        recycledQueue.Enqueue((DataTemplateGeneratedValue<TSrc, TOut>)recycledItem);
    }
}
public static class DataTemplate
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DataTemplate<TSrc, TOut> Auto<TSrc, TOut>(DataTemplateDefinition<TSrc, TOut> TemplateDefinition)
        => new(TemplateDefinition);
}
