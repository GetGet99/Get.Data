using Get.Data.Collections.Update;
namespace Get.Data.Collections.Linq;

public static partial class Extension
{
    public static SpanFixedRegionReadOnlyUpdate<T> Span<T>(this IUpdateReadOnlyCollection<T> c, int initialOffset, int initialLength)
        => new(c, initialOffset, initialLength);
}
public class SpanFixedRegionReadOnlyUpdate<T>(IUpdateReadOnlyCollection<T> src, int initialOffset = 0, int initialLength = 0) : SpanFixedRegionUpdateBase<T>(src, initialOffset, initialLength), IUpdateReadOnlyCollection<T>
{
    public T this[int index]
    {
        get => src[index + Offset];
    }
}