using Get.Data.Properties;

namespace Get.Data.Bindings.Linq;
partial class Extension
{
    public static IBinding<TDest> Select<TSrc, TDest>(this IReadOnlyBinding<TSrc> src, IPropertyDefinition<TSrc, TDest> pDef)
        => SelectPropertyPath<TSrc, TDest>.Create(src, pDef);
    public static IReadOnlyBinding<TDest> Select<TSrc, TDest>(this IReadOnlyBinding<TSrc> src, IReadOnlyPropertyDefinition<TSrc, TDest> pDef)
        => SelectPropertyPath<TSrc, TDest>.Create(src, pDef);
}
partial struct ReadOnlyBindingsHelper<TSrc>
{
    public IBinding<TDest> Select<TDest>(IPropertyDefinition<TSrc, TDest> pDef)
        => binding.Select(pDef);
}
partial struct ReadOnlyBindingsHelper<TSrc>
{
    public IReadOnlyBinding<TDest> Select<TDest>(IReadOnlyPropertyDefinition<TSrc, TDest> pDef)
        => binding.Select(pDef);
}