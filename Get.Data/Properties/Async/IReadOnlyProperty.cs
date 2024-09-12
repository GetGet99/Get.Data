using Get.Data.Bindings;

namespace Get.Data.Properties;

public interface IAsyncProperty<T> : IReadOnlyProperty<T>
{
    public Task Set(T value);
    //void Bind(IBinding<T> binding, BindingModes bindingMode);
    //void Bind(IReadOnlyBinding<T> binding, ReadOnlyBindingModes bindingMode);
}
