using Get.Data.Properties;

namespace Get.Data.Test;
[AutoProperty]
partial class TestSourceGen
{
    [IgnoreProperty]
    public IProperty<int> TestIntegerProperty { get; } = new Property<int>(10);
    [PropertyNameOverride("BooleanLOL")]
    public Property<bool> TestBooleanProperty { get; } = new Property<bool>(true);
    public IProperty<bool?> TestBooleanNullableProperty { get; } = new Property<bool?>(null);
    public IReadOnlyProperty<bool> TestReadOnlyProperty { get; } = new ReadOnlyProperty<bool>(true);
    public IProperty<FileInfo?> TestFileProperty { get; } = new Property<FileInfo?>(null);
    [PropertySetEquivalentCheck(PropertySetEquivalentCheckKinds.DefaultEqualityComparer)]
    public IProperty<FileInfo?> TestFileEqualityComparerProperty { get; } = new Property<FileInfo?>(null);
    public IProperty<FileInfo?> TestFileEquality2ComparerProperty { get; } = new Property<FileInfo?>(null);
    void Test()
    {
        
    }
}