using Get.Data.Properties;
using static Get.Data.Properties.AutoTyper;
namespace Get.Data.Bundles;
public class ConstantContentBundle<T>(T? ele) : IContentBundle<T>
{
    public IReadOnlyProperty<T?> OutputContent { get; } = AutoReadOnly(ele);
}