using Get.Data.Collections;
using Get.Data.Collections.Linq;
using Get.Data.Properties;
using System.Collections;
using System.Diagnostics;
namespace Get.Data.Test;

[DebuggerDisplay("Name = {Name} Age = {Age}")]
partial class Person
{
    public Property<string> NameProperty { get; } = new("Unnamed");
    public Property<int> AgeProperty { get; } = new(0);
}
abstract class UIElement { }
partial class TextBlock : UIElement
{
    public Property<string> TextProperty { get; } = new("");
    public override string ToString()
        => $"TextBlock - Text = {Text}";
}
partial class StackPanel : UIElement
{
    public OnlyOneOfAKindAllowedList<UIElement> Children { get; } = new();
    public override string ToString()
        => "StackPanel\n" + string.Join('\n',
            ((IEnumerable<UIElement>)Children).Select(x => $"    {x}".Replace("\n", "\n    ")));
}
partial class Person
{
    public static PropertyDefinition<Person, string> NamePropertyDefinition { get; } = new(x => x.NameProperty);
    public static PropertyDefinition<Person, int> AgePropertyDefinition { get; } = new(x => x.AgeProperty);
    public int Age { get => AgeProperty.Value; set => AgeProperty.Value = value; }
    public string Name { get => NameProperty.Value; set => NameProperty.Value = value; }
}
partial class TextBlock
{
    public static PropertyDefinition<TextBlock, string> TextPropertyDefinition { get; } = new(x => x.TextProperty);
    public string Text { get => TextProperty.Value; set => TextProperty.Value = value; }
}

class OnlyOneOfAKindAllowedList<T> : IGDCollection<T>, IEnumerable<T>
{
    
    readonly List<T> l = [];
    public T this[int index] { get => l[index]; set
        {
            if (((IGDCollection<T>)this).Contains(value))
                throw new ArgumentException(null, nameof(value));
            l[index] = value;
        }
    }

    public int Count => l.Count;

    public IEnumerator<T> GetEnumerator() => ((IGDCollection<T>)this).AsEnumerable().GetEnumerator();
    public void Insert(int index, T item)
    {
        if (((IGDCollection<T>)this).Contains(item))
            throw new ArgumentException(null, nameof(item));
        l.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        l.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}