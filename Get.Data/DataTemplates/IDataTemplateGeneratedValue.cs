using Get.Data.Bindings;

namespace Get.Data.DataTemplates;

public class DataTemplateGeneratedValue<TSrc, TOut> : IDataTemplateGeneratedValue<TSrc, TOut>
{
    internal DataTemplateGeneratedValue(DataTemplate<TSrc, TOut> Template, IReadOnlyBinding<TSrc> binding)
    {
        this.Template = Template;
        DataRoot = new(binding);
        GeneratedValue = Template.TemplateDefinition(DataRoot);
    }
    internal DataTemplateGeneratedValue(IDataTemplate<TSrc, TOut> Template, RootBinding<TSrc> dataRoot, TOut value)
    {
        this.Template = Template;
        DataRoot = dataRoot;
        GeneratedValue = value;
    }
    public IReadOnlyBinding<TSrc> Binding { get => DataRoot.ParentBinding; set => DataRoot.ParentBinding = value; }
    public IDataTemplate<TSrc, TOut> Template { get; }
    public TOut GeneratedValue { get; private set; }
    internal readonly RootBinding<TSrc> DataRoot;
    public void Recycle()
    {
        Template.NotifyRecycle(this);
        //Value = default!;
    }
}