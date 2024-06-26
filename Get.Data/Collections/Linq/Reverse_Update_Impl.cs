using Get.Data.Collections.Update;

namespace Get.Data.Collections.Linq;
abstract class ReverseUpdateBase<T>(IUpdateReadOnlyCollection<T> src) : CollectionUpdateEvent<T>
{
    protected int FlipIndex(int index) => src.Count == 0 ? 0 : src.Count - 1 - index;
    protected int FlipIndex(int index, int count) => count == 0 ? 0 : count - 1 - index;
    protected sealed override void RegisterItemsChangedEvent() => src.ItemsChanged += Src_ItemsChanged;
    protected sealed override void UnregisterItemsChangedEvent() => src.ItemsChanged -= Src_ItemsChanged;

    private void Src_ItemsChanged(IEnumerable<IUpdateAction<T>> actions)
    {
        InvokeItemsChanged(actions.Select(
            x => (IUpdateAction<T>)(x switch
            {
                ItemsAddedUpdateAction<T> added => new ItemsAddedUpdateAction<T>(
                    (added.OldCollectionCount == 0 || added.StartingIndex == added.OldCollectionCount) ? 0 :
                    added.OldCollectionCount - added.StartingIndex, added.Items.Reverse(), added.OldCollectionCount
                ),
                ItemsRemovedUpdateAction<T> removed => new ItemsRemovedUpdateAction<T>(FlipIndex(removed.StartingIndex, removed.OldCollectionCount), removed.Items.Reverse(), removed.OldCollectionCount),
                ItemsReplacedUpdateAction<T> replaced => new ItemsReplacedUpdateAction<T>(FlipIndex(replaced.Index), replaced.OldItem, replaced.NewItem),
                ItemsMovedUpdateAction<T> moved => new ItemsMovedUpdateAction<T>(FlipIndex(moved.OldIndex), FlipIndex(moved.NewIndex), moved.OldIndexItem, moved.NewIndexItem),
                _ => throw new InvalidCastException()
            })));
    }
#if DEBUG
    public override string ToString()
    {
        return $"{src} > Reverse";
    }
#endif
}