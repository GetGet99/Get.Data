using Get.Data.Collections.Update;
using Get.Data.Properties;

namespace Get.Data.Bindings.Linq;
partial class Extension
{
    public static IReadOnlyBinding<TDest> Select<TSrc, TDest>(this IReadOnlyBinding<TSrc> src, IReadOnlyPropertyDefinition<TSrc, TDest> pDef)
        => new SelectPropertyPathReadOnly<TSrc, TDest>(src, pDef);
}
class SelectPropertyPathReadOnly<TOwner, TOut>(IReadOnlyBinding<TOwner> bindingOwner, IReadOnlyPropertyDefinition<TOwner, TOut> pDef) :
    SelectPropertyPathBase<TOwner, TOut, IReadOnlyProperty<TOut>, IReadOnlyPropertyDefinition<TOwner, TOut>>(bindingOwner, pDef), IReadOnlyBinding<TOut>
{
    public TOut CurrentValue { get => currentProperty.CurrentValue; }

    protected override IReadOnlyProperty<TOut> GetProperty(IReadOnlyPropertyDefinition<TOwner, TOut> pdef, TOwner owner)
        => pdef.GetProperty(owner);
}
partial struct ReadOnlyBindingsHelper<TSrc>
{
    public IReadOnlyBinding<TDest> Select<TDest>(IReadOnlyPropertyDefinition<TSrc, TDest> pDef)
        => binding.Select(pDef);
}