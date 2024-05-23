using Get.Data.Collections.Conversion;
using Get.Data.Collections.Update;
using System.Diagnostics.CodeAnalysis;
namespace Get.Data.Collections.Linq;

class WhereIndexCondition<T> : CollectionUpdateEvent<int>, IUpdateReadOnlyCollection<int>
{
    public WhereIndexCondition(IUpdateReadOnlyCollection<T> src, Func<T, bool> filterFunction)
    {
        this.src = src;
        ConditionFunction = filterFunction;
    }


    readonly IUpdateReadOnlyCollection<T> src;
    protected readonly IGDCollection<int> PassIndices = new List<int>().AsGDCollection();
    Func<T, bool> _ConditionFunction;
    public Func<T, bool> ConditionFunction
    {
        get => _ConditionFunction;
        [MemberNotNull(nameof(_ConditionFunction))]
        set
        {
            _ConditionFunction = value;
            InvokeItemsChanged(
                OnUpdateConditionFunctionPleaseForceEval().ToList() // ToList: force evaluation to the end
            );
        }
    }

    public int Count => PassIndices.Count;

    public int this[int index] => PassIndices[index];

    IEnumerable<IUpdateAction<int>> OnUpdateConditionFunctionPleaseForceEval()
    {
        int idxFI = 0;
        int i = 0;
        for (; i < src.Count; i++)
        {
            bool shouldExist = ConditionFunction(src[i]);
            bool isAlreadyIncluded = idxFI < PassIndices.Count && PassIndices[idxFI] == i;
            if (shouldExist == isAlreadyIncluded)
            {
                // we're good! move to the next item and Continue!
                if (isAlreadyIncluded)
                    idxFI++;
                if (PassIndices.Count is 0 || i > PassIndices[^1]) break;
                if (idxFI > PassIndices.Count) break;
                continue;
            }
            if (shouldExist) // but not isAlreadyIncluded
            {
                PassIndices.Insert(idxFI, i);
                yield return new ItemsAddedUpdateAction<int>(idxFI, Collection.Single(i), PassIndices.Count - 1);
                idxFI++;
                if (idxFI > PassIndices.Count) break;
            }
            else // should not exist, but isAlreadyIncluded
            {
                PassIndices.RemoveAt(idxFI);
                yield return new ItemsRemovedUpdateAction<int>(idxFI, Collection.Single(i), PassIndices.Count + 1);
                if (PassIndices.Count is 0 || i > PassIndices[^1]) break;
                if (idxFI > PassIndices.Count) break;
            }
        }
        for (; i < src.Count; i++)
        {
            bool shouldExist = ConditionFunction(src[i]);
            if (shouldExist) // but not isAlreadyIncluded
            {
                PassIndices.Add(i);
                yield return new ItemsAddedUpdateAction<int>(PassIndices.Count - 1, Collection.Single(i), PassIndices.Count - 1);
            }
        }
    }
    int SearchLowerNextIdx(int givenIdx)
    {
        // I'm too lazy to do binary search right now.
        for (int i = 0; i < PassIndices.Count; i++)
        {
            if (PassIndices[i] == givenIdx) return i;
            if (PassIndices[i] > givenIdx) return i;// i - 1;
        }
        return -1;
    }
    private void Src_ItemsChanged(IEnumerable<IUpdateAction<T>> actions)
    {
        InvokeItemsChanged(ItemsChangedProcessor(actions).ToArray());
    }
    IEnumerable<IUpdateAction<int>> ItemsChangedProcessor(IEnumerable<IUpdateAction<T>> actions)
    {
        foreach (var action in actions)
        {
            switch (action)
            {
                case ItemsAddedUpdateAction<T> added:
                    {
                        // increment
                        for (int i = 0; i < PassIndices.Count; i++)
                        {
                            if (PassIndices[i] >= added.StartingIndex)
                                PassIndices[i] += added.Items.Count;
                        }
                        int passIdx = -2;
                        for (int i = 0; i < added.Items.Count; i++)
                        {
                            if (ConditionFunction(added.Items[i]))
                            {
                                if (passIdx == -2) passIdx = SearchLowerNextIdx(added.StartingIndex + i);
                                if (passIdx == -1)
                                {
                                    PassIndices.Add(added.StartingIndex + i);
                                    yield return new ItemsAddedUpdateAction<int>(PassIndices.Count - 1,
                                        Collection.Single(added.StartingIndex + i),
                                        PassIndices.Count - 1
                                    );
                                }
                                else
                                {
                                    PassIndices.Insert(passIdx, added.StartingIndex + i);
                                    yield return new ItemsAddedUpdateAction<int>(
                                        passIdx,
                                        Collection.Single(added.StartingIndex + i),
                                        PassIndices.Count - 1
                                    );
                                    passIdx++;
                                }
                            }
                        }
                    }
                    break;
                case ItemsRemovedUpdateAction<T> removed:
                    {
                        for (int i = removed.Items.Count - 1; i >= 0; i--)
                        {
                            // I'm getting very lazy.
                            int idx;
                            if ((idx = PassIndices.IndexOf(removed.StartingIndex + i)) >= 0)
                            {
                                PassIndices.RemoveAt(idx);
                                yield return new ItemsRemovedUpdateAction<int>(
                                    idx,
                                    Collection.Single(removed.StartingIndex + i),
                                    PassIndices.Count + 1
                                );
                            }
                        }
                        // decrement
                        for (int i = 0; i < PassIndices.Count; i++)
                        {
                            if (PassIndices[i] >= removed.StartingIndex)
                                PassIndices[i] -= removed.Items.Count;
                        }
                    }
                    break;
                case ItemsReplacedUpdateAction<T> replaced:
                    {
                        int idx;
                        if ((idx = PassIndices.IndexOf(replaced.Index)) >= 0)
                        {
                            if (!ConditionFunction(replaced.NewItem))
                            {
                                PassIndices.RemoveAt(idx);
                                yield return new ItemsRemovedUpdateAction<int>(
                                    idx, Collection.Single(replaced.Index),
                                    PassIndices.Count + 1
                                );
                            } else
                            {
                                yield return new IndexConditionItemUpdate<T>(
                                    idx, replaced.OldItem,
                                    replaced.NewItem
                                );
                            }
                        }
                        else
                        {
                            if (ConditionFunction(replaced.NewItem))
                            {
                                idx = SearchLowerNextIdx(replaced.Index);
                                if (idx >= 0)
                                {
                                    PassIndices.Insert(idx, replaced.Index);
                                    yield return new ItemsAddedUpdateAction<int>(
                                        idx, Collection.Single(replaced.Index),
                                        PassIndices.Count - 1
                                    );
                                }
                                else
                                {
                                    PassIndices.Add(replaced.Index);
                                    yield return new ItemsAddedUpdateAction<int>(
                                        PassIndices.Count - 1, Collection.Single(replaced.Index),
                                        PassIndices.Count - 1
                                    );
                                }
                            }
                        }
                    }
                    break;
                case ItemsMovedUpdateAction<T> moved:
                    {
                        int idxOld = PassIndices.IndexOf(moved.OldIndex), idxNew = PassIndices.IndexOf(moved.NewIndex);
                        if (idxOld < 0 && idxNew < 0) break;
                        if (idxOld >= 0 && idxNew >= 0)
                        {
                            yield return new ItemsMovedUpdateAction<int>(
                                idxOld, idxNew, PassIndices[idxOld], PassIndices[idxNew]
                            );
                            break;
                        }
                        if (idxOld < 0)
                        {
                            moved = new(
                                OldIndex: moved.NewIndex,
                                NewIndex: moved.OldIndex,
                                OldIndexItem: moved.NewIndexItem,
                                NewIndexItem: moved.OldIndexItem);

                            (idxOld, idxNew) = (idxNew, idxOld);
                        }
                        {
                            PassIndices.RemoveAt(idxOld);
                            yield return new ItemsRemovedUpdateAction<int>(
                                idxOld, Collection.Single(moved.OldIndex),
                                    PassIndices.Count + 1
                            );
                            if (moved.OldIndex > moved.NewIndex)
                            {
                                while (idxOld > 0 && PassIndices[idxOld - 1] > moved.NewIndex)
                                    idxOld--;
                                PassIndices.Insert(idxOld, moved.NewIndex);
                                yield return new ItemsAddedUpdateAction<int>(
                                    idxOld, Collection.Single(moved.NewIndex),
                                        PassIndices.Count - 1
                                );
                            }
                            else
                            {
                                while (idxOld < 0 && PassIndices[idxOld] < moved.NewIndex)
                                    idxOld++;
                                PassIndices.Insert(idxOld, moved.NewIndex);
                                yield return new ItemsAddedUpdateAction<int>(
                                    idxOld, Collection.Single(moved.NewIndex),
                                        PassIndices.Count - 1
                                );
                            }
                        }
                    }
                    break;
            }
        }
    }
    protected override void RegisterItemsChangedEvent()
    {
        src.ItemsChanged += Src_ItemsChanged;
    }

    protected override void UnregisterItemsChangedEvent()
    {
        src.ItemsChanged -= Src_ItemsChanged;
    }
}
readonly record struct IndexConditionItemUpdate<T>(int Index, T OldItem, T NewItem) : IUpdateAction<int>;