using Get.Data.Collections.Update;
using Get.Data.Helpers;

namespace Get.Data.Collections.Linq;
public static partial class Extension
{
    public static WhereUpdate<T> Where<T>(this IUpdateFixedSizeCollection<T> c, Func<T, bool> filterFunction)
        => new(c, filterFunction);
}
public class WhereUpdate<T>
    : ElementsAtUpdateFixedSize<T>
{
    readonly WhereIndexCondition<T> indexCollection;
    public WhereUpdate(IUpdateFixedSizeCollection<T> src, Func<T, bool> filterFunction) : base(src, new WhereIndexCondition<T>(src, filterFunction).AssignTo(out var a))
    {
        indexCollection = a;
    }
    public Func<T, bool> ConditionFunction
    {
        get => indexCollection.ConditionFunction;
        set => indexCollection.ConditionFunction = value;
    }
}