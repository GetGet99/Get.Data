using Get.Data.Collections.Update;
namespace Get.Data.Collections.Linq;
public static partial class Extension
{
    public static SpanFixedRegionUpdate<T> Span<T>(this IUpdateFixedSizeCollection<T> c, int initialOffset, int initialLength)
        => new(c, initialOffset, initialLength);
}
public class SpanFixedRegionUpdate<T>(IUpdateFixedSizeCollection<T> src, int initialOffset = 0, int initialLength = 0) : SpanFixedRegionUpdateBase<T>(src, initialOffset, initialLength), IUpdateFixedSizeCollection<T>
{
    public T this[int index]
    {
        get => src[index + Offset];
        set => src[index + Offset] = value;
    }
}