using Get.Data.Collections.Update;
using System.Linq;

namespace Get.Data.Collections.Linq;

public class ElementsAtUpdateBase<T>(IUpdateReadOnlyCollection<T> src, IUpdateReadOnlyCollection<int> indices) : CollectionUpdateEvent<T>
{
    protected override void RegisterItemsChangedEvent()
    {
        indices.ItemsChanged += Indices_ItemsChanged;
        src.ItemsChanged += Src_ItemsChanged;
    }


    protected override void UnregisterItemsChangedEvent()
    {
        indices.ItemsChanged -= Indices_ItemsChanged;
        src.ItemsChanged -= Src_ItemsChanged;
    }


    private void Indices_ItemsChanged(IEnumerable<IUpdateAction<int>> actions)
    {
        InvokeItemsChanged(actions.Select<IUpdateAction<int>, IUpdateAction<T>>(x => x switch
        {
            ItemsAddedUpdateAction<int> added =>
            new ItemsAddedUpdateAction<T>(added.StartingIndex, src.ElementsAt(added.Items), added.OldCollectionCount),
            ItemsRemovedUpdateAction<int> removed =>
            new ItemsRemovedUpdateAction<T>(removed.StartingIndex, src.ElementsAt(removed.Items), removed.OldCollectionCount),
            ItemsMovedUpdateAction<int> moved =>
            new ItemsMovedUpdateAction<T>(moved.OldIndex, moved.NewIndex, src[moved.OldIndexItem], src[moved.NewIndexItem]),
            ItemsReplacedUpdateAction<int> replaced =>
            new ItemsReplacedUpdateAction<T>(replaced.Index, src[replaced.OldItem], src[replaced.NewItem]),
            IndexConditionItemUpdate<T> replaced =>
            new ItemsReplacedUpdateAction<T>(replaced.Index, replaced.OldItem, replaced.NewItem),
            _ => throw new InvalidCastException(),
        }));
    }
    private void Src_ItemsChanged(IEnumerable<IUpdateAction<T>> actions)
    {
        //throw new NotImplementedException();
        // Not Implemented
    }
}