#nullable enable
using Get.Data.Collections.Implementation;

namespace Get.Data.Collections.Implementation
{
    public interface IClearImplGDCollection
    {
        void Clear();
    }
}
namespace Get.Data.Collections
{
    partial class DefaultImplementations
    {
        public static void Clear<T>(this IGDCollection<T> collection)
        {
            if (collection is IClearImplGDCollection impl)
                impl.Clear();
            else
                Implementations.Clear_RemoveLast(collection);
        }
    }
}