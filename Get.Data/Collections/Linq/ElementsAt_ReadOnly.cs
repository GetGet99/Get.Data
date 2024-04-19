namespace Get.Data.Collections.Linq;

public static partial class Extension
{
    public static IGDReadOnlyCollection<T> ElementsAt<T>(this IGDReadOnlyCollection<T> c, IGDReadOnlyCollection<int> indices)
        => new ElementsAtReadOnly<T>(c, indices);
}
readonly struct ElementsAtReadOnly<T>(IGDReadOnlyCollection<T> c, IGDReadOnlyCollection<int> indices) : IGDReadOnlyCollection<T>
{
    public T this[int index]
    {
        get => c[indices[index]];
    }
    public int Count => indices.Count;
}