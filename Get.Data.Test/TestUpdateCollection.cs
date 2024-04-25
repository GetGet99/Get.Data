using Get.Data.Collections;
using Get.Data.Collections.Conversion;
using Get.Data.Collections.Linq;
using Get.Data.Collections.Update;
using Get.Data.DataTemplates;
using System.Diagnostics;

namespace Get.Data.Test;

static class TestUpdateCollection
{
    public static void RunTest()
    {
        RandomSimpleUpdateTest("Select", x => x.Select(x => x.ToString()));
        RandomSimpleUpdateTest("WithIndex", x => x.WithIndex());
        RandomSimpleUpdateTest("Reverse", x => x.Reverse());
        // still fails
        RandomSimpleUpdateTest("Span", x => x.Span(3, 10));
        // no readonly implementation of Where yet
        // RandomSimpleUpdateTest("Where", x => x.Where());
    }
    public static void UpdateTest<T>(Func<IUpdateReadOnlyCollection<int>, IUpdateReadOnlyCollection<T>> func, int[] initialSrc, Action<UpdateCollection<int>> act)
    {
        UpdateCollection<int> src = new();
        src.AddRange(initialSrc);
        var dst = func(src);
        var target = new List<T>().AsGDCollection();
        CollectionBindingExtension.Bind(dst, target, debug: true);
        Debugger.Break();
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
            Fail(getCommand());
            PrintStat();
            Environment.Exit(-1);
        }
        int addedFirst = 0, addedLast = 0, addedRandom = 0, removedFirst = 0, removedLast = 0, removedRandom = 0, moved = 0, replaced = 0;
        void PrintStat()
        {
            Console.WriteLine($"[{testName}] {addedFirst} addedFirst, {addedLast} addedLast, {addedRandom} addedRandom");
            Console.WriteLine($"[{testName}] {removedFirst} removedFirst, {removedLast} removedLast, {removedRandom} removedRandom");
            Console.WriteLine($"[{testName}] {moved} moved, {replaced} replaced");
        }
        void Fail(string cmd)
        {
            Console.WriteLine($"[{testName}] Assertion Failed!");
            Console.WriteLine($"[{testName}] src = [{string.Join(", ", srcSnapshot)}]");
            Console.WriteLine($"[{testName}] action = src => {cmd};");
            Console.WriteLine($"[{testName}] dst (expected) = [{string.Join(", ", dst.AsEnumerable())}]");
            Console.WriteLine($"[{testName}] dst (actual)   = [{string.Join(", ", target.AsEnumerable())}]");
            Console.WriteLine($"[{testName}] To Debug, please run UpdateTest(..., [{string.Join(", ", srcSnapshot)}], src => {cmd})");
        }
        void AssertEx(Action a, Func<string> getCommand)
        {
            try
            {
                a();
                Assert(getCommand);
            } catch (Exception e)
            {
                Console.WriteLine($"[{testName}] Assertion Failed! (Exception)");
                Console.WriteLine($"[{testName}] src = [{string.Join(", ", srcSnapshot)}]");
                Console.WriteLine($"[{testName}] action = src => {getCommand()};");
                Console.WriteLine($"[{testName}] exception =");
                Console.WriteLine(e.GetType().FullName);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine($"[{testName}] To Debug, please run UpdateTest(..., [{string.Join(", ", srcSnapshot)}], src => {getCommand()})");
                throw;
            }
        }
        for (int i = 0; i < 500; i++)
        {
            switch (r.Next(0, 8))
            {
                case 0:
                    {
                        var no = r.Next();
                        AssertEx(() => src.Add(no), () => $"src.Add({no})");
                        addedLast++;
                        break;
                    }
                case 1:
                    {
                        var no = r.Next();
                        AssertEx(() => src.Insert(0, no), () => $"src.Insert(0, {no})");
                        addedFirst++;
                        break;
                    }
                case 2:
                    {
                        if (src.Count is <2)
                        {
                            i--;
                            continue;
                        }
                        var no = r.Next();
                        var idx = r.Next(1, src.Count);
                        AssertEx(() => src.Insert(idx, no), () => $"src.Insert({idx}, {no})");
                        addedRandom++;
                        break;
                    }
                case 3:
                    {
                        if (src.Count is 0)
                        {
                            i--;
                            continue;
                        }
                        AssertEx(() => src.RemoveAt(src.Count - 1), () => $"src.RemoveAt({src.Count - 1})");
                        removedLast++;
                        break;
                    }
                case 4:
                    {
                        if (src.Count is 0)
                        {
                            i--;
                            continue;
                        }
                        AssertEx(() => src.RemoveAt(0), () => $"src.RemoveAt(0)");
                        removedFirst++;
                        break;
                    }
                case 5:
                    {
                        if (src.Count is 0)
                        {
                            i--;
                            continue;
                        }
                        var idx = r.Next(0, src.Count);
                        AssertEx(() => src.RemoveAt(idx), () => $"src.RemoveAt({idx})");
                        removedRandom++;
                        break;
                    }
                case 6:
                    {
                        if (src.Count is 0)
                        {
                            i--;
                            continue;
                        }
                        var idx1 = r.Next(0, src.Count);
                        var idx2 = r.Next(0, src.Count);
                        if (idx1 != idx2)
                        {
                            AssertEx(() => src.Move(idx1, idx2), () => $"src.Move({idx1}, {idx2})");
                            moved++;
                        }
                        else
                        {
                            i--;
                            continue;
                        }
                        break;
                    }
                case 7:
                    if (src.Count is 0)
                    {
                        i--;
                        continue;
                    }
                    var idxrep = r.Next(0, src.Count);
                    var val = r.Next();
                    AssertEx(() => src[idxrep] = val, () => $"src[{idxrep}] = {val}");
                    replaced++;
                    break;
            }
            srcSnapshot = src.EvalArray();
        }
        Console.WriteLine($"[{testName}] Passed!");
        PrintStat();
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
