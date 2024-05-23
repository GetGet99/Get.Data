using Get.Data.Collections.Linq;
using Get.Data.Collections.Update;
namespace Get.Data.Collections;
public static partial class Extension
{
    public static IUpdateReadOnlyCollection<T> ElementsAt<T>(this IUpdateReadOnlyCollection<T> c, IUpdateReadOnlyCollection<int> indices)
        => new ElementsAtUpdateReadOnly<T>(c, indices);
}
public class ElementsAtUpdateReadOnly<T>(IUpdateReadOnlyCollection<T> c, IUpdateReadOnlyCollection<int> indices) : ElementsAtUpdateBase<T>(c, indices), IUpdateReadOnlyCollection<T>
{
    public T this[int index] { get => c[indices[index]]; }

    public int Count => indices.Count;
}