namespace Get.Data.Bundles;


public interface IItemsBundle<TOut> : IReadOnlyItemsBundle<TOut>
{
    new IUpdateItemsBundleOutputCollection<TOut> OutputContent { get; }
}