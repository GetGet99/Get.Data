using Get.Data.Collections;
using Get.Data.Collections.Linq;
using Get.Data.Collections.Update;
using Get.Data.ModelLinker;
namespace Get.Data.DataTemplates;
public static class CollectionBindingExtension
{
    public static IDisposable Bind<TSrc, TOut>(this IUpdateReadOnlyCollection<TSrc> collection, IGDCollection<TOut> @out, IDataTemplate<TSrc, TOut> dataTemplate
#if DEBUG
        , bool debug = false
#endif
        )
    {
        UpdateCollection<IDataTemplateGeneratedValue<TSrc, TOut>> middleCollection = new();
        var a = new TemplateLinker<TSrc, TOut>(collection, middleCollection, dataTemplate)
#if DEBUG
        { DebugMode = debug }
        #endif
        ;
        var b = middleCollection.Select(x => x.GeneratedValue).Bind(@out);
        a.ResetAndReadd();
        return new Disposable(() =>
        {
            a.Dispose();
            b.Dispose();
            @out.Clear();
        });
    }
    public static IDisposable Bind<T>(this IUpdateReadOnlyCollection<T> collection, IGDCollection<T> @out
        #if DEBUG
        , bool debug = false
#endif
        )
    {
        var linker = new UpdateCollectionModelLinker<T>(collection, @out)
#if DEBUG
        { DebugMode = debug }
#endif
        ;
        linker.ResetAndReadd();
        return new Disposable(() =>
        {
            linker.Dispose();
            @out.Clear();
        });
    }

    public static IDisposable Bind<TSrc, TOut>(this IUpdateReadOnlyCollection<TSrc> collection, IGDCollection<TOut> @out, Func<TSrc, TOut> createFrom
#if DEBUG
        , bool debug = false
#endif
        )
    {
        var a = new UpdateCollectionModelLinkerDelegate<TSrc, TOut>(collection, @out, createFrom)
#if DEBUG
        { DebugMode = debug }
#endif
        ;
        a.ResetAndReadd();
        return new Disposable(() =>
        {
            a.Dispose();
            @out.Clear();
        });
    }
}