using Get.Data.Collections;
using Get.Data.Collections.Conversion;
using Get.Data.Collections.Linq;
using Get.Data.Collections.Update;
using Get.Data.DataTemplates;

namespace Get.Data.Test;

static class Test
{
    public static void UpdateTest<T>(Func<IUpdateReadOnlyCollection<int>, IUpdateReadOnlyCollection<T>> func, int[] initialSrc, Action<UpdateCollection<int>> act)
    {
        UpdateCollection<int> src = new();
        src.AddRange(initialSrc);
        var dst = func(src);
        var target = new List<T>().AsGDCollection();
        CollectionBindingExtension.Bind(dst, target, debug: true);
        act(src);
        if (IsEqual(dst, target))
        {
            Console.WriteLine("Passed!");
        } else
        {
            Console.WriteLine("Assertion Failed!");
        }
    }
    public static void RandomSimpleUpdateTest<T>(string testName, Func<IUpdateReadOnlyCollection<int>, IUpdateReadOnlyCollection<T>> func)
    {
        UpdateCollection<int> src = new();
        var dst = func(src);
        var target = new List<T>().AsGDCollection();
        CollectionBindingExtension.Bind(dst, target);
        Random r = new(0);
        int[] srcSnapshot = src.EvalArray();
        void Assert(Func<string> getCommand)
        {
            if (IsEqual(dst, target)) return;
            Console.WriteLine($"[{testName}] Assertion Failed!");
            Console.WriteLine($"[{testName}] src = [{string.Join(", ", srcSnapshot)}]");
            Console.WriteLine($"[{testName}] action = src => {getCommand()};");
            Console.WriteLine($"[{testName}] dst (expected) = [{string.Join(", ", dst.AsEnumerable())}]");
            Console.WriteLine($"[{testName}] dst (actual)   = [{string.Join(", ", target.AsEnumerable())}]");
            Console.WriteLine($"[{testName}] To Debug, please run UpdateTest(..., [{string.Join(", ", srcSnapshot)}], src => {getCommand()})");
            Environment.Exit(-1);
        }
        for (int i = 0; i < 100; i++)
        {
            switch (r.Next(0, 4))
            {
                case 0:
                    var no = r.Next();
                    src.Add(no);
                    Assert(() => $"src.Add({no})");
                    break;
                case 1:
                    if (src.Count is 0)
                    {
                        i--;
                        continue;
                    }
                    var idx = r.Next(0, src.Count);
                    src.RemoveAt(idx);
                    Assert(() => $"src.RemoveAt({idx})");
                    break;
                case 2:
                    if (src.Count is 0)
                    {
                        i--;
                        continue;
                    }
                    var idx1 = r.Next(0, src.Count);
                    var idx2 = r.Next(0, src.Count);
                    if (idx1 != idx2)
                    {
                        src.Move(idx1, idx2);
                        Assert(() => $"src.Move({idx1}, {idx2})");
                    } else
                    {
                        i--;
                        continue;
                    }
                    break;
                case 3:
                    if (src.Count is 0)
                    {
                        i--;
                        continue;
                    }
                    var idxrep = r.Next(0, src.Count);
                    var val = r.Next();
                    src[idxrep] = val;
                    Assert(() => $"src[{idxrep}] = {val}");
                    break;
            }
            srcSnapshot = src.EvalArray();
        }
        Console.WriteLine($"[{testName}] Passed!");
    }
    public static bool IsEqual<T>(IGDReadOnlyCollection<T> c1, IGDReadOnlyCollection<T> c2)
    {
        if (c1.Count != c2.Count) return false;
        for (int i = 0; i < c1.Count; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(c1[i], c2[i])) return false;
        }
        return true;
    }
}
