using Get.Data.Collections.Implementation;

namespace Get.Data.Collections;
public interface IGDCollection<T> : IGDFixedSizeCollection<T>, IRemoveAtImplGDCollection
{
    void Insert(int index, T item);
}