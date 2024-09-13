using Get.Data.Bindings;

namespace Get.Data.Properties;

public interface IAsyncProperty<T> : IReadOnlyProperty<T>, IAsyncNotifyBinding<T>
{
    public Task SetValueAsync(T value);
    //void Bind(IBinding<T> binding, BindingModes bindingMode);
    //void Bind(IReadOnlyBinding<T> binding, ReadOnlyBindingModes bindingMode);
}
