using Get.Data.Properties;
namespace Get.Data.Bundles;
public class ConstantContentBundle<T>(T? ele) : IContentBundle<T>
{
    public IReadOnlyProperty<T?> OutputContent { get; } = new ReadOnlyProperty<T?>(ele);
}