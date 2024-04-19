﻿using Get.Data.Collections.Update;
using System;
using System.Reflection;
namespace Get.Data.Collections.Linq;

public class SpanFixedRegionUpdate<T>(IUpdateFixedSizeCollection<T> src, int initialOffset = 0, int initialLength = 0) : CollectionUpdateEvent<T>, IUpdateFixedSizeCollection<T>
{
    IGDReadOnlyCollection<T> cached = src.EvalFixedSize();
    public T this[int index]
    {
        get => src[index + Offset];
        set => src[index + Offset] = value;
    }
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
                            added.StartingIndex + Offset,
                            added.Items.Span(0..(added.StartingIndex + Math.Min(Length - added.StartingIndex, added.Items.Count)))
                        );
                    else if (added.StartingIndex < Offset)
                    {
                        var affectedLength = Math.Min(Length, added.Items.Count);
                        var cachedSpan = cached.Span(Offset..(Offset + Length));
                        var oldItems = cached.Span(added.StartingIndex..Offset);
                        if (oldItems.Count >= affectedLength)
                        {
                            yield return new ItemsAddedUpdateAction<T>(0,
                                oldItems.Span(^affectedLength..)
                            );
                            yield return new ItemsRemovedUpdateAction<T>(0,
                                cachedSpan.Span(^affectedLength..)
                            );
                        }
                        else
                        {
                            yield return new ItemsAddedUpdateAction<T>(0,
                                oldItems
                            );
                            yield return new ItemsRemovedUpdateAction<T>(0,
                                (cachedSpan = cachedSpan.Span(^oldItems.Count..))
                            );
                            yield return new ItemsAddedUpdateAction<T>(0,
                                added.Items.Span(^(affectedLength - Offset)..)
                            );
                            //if (affectedLength - Offset <= cachedSpan.Count)
                            //{
                            yield return new ItemsRemovedUpdateAction<T>(0,
                                 cachedSpan.Span(^(affectedLength - Offset)..)
                             );
                            //}
                            //else
                            //{

                            //}
                        }
                        yield return new ItemsRemovedUpdateAction<T>(0,
                            cached.Span(Math.Max(0, Offset - Math.Min(Length, added.Items.Count))..Offset)
                        );
                    }
                    break;
                case ItemsRemovedUpdateAction<T> removed:
                    if (removed.StartingIndex >= Offset && removed.StartingIndex - Offset < Length)
                        yield return new ItemsRemovedUpdateAction<T>(
                            removed.StartingIndex + Offset,
                            removed.Items.Span(0..(removed.StartingIndex + Math.Min(Length - removed.StartingIndex, removed.Items.Count)))
                        );
                    else if (removed.StartingIndex < Offset)
                        yield return new ItemsRemovedUpdateAction<T>(0,
                            cached.Span(Math.Max(0, Offset - Math.Min(Length, removed.Items.Count))..Offset)
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
                                moved.NewIndex, moved.NewIndexItem, moved.OldIndexItem
                            );
                        }
                        else if (isOldIn)
                        {
                            yield return new ItemsReplacedUpdateAction<T>(
                                moved.OldIndex, moved.OldIndexItem, moved.NewIndexItem
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
}