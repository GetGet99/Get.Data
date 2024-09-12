using Get.Data.Bindings;
using Get.Data.Bindings.Linq;
using Get.Data.Collections.Update;

namespace Get.Data.Bindings.Linq
{
    partial class Extension
    {
        public static IBinding<T?> ElementAt<T>(this IUpdateFixedSizeCollection<T> collection, IReadOnlyBinding<int> index)
            => Linq.ElementAt<T>.Create(collection, index);
        public static IReadOnlyBinding<T?> ElementAt<T>(this IUpdateReadOnlyCollection<T> collection, IReadOnlyBinding<int> index)
            => Linq.ElementAt<T>.Create(collection, index);
    }
}
namespace Get.Data.Collections.Linq
{
    partial struct GDUpdateFixedSizeCollectionHelper<T>
    {
        public IBinding<T?> ElementAt(IReadOnlyBinding<int> index)
            => collection.ElementAt(index);
    }
    partial struct GDUpdateReadOnlyCollectionHelper<T>
    {
        public IReadOnlyBinding<T?> ElementAt(IReadOnlyBinding<int> index)
            => collection.ElementAt(index);
    }
}