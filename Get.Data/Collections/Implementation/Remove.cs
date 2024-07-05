#nullable enable
using Get.Data.Collections.Implementation;

namespace Get.Data.Collections
{
    partial class DefaultImplementations
    {
        public static bool Remove<T>(this IGDCollection<T> collection, T item)
        {
            if (collection is IRemoveImplGDCollection<T> remImpl) return remImpl.Remove(item);
            var idx = collection.IndexOf(item);
            if (idx >= 0)
                collection.RemoveAt(idx);
            return idx >= 0;
        }
    }
}

namespace Get.Data.Collections.Implementation
{
    public interface IRemoveImplGDCollection<T>
    {
        bool Remove(T item);
    }
}