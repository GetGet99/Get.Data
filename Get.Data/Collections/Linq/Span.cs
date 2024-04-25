using Get.Data.Collections;

namespace Get.Data.Collections.Linq;

public static partial class Extension {
    internal static IGDReadOnlyCollection<T> Span<T>(this IGDReadOnlyCollection<T> c, Range range)
        => new SpanHandler<T>(c, range);
}

readonly struct SpanHandler<T>(IGDReadOnlyCollection<T> c, Range range) : IGDReadOnlyCollection<T>
{
    // probably should do a bit of caching
    public T this[int index] => c[SafeGetOffsetAndLength(range, c.Count).Offset + index];
    public int Count => SafeGetOffsetAndLength(range, c.Count).Length;
    static (int Offset, int Length) SafeGetOffsetAndLength(Range r, int length)
    {
        int start;
        Index startIndex = r.Start;
        if (startIndex.IsFromEnd)
            start = length - startIndex.Value;
        else
            start = startIndex.Value;

        int end;
        Index endIndex = r.End;
        if (endIndex.IsFromEnd)
            end = length - endIndex.Value;
        else
            end = endIndex.Value;

        if ((uint)end > (uint)length || (uint)start > (uint)end)
        {
            return (0, 0);
        }

        return (start, end - start);
    }
}