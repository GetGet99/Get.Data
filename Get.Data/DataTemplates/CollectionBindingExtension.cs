using Get.Data.Collections;
using Get.Data.Collections.Linq;
using Get.Data.Collections.Update;
using Get.Data.ModelLinker;
namespace Get.Data.DataTemplates;
public static class CollectionBindingExtension
{
    public static IDisposable Bind<TSrc, TOut>(this IUpdateReadOnlyCollection<TSrc> collection, IGDCollection<TOut> @out, DataTemplate<TSrc, TOut> dataTemplate)
    {
        UpdateCollection<DataTemplateGeneratedValue<TSrc, TOut>> middleCollection = new();
        var a = new TemplateLinker<TSrc, TOut>(collection, middleCollection, dataTemplate);
        var b = middleCollection.Select(x => x.GeneratedValue).Bind(@out);
        a.ResetAndReadd();
        return new Disposable(() =>
        {
            a.Dispose();
            b.Dispose();
            @out.Clear();
        });
    }
    public static IDisposable Bind<T>(this IUpdateReadOnlyCollection<T> collection, IGDCollection<T> @out)
    {
        var linker = new UpdateCollectionModelLinker<T>(collection, @out);
        linker.ResetAndReadd();
        return new Disposable(() =>
        {
            linker.Dispose();
            @out.Clear();
        });
    }
}