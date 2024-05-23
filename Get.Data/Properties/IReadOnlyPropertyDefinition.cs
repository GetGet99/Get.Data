using Get.Data.Bindings;

namespace Get.Data.Properties;

public interface IReadOnlyPropertyDefinition<TOwner, TProperty>
{
    IReadOnlyProperty<TProperty> GetProperty(TOwner owner);
}
