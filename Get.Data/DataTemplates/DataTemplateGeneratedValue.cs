using Get.Data.Bindings;

namespace Get.Data.DataTemplates;

public interface IDataTemplateGeneratedValue<TSrc, TOut>
{
    IReadOnlyBinding<TSrc> Binding { get; set; }
    IDataTemplate<TSrc, TOut> Template { get; }
    TOut GeneratedValue { get; }
    void Recycle();
}