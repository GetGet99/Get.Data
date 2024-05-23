using Get.Data.Collections.Linq;
using Get.Data.Collections.Update;
namespace Get.Data.Collections;
public static partial class Extension
{
    public static IUpdateFixedSizeCollection<T> ElementsAt<T>(this IUpdateFixedSizeCollection<T> c, IUpdateReadOnlyCollection<int> indices)
        => new ElementsAtUpdateFixedSize<T>(c, indices);
}
public class ElementsAtUpdateFixedSize<T>(IUpdateFixedSizeCollection<T> c, IUpdateReadOnlyCollection<int> indices) : ElementsAtUpdateBase<T>(c, indices), IUpdateFixedSizeCollection<T>
{
    public T this[int index] { get => c[indices[index]]; set => c[indices[index]] = value; }

    public int Count => indices.Count;
}