namespace Get.Data.Bindings.Linq;
partial class Extension
{
    public static IReadOnlyBinding<TOut> SelectMany<TIn1, TIn2, TOut>(this IBinding<TIn1> inBinding1, Func<TIn1, IBinding<TIn2>> inBinding2Selector, ZipForwardConverter<TIn1, TIn2, TOut> converter)
        => inBinding1.Zip(inBinding1.SelectPath(inBinding2Selector), converter);
}