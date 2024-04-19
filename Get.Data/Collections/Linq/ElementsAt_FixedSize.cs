namespace Get.Data.Collections.Linq;

public static partial class Extension
{
    public static IGDFixedSizeCollection<T> ElementsAt<T>(this IGDFixedSizeCollection<T> c, IGDReadOnlyCollection<int> indices)
        => new ElementsAtFixedSize<T>(c, indices);
}
readonly struct ElementsAtFixedSize<T>(IGDFixedSizeCollection<T> c, IGDReadOnlyCollection<int> indices) : IGDFixedSizeCollection<T>
{
    public T this[int index]
    {
        get => c[indices[index]];
        set => c[indices[index]] = value;
    }
    public int Count => indices.Count;
}