using Get.Data.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace Get.Data.Bundles;
public interface IContentBundle<TOut>
{
    IReadOnlyProperty<TOut?> OutputContent { get; }
}