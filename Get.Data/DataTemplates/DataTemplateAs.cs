using Get.Data.Bindings;
using Get.Data.DataTemplates;

namespace Get.Data.DataTemplates;
public static class Extensions
{
    public static IDataTemplate<TIn, TNewOut> As<TIn, TOldOut, TNewOut>(this IDataTemplate<TIn, TOldOut> dataTemplate, Func<TOldOut, TNewOut> converter)
        => new DataTemplateAs<TIn, TOldOut, TNewOut>(dataTemplate, converter);
    public static IDataTemplate<TIn, TNewOut> As<TIn, TOldOut, TNewOut>(this IDataTemplate<TIn, TOldOut> dataTemplate)
        where TOldOut : TNewOut
        => new DataTemplateAs<TIn, TOldOut, TNewOut>(dataTemplate, x => x);
    public static DataTemplateHelper<TIn, TOut> GetHelper<TIn, TOut>(this IDataTemplate<TIn, TOut> dataTemplate)
        => new(dataTemplate);
    readonly struct DataTemplateAs<TIn, TOldOut, TNewOut>(IDataTemplate<TIn, TOldOut> input, Func<TOldOut, TNewOut> converter) : IDataTemplate<TIn, TNewOut>
    {
        IDataTemplate<TIn, TOldOut> Input { get; } = input;
        public IDataTemplateGeneratedValue<TIn, TNewOut> Generate(IReadOnlyBinding<TIn> source)
        {
            return new Wrapper(this, Input.Generate(source), converter);
        }

        public void NotifyRecycle(IDataTemplateGeneratedValue<TIn, TNewOut> recycledItem)
        {
            Input.NotifyRecycle(((Wrapper)recycledItem).Old);
        }
        readonly struct Wrapper(IDataTemplate<TIn, TNewOut> parent, IDataTemplateGeneratedValue<TIn, TOldOut> old, Func<TOldOut, TNewOut> converter) : IDataTemplateGeneratedValue<TIn, TNewOut>
        {
            public IDataTemplateGeneratedValue<TIn, TOldOut> Old { get; } = old;
            public IReadOnlyBinding<TIn> Binding { get => Old.Binding; set => Old.Binding = value; }

            public IDataTemplate<TIn, TNewOut> Template => parent;

            public TNewOut GeneratedValue => converter(old.GeneratedValue);

            public void Recycle() => Old.Recycle();
        }
    }
}

public readonly struct DataTemplateHelper<TIn, TOut>(IDataTemplate<TIn, TOut> oldTemplate)
{
    public IDataTemplate<TIn, TNewOut> As<TNewOut>(Func<TOut, TNewOut> converter)
        => oldTemplate.As(converter);
}