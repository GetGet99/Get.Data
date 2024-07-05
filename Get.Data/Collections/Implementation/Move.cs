#nullable enable
using Get.Data.Collections.Implementation;

namespace Get.Data.Collections
{
    partial class DefaultImplementations
    {
        public static void Move<T>(this IGDCollection<T> collection, int index1, int index2)
        {
            if (collection is IMoveImplGDCollection impl)
                impl.Move(index1, index2);
            else
                (collection[index1], collection[index2]) = (collection[index2], collection[index1]);
        }
    }
}
namespace Get.Data.Collections.Implementation
{
    public interface IMoveImplGDCollection
    {
        void Move(int index1, int index2);
    }
}