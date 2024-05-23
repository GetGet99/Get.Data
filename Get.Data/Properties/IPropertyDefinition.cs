namespace Get.Data.Properties;

public interface IPropertyDefinition<TOwner, TProperty> : IReadOnlyPropertyDefinition<TOwner, TProperty>
{
    new IProperty<TProperty> GetProperty(TOwner owner);
}
