using Get.Data.Collections.Update;
using Get.Data.Properties;

namespace Get.Data.Bindings.Linq;
partial class Extension
{
    public static IBinding<TDest> Select<TSrc, TDest>(this IReadOnlyBinding<TSrc> src, IPropertyDefinition<TSrc, TDest> pDef)
        => new SelectPropertyPath<TSrc, TDest>(src, pDef);
}
class SelectPropertyPath<TOwner, TOut>(IReadOnlyBinding<TOwner> bindingOwner, IPropertyDefinition<TOwner, TOut> pDef) :
    SelectPropertyPathBase<TOwner, TOut, IProperty<TOut>, IPropertyDefinition<TOwner, TOut>>(bindingOwner, pDef), IBinding<TOut>
{
    public TOut CurrentValue { get => currentProperty.CurrentValue; set => currentProperty.CurrentValue = value; }
    protected override IProperty<TOut> GetProperty(IPropertyDefinition<TOwner, TOut> pdef, TOwner owner)
        => pdef.GetProperty(owner);
}