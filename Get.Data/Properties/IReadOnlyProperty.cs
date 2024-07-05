using Get.Data.Bindings;

namespace Get.Data.Properties;

public interface IReadOnlyProperty<T> : IReadOnlyBinding<T>, INotifyBinding<T>
{
    void BindOneWayToSource(IBinding<T> binding);
    void RemoveBinding();
}
