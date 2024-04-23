namespace Get.Data.DataTemplates;

public class DataTemplateWithIndex<TSrc, TOut>(DataTemplateDefinition<IndexItem<TSrc>, TOut> TemplateDefinition) :
    DataTemplate<IndexItem<TSrc>, TOut>(TemplateDefinition)
{

}
