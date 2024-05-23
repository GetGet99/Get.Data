using Get.Data.Collections.Update;
using Get.Data.Helpers;

namespace Get.Data.Collections.Linq;
public static partial class Extension
{
    public static WhereReadOnlyUpdate<T> Where<T>(this IUpdateReadOnlyCollection<T> c, Func<T, bool> filterFunction)
        => new(c, filterFunction);
}
public class WhereReadOnlyUpdate<T>
    : ElementsAtUpdateReadOnly<T>
{
    readonly WhereIndexCondition<T> indexCollection;
    public WhereReadOnlyUpdate(IUpdateReadOnlyCollection<T> src, Func<T, bool> filterFunction) : base(src, new WhereIndexCondition<T>(src, filterFunction).AssignTo(out var a))
    {
        indexCollection = a;
    }
    public Func<T, bool> ConditionFunction
    {
        get => indexCollection.ConditionFunction;
        set => indexCollection.ConditionFunction = value;
    }
}
