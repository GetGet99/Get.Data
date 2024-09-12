namespace Get.Data.Bindings.Linq;
partial class Extension
{
    public static IBinding<TDest> SelectPath<TSrc, TDest>(this IReadOnlyBinding<TSrc> src, Func<TSrc, IBinding<TDest>> selector)
        => Linq.SelectPath<TSrc, TDest>.Create(src, selector);
    public static IReadOnlyBinding<TDest> SelectPath<TSrc, TDest>(this IReadOnlyBinding<TSrc> src, SelectPathDelegate<TSrc, TDest> selector)
        => Linq.SelectPath<TSrc, TDest>.Create(src, selector);
}
partial struct ReadOnlyBindingsHelper<TSrc>
{
    public IBinding<TDest> SelectPath<TDest>(Func<TSrc, IBinding<TDest>> selector)
        => binding.SelectPath(selector);
}
partial struct ReadOnlyBindingsHelper<TSrc>
{
    public IReadOnlyBinding<TDest> SelectPath<TDest>(SelectPathDelegate<TSrc, TDest> selector)
        => binding.SelectPath(selector);
}