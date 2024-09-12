using Get.Data.Properties;
namespace Get.Data.Bindings;
public delegate void ValueChangingHandler<T>(T oldValue, T newValue);
public delegate void ValueChangedHandler<T>(T oldValue, T newValue);
public interface INotifyBinding<T>
{
    event Action RootChanged;
    event ValueChangingHandler<T>? ValueChanging;
    event ValueChangedHandler<T>? ValueChanged;
}