namespace Get.Data.Bindings.Linq;
partial class Extension
{
    public static IReadOnlyBinding<TDest> Select<TSrc, TDest>(this IReadOnlyBinding<TSrc> src, ForwardConverter<TSrc, TDest> forwardConverter)
        => new SelectReadOnly<TSrc, TDest>(src, forwardConverter);
}
class SelectReadOnly<TIn, TOut>(IReadOnlyBinding<TIn> inBinding, ForwardConverter<TIn, TOut> converter) : SelectBase<TIn, TOut>(inBinding, converter), IReadOnlyBinding<TOut>
{
    public TOut CurrentValue => _value;
}
partial struct ReadOnlyBindingsHelper<TSrc>
{
    public IReadOnlyBinding<TDest> Select<TDest>(ForwardConverter<TSrc, TDest> forwardConverter)
        => binding.Select(forwardConverter);
}
partial struct BindingsHelper<TSrc>
{
    // Select is often used, so make it for binding as well
    public IReadOnlyBinding<TOut> Select<TOut>(ForwardConverter<TSrc, TOut> forwardConverter)
        => binding.Select(forwardConverter);
}