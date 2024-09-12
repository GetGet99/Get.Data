using Get.Data.Bindings;

namespace Get.Data.Properties;

public interface IProperty<T> : IBinding<T>, IReadOnlyProperty<T>
{
    void Bind(IBinding<T> binding, BindingModes bindingMode);
    void Bind(IReadOnlyBinding<T> binding, ReadOnlyBindingModes bindingMode);
}