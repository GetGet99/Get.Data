using Get.Data.Bindings;
using Get.Data.Bindings.Linq;
using Get.Data.Collections;
using Get.Data.Collections.Update;
using Get.Data.Properties;

namespace Get.Data.UIModels;
[AutoProperty]
public partial class SelectionManager<T>
{
    public IProperty<int> SelectedIndexProperty { get; } = Auto(-1);
    public IReadOnlyProperty<T?> SelectedValueProperty { get; } // TODO: Make it not readonly once Wrapper is done
    readonly Property<T?> _SelectedValueProperty;
    public IProperty<bool> PreferAlwaysSelectItemProperty { get; } = Auto(false);
    public IUpdateReadOnlyCollection<T> Collection { get; }
    public SelectionManager() : this(new UpdateCollection<T>()) { }
    public SelectionManager(IUpdateReadOnlyCollection<T> updateCollection)
    {
        Collection = updateCollection;
        
        SelectedIndexProperty.ValueChanged += SelectedIndexProperty_ValueChanged;
        Collection.ItemsChanged += ItemsSourceProperty_ItemsChanged;
        PreferAlwaysSelectItemProperty.ValueChanged += (old, @new) =>
        {
            if (@new && SelectedIndex < 0)
            {
                // let's trigger SelectedIndex auto selection logic
                SelectedIndexProperty_ValueChanged(SelectedIndex, SelectedIndex);
            }
        };
        SelectedValueProperty = new Wrapper(_SelectedValueProperty = new(default), this);
    }

    private void ItemsSourceProperty_ItemsChanged(IEnumerable<IUpdateAction<T>> actions)
    {
        var selectedIdx = SelectedIndex;
        foreach (var action in actions)
        {

            switch (action)
            {
                case ItemsAddedUpdateAction<T> added:
                    if (added.StartingIndex <= selectedIdx)
                        selectedIdx += added.Items.Count;
                    break;
                case ItemsRemovedUpdateAction<T> removed:
                    if (removed.StartingIndex <= SelectedIndex)
                    {
                        if (selectedIdx >= removed.StartingIndex + removed.Items.Count)
                            selectedIdx -= removed.Items.Count;
                        else
                        {
                            // the item that we selected got removed
                            selectedIdx = -1;
                            goto End;
                        }
                    }
                    break;
                case ItemsMovedUpdateAction<T> moved:
                    if (selectedIdx == moved.OldIndex) selectedIdx = moved.NewIndex;
                    else if (selectedIdx == moved.NewIndex) selectedIdx = moved.OldIndex;
                    break;
                case ItemsReplacedUpdateAction<T> replaced:
                    if (selectedIdx == replaced.Index)
                    {
                        selectedIdx = -1;
                        goto End;
                    }
                    break;
            }
        }
    End:
        SelectedIndex = selectedIdx;
    }

    private void SelectedIndexProperty_ValueChanged(int oldValue, int newValue)
    {
        if (newValue >= 0)
            _SelectedValueProperty.Value = Collection[newValue];
        else
            _SelectedValueProperty.Value = default;

        if (!PausePreferAlwaysSelectItemProperty && newValue is < 0 && PreferAlwaysSelectItem && Collection.Count > 0)
        {
            static int Clamp(int val, int min, int max) => val > min ? (val < max ? val : max) : min;
            var guessNewIndex = Clamp(oldValue - 1, 0, Collection.Count - 1);
            SelectedIndex = guessNewIndex;
        }
    }
    bool _PausePreferAlwaysSelectItemProperty;
    public bool PausePreferAlwaysSelectItemProperty
    {
        get => _PausePreferAlwaysSelectItemProperty;
        set
        {
            _PausePreferAlwaysSelectItemProperty = value;
            if (value == false)
            {
                if (SelectedIndex is < 0 && PreferAlwaysSelectItem && Collection.Count > 0)
                {
                    SelectedIndex = 0;
                }
            }
        }
    }
    readonly struct Wrapper(Property<T?> SelectedValueProperty, SelectionManager<T> self) : IProperty<T?>
    {
        public T? CurrentValue {
            get => SelectedValueProperty.Value;
            set => self.SelectedIndex = self.Collection.IndexOf(value!);
        }

        
        public event Action RootChanged { add { } remove { } }
        public event ValueChangingHandler<T?>? ValueChanging {
            add => SelectedValueProperty.ValueChanging += value;
            remove => SelectedValueProperty.ValueChanging -= value;
        }
        public event ValueChangedHandler<T?>? ValueChanged
        {
            add => SelectedValueProperty.ValueChanged += value;
            remove => SelectedValueProperty.ValueChanged -= value;
        }

        public void Bind(IBinding<T?> binding, BindingModes bindingMode)
        {
            var @this = self;
            self.SelectedIndexProperty.Bind(binding.Select(
                val => @this.Collection.IndexOf(val!),
                idx => idx < 0 ? default : @this.Collection[idx]
            ), bindingMode);
        }

        public void Bind(IReadOnlyBinding<T?> binding, ReadOnlyBindingModes bindingMode)
        {
            var @this = self;
            self.SelectedIndexProperty.Bind(from val in binding select @this.Collection.IndexOf(val), bindingMode);
        }

        public void BindOneWayToSource(IBinding<T?> binding)
            => SelectedValueProperty.BindOneWayToSource(binding);

        public void RemoveBinding()
        {
            SelectedValueProperty.RemoveBinding();
            // TODO: Update this!
        }
    }
}
