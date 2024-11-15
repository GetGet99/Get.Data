using Get.Data.Collections.Update;
using Get.Data.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Get.Data.UIModels;

public class SelectionManagerMutable<T> : SelectionManager<T>
{
    public SelectionManagerMutable() : base() {
        Collection = (IUpdateCollection<T>)base.Collection;
    }
    public SelectionManagerMutable(IUpdateCollection<T> updateCollection) : base(updateCollection) {
        Collection = updateCollection;
    }
    public new IUpdateCollection<T> Collection { get; }
}
