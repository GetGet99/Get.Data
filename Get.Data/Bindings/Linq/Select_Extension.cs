using Get.Data.Helpers;

namespace Get.Data.Bindings.Linq;
// Normal
partial class Extension
{
    public static IBinding<TDest> Select<TSrc, TDest>(this IBinding<TSrc> src, ForwardConverter<TSrc, TDest> forwardConverter, BackwardConverter<TSrc, TDest> backwardConverter)
        => Linq.Select<TSrc, TDest>.Create(src, forwardConverter, backwardConverter);
    public static IBinding<TDest> Select<TSrc, TDest>(this IBinding<TSrc> src, ForwardConverter<TSrc, TDest> forwardConverter, AdvancedBackwardConverter<TSrc, TDest> backwardConverter)
        => Linq.Select<TSrc, TDest>.Create(src, forwardConverter, backwardConverter);
    public static IReadOnlyBinding<TDest> Select<TSrc, TDest>(this IReadOnlyBinding<TSrc> src, ForwardConverter<TSrc, TDest> forwardConverter)
        => Linq.Select<TSrc, TDest>.Create(src, forwardConverter);
}
partial struct BindingsHelper<TSrc>
{
    public IBinding<TDest> Select<TDest>(ForwardConverter<TSrc, TDest> forwardConverter, BackwardConverter<TSrc, TDest> backwardConverter)
        => binding.Select(forwardConverter, backwardConverter);
    public IBinding<TDest> Select<TDest>(ForwardConverter<TSrc, TDest> forwardConverter, AdvancedBackwardConverter<TSrc, TDest> backwardConverter)
        => binding.Select(forwardConverter, backwardConverter);
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