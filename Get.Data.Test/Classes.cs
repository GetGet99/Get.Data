using Get.Data.Properties;
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
    public List<UIElement> Children { get; } = [];
    public override string ToString()
        => "StackPanel\n" + string.Join('\n', Children.Select(x => $"    {x}".Replace("\n", "\n    ")));
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