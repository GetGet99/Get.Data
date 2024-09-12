using Get.Data.Collections.Update;
using System.Collections.ObjectModel;

namespace Get.Data.Collections.Linq;

public static partial class Extension
{
    public static IGDCollection<T> AsCollection<T>(this IGDCollection<T> c) => c;
    public static IGDReadOnlyCollection<T> AsReadOnly<T>(this IGDReadOnlyCollection<T> c) => c;
    public static IGDFixedSizeCollection<T> AsFixedSize<T>(this IGDFixedSizeCollection<T> c) => c;
    public static IUpdateCollection<T> AsUpdate<T>(this IUpdateCollection<T> c) => c;
    public static IUpdateReadOnlyCollection<T> AsUpdateReadOnly<T>(this IUpdateReadOnlyCollection<T> c) => c;
    public static IUpdateFixedSizeCollection<T> AsUpdateFixedSize<T>(this IUpdateFixedSizeCollection<T> c) => c;
    public static IUpdateReadOnlyCollection<T> ToUpdateReadOnly<T>(this IUpdateReadOnlyCollection<T> c) => c;
}
public readonly partial struct GDReadOnlyCollectionHelper<T>(IGDReadOnlyCollection<T> collection);
public readonly partial struct GDFixedSizeCollectionHelper<T>(IGDFixedSizeCollection<T> collection)
{
    public GDReadOnlyCollectionHelper<T> ReadOnly => new(collection);
}
public readonly partial struct GDCollectionHelper<T>(IGDCollection<T> collection)
{
    public GDReadOnlyCollectionHelper<T> ReadOnly => new(collection);
    public GDFixedSizeCollectionHelper<T> FixedSize => new(collection);
}
public readonly partial struct GDUpdateReadOnlyCollectionHelper<T>(IUpdateReadOnlyCollection<T> collection);
public readonly partial struct GDUpdateFixedSizeCollectionHelper<T>(IUpdateFixedSizeCollection<T> collection)
{
    public GDUpdateReadOnlyCollectionHelper<T> ReadOnly => new(collection);
}
public readonly partial struct GDUpdateCollectionHelper<T>(IUpdateCollection<T> collection)
{
    public GDUpdateReadOnlyCollectionHelper<T> ReadOnly => new(collection);
    public GDUpdateFixedSizeCollectionHelper<T> FixedSize => new(collection);
}