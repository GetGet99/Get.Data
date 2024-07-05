using Get.Data.Collections.Implementation;
using Get.Data.Collections.Update;

namespace Get.Data.Bundles;

public interface IUpdateItemsBundleOutputCollection<T> :
    IUpdateReadOnlyCollection<T>, IRemoveImplGDCollection<T>, IRemoveAtImplGDCollection, IMoveImplGDCollection
{

}