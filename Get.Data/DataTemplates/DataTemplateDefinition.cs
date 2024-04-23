using Get.Data.Bindings;
namespace Get.Data.DataTemplates;

public delegate TOut DataTemplateDefinition<TSrc, TOut>(RootBinding<TSrc> dataRoot);