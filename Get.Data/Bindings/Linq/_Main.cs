namespace Get.Data.Bindings.Linq;
public static partial class Extension;
public readonly partial struct ReadOnlyBindingsHelper<TSrc>(IReadOnlyBinding<TSrc> binding);
public readonly partial struct BindingsHelper<TSrc>(IBinding<TSrc> binding)
{
    public ReadOnlyBindingsHelper<TSrc> ReadOnly => new(binding);
}