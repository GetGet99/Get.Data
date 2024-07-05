using Get.Data.Bindings;
using Get.Data.Collections;
using Get.Data.Collections.Update;
using Get.Data.ModelLinker;
namespace Get.Data.DataTemplates;

class TemplateLinker<TSource, TDest>(IUpdateReadOnlyCollection<TSource> source, UpdateCollection<IDataTemplateGeneratedValue<TSource, TDest>> dest, IDataTemplate<TSource, TDest> dataTemplate) : UpdateCollectionModelLinker<TSource, IDataTemplateGeneratedValue<TSource, TDest>>(source, dest)
{
    protected override IDataTemplateGeneratedValue<TSource, TDest> CreateFrom(TSource source)
    {
        return dataTemplate.Generate(new ValueBinding<TSource>(source));
    }
    protected override void Recycle(IDataTemplateGeneratedValue<TSource, TDest> dest)
    {
        base.Recycle(dest);
        dest.Recycle();
    }
}