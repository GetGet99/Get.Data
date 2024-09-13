using Get.Data.Bindings;

namespace Get.Data.Properties;

/// <summary>
/// Represents the property that is updated asyncronously.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IAsyncWriteProperty<T> : IReadOnlyProperty<T>, IAsyncNotifyBinding<T>
{
    public Task SetValueAsync(T value);
}
