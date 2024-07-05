using Get.Data.Collections.Implementation;

namespace Get.Data.Collections.Update;
public interface IUpdateCollection<T> : IUpdateFixedSizeCollection<T>, IGDCollection<T>, ICollectionUpdateEvent<T>, IClearImplGDCollection, IMoveImplGDCollection
{
    
}
public interface IUpdateFixedSizeCollection<T> : IUpdateReadOnlyCollection<T>, IGDFixedSizeCollection<T>, ICollectionUpdateEvent<T>
{
}
public interface IUpdateReadOnlyCollection<T> : IGDReadOnlyCollection<T>, ICollectionUpdateEvent<T>
{
}