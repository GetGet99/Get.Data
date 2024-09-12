using System;
using System.Collections.Generic;
using System.Text;

namespace Get.Data.Bindings;
public delegate Task AsyncValueChangingHandler<T>(T oldValue, T newValue);
public delegate Task AsyncValueChangedHandler<T>(T oldValue, T newValue);
public interface IAsyncNotifyBinding<T>
{
    event Func<Task> RootChanged;
    event AsyncValueChangingHandler<T>? ValueChanging;
    event AsyncValueChangedHandler<T>? ValueChanged;
}
