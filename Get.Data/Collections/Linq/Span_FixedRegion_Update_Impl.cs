using Get.Data.Collections.Update;
using System.Diagnostics;
namespace Get.Data.Collections.Linq;

public class SpanFixedRegionUpdateBase<T>(IUpdateReadOnlyCollection<T> src, int initialOffset = 0, int initialLength = 0) : CollectionUpdateEvent<T>
{
    IGDReadOnlyCollection<T> cached = src.EvalFixedSize();
    int _Offset = initialOffset;
    public int Offset { get => _Offset; set => _Offset = value; }
    int _Length = initialLength;
    public int Length { get => _Length; set => _Length = value; }
    public int Count => Math.Min(Length, Math.Max(0, src.Count - Offset));
    private void Src_ItemsChanged(IEnumerable<IUpdateAction<T>> actions)
    {
        InvokeItemsChanged(ItemsChangedProcessor(actions).ToList() /* eval */);
    }
    IEnumerable<IUpdateAction<T>> ItemsChangedProcessor(IEnumerable<IUpdateAction<T>> actions)
    {
        foreach (var action in actions)
        {
            switch (action)
            {
                case ItemsAddedUpdateAction<T> added:
                    if (added.StartingIndex >= Offset && added.StartingIndex - Offset < Length)
                        yield return new ItemsAddedUpdateAction<T>(
                            added.StartingIndex - Offset,
                            added.Items.Span(0..(Math.Min(Length - (added.StartingIndex - Offset), added.Items.Count))),
                            Math.Min(Length, Math.Max(0, added.OldCollectionCount - Offset))
                        );
                    else if (added.StartingIndex < Offset)
                    {
                        var affectedLength = Math.Min(Length, added.Items.Count);
                        var cachedSpan = cached.Span(Offset..(Offset + Length));
                        var oldItems = cached.Span(added.StartingIndex..Offset);
                        var count = Math.Min(Length, Math.Max(0, added.OldCollectionCount - Offset));
                        if (oldItems.Count >= affectedLength)
                        {
                            var addedItems = oldItems.Span(^affectedLength..);
                            yield return new ItemsAddedUpdateAction<T>(0,
                                addedItems,
                                count
                            );
                            count += addedItems.Count;

                            var removedItems = cachedSpan.Span(^affectedLength..);
                            yield return new ItemsRemovedUpdateAction<T>(0,
                                removedItems,
                                count
                            );
                            count -= removedItems.Count;
                        }
                        else
                        {
                            if (oldItems.Count > 0)
                                yield return new ItemsAddedUpdateAction<T>(0,
                                    oldItems,
                                    count
                                );
                            count += oldItems.Count;
                            if ((cachedSpan = cachedSpan.Span(^oldItems.Count..)).Count > 0)
                                yield return new ItemsRemovedUpdateAction<T>(0,
                                    cachedSpan,
                                    count
                                );
                            count -= cachedSpan.Count;
                            var addedItems = added.Items.Span(^Math.Max(0, affectedLength - Offset)..);
                            yield return new ItemsAddedUpdateAction<T>(0,
                                addedItems,
                                count
                            );
                            count += addedItems.Count;
                            //if (affectedLength - Offset <= cachedSpan.Count)
                            //{
                            var removedItems = cachedSpan.Span(^Math.Max(0, affectedLength - Offset)..);
                            yield return new ItemsRemovedUpdateAction<T>(0,
                                 removedItems,
                                 count
                             );
                            count -= removedItems.Count;
                            //}
                            //else
                            //{

                            //}
                        }
                        yield return new ItemsRemovedUpdateAction<T>(0,
                            cached.Span(Math.Max(0, Offset - Math.Min(Length, added.Items.Count))..Offset),
                            count
                        );
                    }
                    break;
                case ItemsRemovedUpdateAction<T> removed:
                    if (removed.StartingIndex >= Offset && removed.StartingIndex - Offset < Length)
                        yield return new ItemsRemovedUpdateAction<T>(
                            removed.StartingIndex - Offset,
                            removed.Items.Span(0..(Math.Min(Length - (removed.StartingIndex - Offset), removed.Items.Count))),
                            Math.Min(Length, Math.Max(0, removed.OldCollectionCount - Offset))
                        );
                    else if (removed.StartingIndex < Offset)
                        yield return new ItemsRemovedUpdateAction<T>(0,
                            cached.Span(Math.Max(0, Offset - Math.Min(Length, removed.Items.Count))..Offset),
                            Math.Min(Length, Math.Max(0, removed.OldCollectionCount - Offset))
                        );
                    // need to invoke items added
                    break;
                case ItemsReplacedUpdateAction<T> replaced:
                    if (replaced.Index >= Offset && replaced.Index - Offset < Count)
                        yield return new ItemsReplacedUpdateAction<T>(
                            replaced.Index - Offset, replaced.OldItem, replaced.NewItem
                        );
                    break;
                case ItemsMovedUpdateAction<T> moved:
                    bool isOldIn = moved.OldIndex >= Offset && moved.OldIndex - Offset < Count;
                    bool isNewIn = moved.NewIndex >= Offset && moved.NewIndex - Offset < Count;
                    if (isOldIn && isNewIn)
                        yield return new ItemsMovedUpdateAction<T>(
                            moved.OldIndex - Offset, moved.NewIndex - Offset,
                            moved.OldIndexItem, moved.NewIndexItem
                        );
                    else
                    {
                        if (isNewIn)
                        {
                            yield return new ItemsReplacedUpdateAction<T>(
                                moved.NewIndex - Offset, moved.NewIndexItem, moved.OldIndexItem
                            );
                        }
                        else if (isOldIn)
                        {
                            yield return new ItemsReplacedUpdateAction<T>(
                                moved.OldIndex - Offset, moved.OldIndexItem, moved.NewIndexItem
                            );
                        }
                    }
                    break;
            }
        }
        cached = src.EvalFixedSize();
    }

    protected override void RegisterItemsChangedEvent()
        => src.ItemsChanged += Src_ItemsChanged;
    protected override void UnregisterItemsChangedEvent()
        => src.ItemsChanged -= Src_ItemsChanged;


#if DEBUG
    public override string ToString()
    {
        return $"{src} > Span";
    }
#endif
}