using Get.Data.Bindings;
using Get.Data.Collections.Update;
using Get.Data.Bindings.Linq;

namespace Get.Data.Bindings.Linq
{
    partial class Extension
    {
        public static IReadOnlyBinding<T?> ElementAt<T>(this IUpdateReadOnlyCollection<T> collection, IReadOnlyBinding<int> index)
            => new ElementAtReadOnly<T>(collection, index);
    }
    class ElementAtReadOnly<T>(IUpdateReadOnlyCollection<T> collection, IReadOnlyBinding<int> index) : ElementAtBase<T?>(collection, index), IReadOnlyBinding<T?>
    {
        public T? CurrentValue { get => value; }
    }
}
namespace Get.Data.Collections.Linq
{
    partial struct GDUpdateReadOnlyCollectionHelper<T>
    {
        public IReadOnlyBinding<T?> ElementAt(IReadOnlyBinding<int> index)
            => collection.ElementAt(index);
    }
}