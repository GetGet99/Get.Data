using Get.Data.Bindings;
namespace Get.Data.DataTemplates;

public interface IDataTemplate<TSrc, TOut>
{
    IDataTemplateGeneratedValue<TSrc, TOut> Generate(IReadOnlyBinding<TSrc> source);
    void NotifyRecycle(IDataTemplateGeneratedValue<TSrc, TOut> recycledItem);
}
