namespace Get.Data.Bindings.Linq;
partial class Extension
{
    public static IBinding<TDest> SelectPath<TSrc, TDest>(this IReadOnlyBinding<TSrc> src, Func<TSrc, IBinding<TDest>> selector)
        => new SelectPath<TSrc, TDest>(src, selector);
}
class SelectPath<TSrc, TDest>(IReadOnlyBinding<TSrc> src, Func<TSrc, IBinding<TDest>> selector) :
    SelectPathBase<TSrc, TDest, IBinding<TDest>>(src, selector), IBinding<TDest>
{
    public TDest CurrentValue {
        get => currentBinding is null ? default! : currentBinding.CurrentValue;
        set => (currentBinding ?? throw new NullReferenceException()).CurrentValue = value; }
}
partial struct ReadOnlyBindingsHelper<TSrc>
{
    public IBinding<TDest> SelectPath<TDest>(Func<TSrc, IBinding<TDest>> selector)
        => binding.SelectPath(selector);
}