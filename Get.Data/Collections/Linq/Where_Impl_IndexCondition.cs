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
                yield return new ItemsAddedUpdateAction<int>(idxFI, Collection.Single(i));
                idxFI++;
                if (idxFI > PassIndices.Count) break;
            }
            else // should not exist, but isAlreadyIncluded
            {
                PassIndices.RemoveAt(idxFI);
                yield return new ItemsRemovedUpdateAction<int>(idxFI, Collection.Single(i));
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
                yield return new ItemsAddedUpdateAction<int>(PassIndices.Count - 1, Collection.Single(i));
            }
        }
    }
    int SearchLowerNextIdx(int givenIdx)
    {
        // I'm too lazy to do binary search right now.
        for (int i = 0; i < PassIndices.Count; i++)
        {
            if (PassIndices[i] == givenIdx) return i;
            if (PassIndices[i] > givenIdx) return i - 1;
        }
        return -1;
    }
    private void Src_ItemsChanged(IEnumerable<IUpdateAction<T>> actions)
    {
        InvokeItemsChanged(ItemsChangedProcessor(actions));
    }
    IEnumerable<IUpdateAction<int>> ItemsChangedProcessor(IEnumerable<IUpdateAction<T>> actions)
    {
        foreach (var action in actions)
        {
            switch (action)
            {
                case ItemsAddedUpdateAction<T> added:
                    {
                        int passIdx = -2;
                        for (int i = 0; i < added.Items.Count; i++)
                        {
                            if (ConditionFunction(added.Items[i]))
                            {
                                if (passIdx == -2) SearchLowerNextIdx(added.StartingIndex + i);
                                if (passIdx == -1)
                                {
                                    PassIndices.Add(i);
                                    yield return new ItemsAddedUpdateAction<int>(PassIndices[i] - 1,
                                        Collection.Single(added.StartingIndex + i)
                                    );
                                }
                                else
                                {
                                    PassIndices.Insert(passIdx, i);
                                    yield return new ItemsAddedUpdateAction<int>(
                                        passIdx,
                                        Collection.Single(added.StartingIndex + i)
                                    );
                                    passIdx++;
                                }
                            }
                        }
                    }
                    break;
                case ItemsRemovedUpdateAction<T> removed:
                    {
                        for (int i = 0; i < removed.Items.Count; i++)
                        {
                            // I'm getting very lazy.
                            int idx;
                            if ((idx = PassIndices.IndexOf(removed.StartingIndex + i)) >= 0)
                            {
                                PassIndices.RemoveAt(idx);
                                yield return new ItemsRemovedUpdateAction<int>(
                                    idx,
                                    Collection.Single(removed.StartingIndex + i)
                                );
                            }
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
                                    idx, Collection.Single(replaced.Index)
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
                                        idx, Collection.Single(replaced.Index)
                                    );
                                }
                                else
                                {
                                    PassIndices.Add(replaced.Index);
                                    yield return new ItemsAddedUpdateAction<int>(
                                        PassIndices.Count - 1, Collection.Single(replaced.Index)
                                    );
                                }
                            }
                        }
                    }
                    break;
                case ItemsMovedUpdateAction<T> moved:
                    {
                        int idx;
                        if ((idx = PassIndices.IndexOf(moved.OldIndex)) >= 0)
                        {
                            PassIndices.RemoveAt(idx);
                            yield return new ItemsRemovedUpdateAction<int>(
                                idx, Collection.Single(moved.OldIndex)
                            );
                            if (moved.OldIndex > moved.NewIndex)
                            {
                                while (idx > 0 && PassIndices[idx - 1] > moved.NewIndex)
                                    idx--;
                                PassIndices.Insert(idx, moved.NewIndex);
                                yield return new ItemsAddedUpdateAction<int>(
                                    idx, Collection.Single(moved.NewIndex)
                                );
                            }
                            else
                            {
                                while (idx < 0 && PassIndices[idx] < moved.NewIndex)
                                    idx++;
                                PassIndices.Insert(idx, moved.NewIndex);
                                yield return new ItemsAddedUpdateAction<int>(
                                    idx, Collection.Single(moved.NewIndex)
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