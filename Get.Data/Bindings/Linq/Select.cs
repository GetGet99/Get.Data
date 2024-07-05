namespace Get.Data.Bindings.Linq;
partial class Extension
{
    public static IBinding<TDest> Select<TSrc, TDest>(this IBinding<TSrc> src, ForwardConverter<TSrc, TDest> forwardConverter, BackwardConverter<TSrc, TDest> backwardConverter)
        => new Select<TSrc, TDest>(src, forwardConverter, (x, _) => backwardConverter(x));
    public static IBinding<TDest> Select<TSrc, TDest>(this IBinding<TSrc> src, ForwardConverter<TSrc, TDest> forwardConverter, AdvancedBackwardConverter<TSrc, TDest> backwardConverter)
        => new Select<TSrc, TDest>(src, forwardConverter, backwardConverter);
}
class Select<TIn, TOut>(IBinding<TIn> inBinding, ForwardConverter<TIn, TOut> converter, AdvancedBackwardConverter<TIn, TOut> backwardConverter) : SelectBase<TIn, TOut>(inBinding, converter), IBinding<TOut>
{
    public TOut CurrentValue
    {
        get => _value;
        set => inBinding.CurrentValue = backwardConverter(value, owner);
    }
}
partial struct BindingsHelper<TSrc>
{
    public IBinding<TDest> Select<TDest>(ForwardConverter<TSrc, TDest> forwardConverter, BackwardConverter<TSrc, TDest> backwardConverter)
        => binding.Select(forwardConverter, backwardConverter);
    public IBinding<TDest> Select<TDest>(ForwardConverter<TSrc, TDest> forwardConverter, AdvancedBackwardConverter<TSrc, TDest> backwardConverter)
        => binding.Select(forwardConverter, backwardConverter);
}