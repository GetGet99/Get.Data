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
        // ElementsAt (no test currently)
        RandomSimpleUpdateTest("Select", x => x.Select(x => x.ToString()));
        RandomSimpleUpdateTest("WithIndex", x => x.WithIndex());
        RandomSimpleUpdateTest("Reverse", x => x.Reverse());
        RandomSimpleUpdateTest("Span", x => x.Span(3, 10));
        RandomSimpleUpdateTest("Where", x => x.Where(x => (x & 1) == 0));
        //UpdateTest(x => x.Where(x => (x & 1) == 0), [1241177922, 1110752158, 1256935581, 265479717, 1503106162, 860472952], src => src.Move(3, 4));
        // no readonly implementation of Where yet
        // RandomSimpleUpdateTest("Where", x => x.Where());
    }
    public static void UpdateTest<T>(Func<IUpdateReadOnlyCollection<int>, IUpdateReadOnlyCollection<T>> func, int[] initialSrc, Action<UpdateCollection<int>> act)
    {
        UpdateCollection<int> src = new();
        src.AddRange(initialSrc);
        var dst = func(src);
        var dstSnapshot = dst.EvalArray();
        var target = new List<T>().AsGDCollection();
        CollectionBindingExtension.Bind(dst, target, debug: true);
        Debugger.Break();
        act(src);
        if (IsEqual(dst, target))
        {
            Console.WriteLine("Passed!");
        } else
        {
            Console.WriteLine($"[Manual] Assertion Failed!");
            Console.WriteLine($"[Manual] src (before)   = [{string.Join(", ", initialSrc)}]");
            Console.WriteLine($"[Manual] src (after)    = [{string.Join(", ", src.AsEnumerable())}]");
            Console.WriteLine($"[Manual] dst (before)   = [{string.Join(", ", dstSnapshot)}]");
            Console.WriteLine($"[Manual] dst (by read)  = [{string.Join(", ", dst.AsEnumerable())}]");
            Console.WriteLine($"[Manual] dst (by event) = [{string.Join(", ", target.AsEnumerable())}]");
            Environment.Exit(-1);
        }
    }
    public static void RandomSimpleUpdateTest<T>(string testName, Func<IUpdateReadOnlyCollection<int>, IUpdateReadOnlyCollection<T>> func, int? debugAt = null, bool debugAll = false)
    {
        UpdateCollection<int> src = new();
        var dst = func(src);
        var target = new List<T>().AsGDCollection();
        CollectionBindingExtension.Bind(dst, target);
        Random r = new(0);
        int[] srcSnapshot = src.EvalArray();
        T[] dstSnapshot = dst.EvalArray();
        void Assert(Func<string> getCommand, int i)
        {
            if (IsEqual(dst, target)) return;
            Fail(getCommand(), i);
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
        void Fail(string cmd, int i)
        {
            Console.WriteLine($"[{testName}] Assertion Failed!");
            PrintAction(cmd, i);
        }
        void PrintAction(string cmd, int i)
        {
            Console.WriteLine($"[{testName}] src (before)   = [{string.Join(", ", srcSnapshot)}]");
            Console.WriteLine($"[{testName}] src (after)    = [{string.Join(", ", src.AsEnumerable())}]");
            Console.WriteLine($"[{testName}] action = src => {cmd};");
            Console.WriteLine($"[{testName}] dst (before)   = [{string.Join(", ", dstSnapshot)}]");
            Console.WriteLine($"[{testName}] dst (by read)  = [{string.Join(", ", dst.AsEnumerable())}]");
            Console.WriteLine($"[{testName}] dst (by event) = [{string.Join(", ", target.AsEnumerable())}]");
            Console.WriteLine($"[{testName}] To Debug, please run UpdateTest(..., [{string.Join(", ", srcSnapshot)}], src => {cmd})");
            Console.WriteLine($"[{testName}] Or run RandomSimpleUpdateTest(..., debugAt: {i})");
        }
        void AssertEx(Action a, Func<string> getCommand, int i)
        {
            try
            {
                a();
                Assert(getCommand, i);
                if (debugAll)
                {
                    Console.WriteLine($"[{testName}] ------------------------");
                    PrintAction(getCommand(), i);
                    Console.WriteLine($"[{testName}] Press Return To Continue");
                    Console.ReadLine();
                }
            } catch (Exception e)
            {
                Console.WriteLine($"[{testName}] Assertion Failed! (Exception)");
                Console.WriteLine($"[{testName}] src (before)   = [{string.Join(", ", srcSnapshot)}]");
                Console.WriteLine($"[{testName}] action = src => {getCommand()};");
                Console.WriteLine($"[{testName}] dst (before)   = [{string.Join(", ", dstSnapshot)}]");
                Console.WriteLine($"[{testName}] exception =");
                Console.WriteLine(e.GetType().FullName);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                PrintStat();
                Console.WriteLine($"[{testName}] To Debug, please run UpdateTest(..., [{string.Join(", ", srcSnapshot)}], src => {getCommand()})");
                throw;
            }
        }
        for (int i = 0; i < 500; i++)
        {
            if (i == debugAt) Debugger.Break();
            switch (r.Next(0, 8))
            {
                case 0:
                    {
                        var no = r.Next();
                        AssertEx(() => src.Add(no), () => $"src.Add({no})", i);
                        addedLast++;
                        break;
                    }
                case 1:
                    {
                        var no = r.Next();
                        AssertEx(() => src.Insert(0, no), () => $"src.Insert(0, {no})", i);
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
                        AssertEx(() => src.Insert(idx, no), () => $"src.Insert({idx}, {no})", i);
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
                        var pos = src.Count - 1;
                        AssertEx(() => src.RemoveAt(pos), () => $"src.RemoveAt({pos})", i);
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
                        AssertEx(() => src.RemoveAt(0), () => $"src.RemoveAt(0)", i);
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
                        AssertEx(() => src.RemoveAt(idx), () => $"src.RemoveAt({idx})", i);
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
                            AssertEx(() => src.Move(idx1, idx2), () => $"src.Move({idx1}, {idx2})", i);
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
                    AssertEx(() => src[idxrep] = val, () => $"src[{idxrep}] = {val}", i);
                    replaced++;
                    break;
            }
            srcSnapshot = src.EvalArray();
            dstSnapshot = dst.EvalArray();
        }
        Console.WriteLine($"[{testName}] Passed!");
        PrintStat();
    }
    public static bool IsEqual<T>(IGDReadOnlyCollection<T> c1, IGDReadOnlyCollection<T> c2)
    {
        if (c1.Count != c2.Count) return false;
        for (int i = 0; i < c1.Count; i++)
        {
            try
            {
                if (!EqualityComparer<T>.Default.Equals(c1[i], c2[i])) return false;
            } catch
            {
                Debugger.Break();
                i--;
                if (Console.Read() != 't')
                    continue;
                else
                    throw;
            }
        }
        return true;
    }
}
