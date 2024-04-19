using Get.Data.Collections.Update;
using System.Collections.ObjectModel;
using Get.Data.Test;
using Get.Data.XACL;
using Get.Data.Collections;
using Get.Data.Bindings;
using Get.Data.Bindings.Linq;

UpdateCollectionInitializer<Person> people = [
    new() { Age = 18, Name = "Person 1" },
    new() { Age = 19, Name = "Person 2" },
    new() { Age = 20, Name = "Person 3" }
];

DataTemplate<Person, UIElement> UICreatorDataTemplate = new(
    root =>
        new StackPanel()
        {
            Children =
            {
                new TextBlock()
                .WithOneWayBinding(
                    TextBlock.TextPropertyDefinition,
                    root.Select(Person.NamePropertyDefinition)
                ),
                new TextBlock()
                .WithOneWayBinding(
                    TextBlock.TextPropertyDefinition,
                    root.Select(Person.AgePropertyDefinition).Select(x => x.ToString())
                )
            }
        }
);

StackPanel rootStackPanel = new()
{
    Children = {
        new CollectionItemsBinding<Person, UIElement>(people, UICreatorDataTemplate)
    }
};

PrintVisualTree(rootStackPanel);

people[1].Name = "random name";

Console.WriteLine("After Changing Value");

PrintVisualTree(rootStackPanel);



static void PrintVisualTree(object o)
{
    Console.WriteLine(o);
}