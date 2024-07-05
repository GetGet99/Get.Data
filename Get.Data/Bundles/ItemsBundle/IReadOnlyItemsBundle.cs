using Get.Data.Bindings;
using Get.Data.Bindings.Linq;
using Get.Data.Collections;
using Get.Data.Collections.Update;
using Get.Data.DataTemplates;
using Get.Data.Properties;

namespace Get.Data.Bundles;


public interface IReadOnlyItemsBundle<TOut>
{
    IUpdateReadOnlyCollection<TOut> OutputContent { get; }
}