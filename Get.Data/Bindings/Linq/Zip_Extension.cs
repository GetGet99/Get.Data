namespace Get.Data.Bindings.Linq;
partial class Extension
{
    public static IBinding<TOut> Zip<TIn1, TIn2, TOut>(this IBinding<TIn1> inBinding1, IBinding<TIn2> inBinding2, ZipForwardConverter<TIn1, TIn2, TOut> converter, ZipAdvancedBackwardConverter<TIn1, TIn2, TOut> backwardConverter)
        => Linq.Zip<TIn1, TIn2, TOut>.Create(inBinding1, inBinding2, converter, backwardConverter);
    public static IReadOnlyBinding<TOut> Zip<TIn1, TIn2, TOut>(this IReadOnlyBinding<TIn1> inBinding1, IReadOnlyBinding<TIn2> inBinding2, ZipForwardConverter<TIn1, TIn2, TOut> converter)
        => Linq.Zip<TIn1, TIn2, TOut>.Create(inBinding1, inBinding2, converter);
    public static IBinding<TOut> Zip<TIn1, TIn2, TOut>(this IBinding<TIn1> inBinding1, IReadOnlyBinding<TIn2> inBinding2, ZipForwardConverter<TIn1, TIn2, TOut> converter, ZipPartialAdvancedBackwardConverter<TIn1, TIn2, TOut> backwardConverter)
        => Linq.Zip<TIn1, TIn2, TOut>.Create(inBinding1, inBinding2, converter, backwardConverter);
}
partial struct BindingsHelper<TSrc>
{
    public IReadOnlyBinding<TOut> Zip<TIn2, TOut>(IBinding<TIn2> inBinding2, ZipForwardConverter<TSrc, TIn2, TOut> converter, ZipAdvancedBackwardConverter<TSrc, TIn2, TOut> backwardConverter)
        => binding.Zip(inBinding2, converter, backwardConverter);
}