# Get.Data

Get.Data is .NET 8 and .NET Standard 2.0 library that can do data bindings, update collections, and properties in C# without any XAML or markup language.

Below is the draft of what this library is about. It is not 100% ready yet but I will just put it up for now.

## Motivation

Right now we have seen things like bindings in WPF/WinUI that allows us to define the data flow from the backend code to the UI. But what if we can also bring that data flow into code with code binding or use it in any other UI framework without similar features?

## Collections
### Update Collection
What makes this unique is that this library allows collection-level data flow.

We provide "Update Collection" which is basically `ObservableCollection` but it gives higher back end flexibility. WIth the help of `Get.Data.Collections.Linq`, we can easily create collection-level data flow.

```csharp
UpdateCollectionInitializer<int> Xs = [ 1, 2, 3, 4 ];
UpdateCollectionInitializer<int> Ys = [ 4, 3, 2, 1 ];

IReadOnlyUpdateCollection<double> distanceFromOrigin = Xs.Zip(Ys, (x, y) => Math.Sqrt(x * x + y * y));

IReadOnlyUpdateCollection<string> filteredMessage = distanceFromOrigin.Where(d => d > 2).Select(d => $"Hello, I'm {d} units away from the origin!");

filteredMessage.ItemsChanged += (actions) => ...;
```

Most of the extension method do not cache the value, with the only exception would be the one that has to be cached (Currently `Where` cached the valid indices). This means that there are usually no duplicated data and they are lazily evaluated. However, even though it is lazily evaluated, `ItemsChanged` will be sending out all the changes like ObservableCollection does.

### Other Linq-like Collection Methods
We introduced `IGDReadOnlyCollection`, `IGDFixedSizeCollection`, and `IGDCollection` as an alternative to `IList` and `IReadOnlyList` so that the impelmentations are easier. (because this is an insane fact that not every `IList` is `IReadOnlyList`... wow...)

`Get.Data.Collections.Linq` for non-update collection provide a new view of collection. Modifying the (non-readonly) output collection will edit the original collection, and any new items in original collection will be shown in the resulting collection.
```csharp
IGDReadOnlyCollection<TOut> Select<TIn, TOut>(this IGDReadOnlyCollection<TIn> c, ForwardConverter<TIn, TOut> f);
IGDCollection<TOut> Select<TIn, TOut>(this IGDCollection<TIn> c, ForwardConverter<TIn, TOut> f, BackwardConverter<TIn, TOut> b);
IGDCollection<T> Reverse<T>(this IGDCollection<T> c);
// record struct IndexItem<T>(int Index, T Value);
IGDReadOnlyCollection<T> Span<T>(this IGDReadOnlyCollection<T> c, Range range);
IGDCollection<IndexItem<T>> WithIndex<T>(this IGDCollection<T> c);
```

## Properties
`Property<T>`: Just act like a field that has its own value changed events, it can be bind using bindings
`PropertyDefinition(Base)<TOwnerType, TPropertyType>`: acts like WinUI/WPF's `DependencyProperty` but strongly typed.

## Bindings
Bindings can be created from a property, and it can be manipulated using Linq-like extension. It is strongly typed.

```csharp
Property<int> SampleIntProperty = new(1);
Property<float> SampleFloatProperty = new(2f);

// one way binding
IReadOnlyBinding<string> strReadOnlyBinding = SampleIntProperty .Select(x => x.ToString());

// two way binding
IBinding<string> strBinding = SampleIntProperty .Select(x => x.ToString(), str => int.Parse(str));

// more metohds
SampleIntProperty.Zip(SampleFloatProperty, (x, y) => x + y) // one way

// bind to output property
Property<float> target = new(default);

target.Bind(SampleIntProperty.Zip(SampleFloatProperty, (x, y) => x + y), ReadOnlyBindingModes.OneWay);

Console.WriteLine(target.Value); // 2
SampleFloatProperty.Value = 3;
Console.WriteLine(target.Value); // 4
```
