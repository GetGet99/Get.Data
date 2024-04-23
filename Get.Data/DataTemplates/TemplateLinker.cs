using Get.Data.Collections;
using Get.Data.Collections.Update;
using Get.Data.ModelLinker;
namespace Get.Data.DataTemplates;

class TemplateLinker<TSource, TDest>(IUpdateReadOnlyCollection<TSource> source, UpdateCollection<DataTemplateGeneratedValue<TSource, TDest>> dest, DataTemplate<TSource, TDest> dataTemplate) : UpdateCollectionModelLinker<TSource, DataTemplateGeneratedValue<TSource, TDest>>(source, dest)
{
    protected override DataTemplateGeneratedValue<TSource, TDest> CreateFrom(TSource source)
    {
        return dataTemplate.Generate(source);
    }
    protected override void Recycle(DataTemplateGeneratedValue<TSource, TDest> dest)
    {
        base.Recycle(dest);
        dest.Recycle();
    }
}