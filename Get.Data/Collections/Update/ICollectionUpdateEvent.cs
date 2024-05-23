using Get.Data.Collections;

namespace Get.Data.Collections.Update;
public interface ICollectionUpdateEvent<T>
{
    event UpdateCollectionItemsChanged<T> ItemsChanged;
}
public interface IUpdateAction<T>
{
    
}
public readonly record struct ItemsAddedUpdateAction<T>(int StartingIndex, IGDReadOnlyCollection<T> Items, int OldCollectionCount) : IUpdateAction<T>;
public readonly record struct ItemsRemovedUpdateAction<T>(int StartingIndex, IGDReadOnlyCollection<T> Items, int OldCollectionCount) : IUpdateAction<T>;
public readonly record struct ItemsReplacedUpdateAction<T>(int Index, T OldItem, T NewItem) : IUpdateAction<T>;
public readonly record struct ItemsMovedUpdateAction<T>(int OldIndex, int NewIndex, T OldIndexItem, T NewIndexItem) : IUpdateAction<T>;

public delegate void UpdateCollectionItemsChanged<T>(IEnumerable<IUpdateAction<T>> actions);

public abstract class CollectionUpdateEvent<T> : ICollectionUpdateEvent<T>
{

    public event UpdateCollectionItemsChanged<T> ItemsChanged
    {
        add
        {
            if (_ItemsChanged == null) RegisterItemsChangedEvent();
            _ItemsChanged += value;
        }
        remove
        {
            _ItemsChanged -= value;
            if (_ItemsChanged == null) UnregisterItemsChangedEvent();
        }
    }
    UpdateCollectionItemsChanged<T> _ItemsChanged;
    protected void InvokeItemsChanged(IEnumerable<IUpdateAction<T>> actions) => _ItemsChanged?.Invoke(actions);
    protected abstract void RegisterItemsChangedEvent();
    protected abstract void UnregisterItemsChangedEvent();
}