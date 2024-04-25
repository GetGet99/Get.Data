using Get.Data.Test;
using Get.Data.XACL;
using Get.Data.Collections;
using Get.Data.Bindings.Linq;
using Get.Data.DataTemplates;
using Get.Data.Collections.Linq;

static class ProgramSample
{
    public static void Main()
    {
        //Test.UpdateTest(x => x.Select(x => x.ToString()), [1200195957], src => src[0] = 2099272109);
        TestUpdateCollection.RandomSimpleUpdateTest("Select", x => x.Select(x => x.ToString()));
        TestUpdateCollection.RandomSimpleUpdateTest("WithIndex", x => x.WithIndex());
        TestUpdateCollection.RandomSimpleUpdateTest("Reverse", x => x.Reverse());


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

        people.RemoveAt(1);
        people.Insert(1, new Person { Name = "Person 4", Age = 23 });
        people.RemoveAt(1);
        people.Insert(1, new Person { Name = "Person 4", Age = 23 });
        people.RemoveAt(1);
        people.Insert(1, new Person { Name = "Person 4", Age = 23 });

        Console.WriteLine("After Changing Value");

        PrintVisualTree(rootStackPanel);



        static void PrintVisualTree(object o)
        {
            Console.WriteLine(o);
        }
    }
}