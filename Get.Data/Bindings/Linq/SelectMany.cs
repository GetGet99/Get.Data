namespace Get.Data.Bindings.Linq;
partial class Extension
{
    public static IReadOnlyBinding<TOut> SelectMany<TIn1, TIn2, TOut>(this IBinding<TIn1> inBinding1, Func<TIn1, IBinding<TIn2>> inBinding2Selector, ZipForwardConverter<TIn1, TIn2, TOut> converter)
        => inBinding1.Zip(inBinding1.SelectPath(inBinding2Selector), converter);
    public static IReadOnlyBinding<TOut> SelectMany<TIn1, TIn2, TOut>(this IReadOnlyBinding<TIn1> inBinding1, Func<TIn1, IReadOnlyBinding<TIn2>> inBinding2Selector, ZipForwardConverter<TIn1, TIn2, TOut> converter)
        => inBinding1.Zip(inBinding1.SelectPath(x => inBinding2Selector(x)), converter);
}
partial struct BindingsHelper<TSrc>
{
    public IReadOnlyBinding<TOut> SelectMany<TIn2, TOut>(Func<TSrc, IBinding<TIn2>> inBinding2Selector, ZipForwardConverter<TSrc, TIn2, TOut> converter)
        => binding.SelectMany(inBinding2Selector, converter);
}