using MGUI.Core.UI.Brushes.Border_Brushes;
using MGUI.Core.UI.Brushes.Fill_Brushes;
using MGUI.Core.UI.Containers;
using MGUI.Core.UI.XAML;
using MGUI.Shared.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Thickness = MonoGame.Extended.Thickness;

namespace MGUI.Core.UI;

public enum ListBoxSelectionMode
{
    /// <summary>Items cannot be selected</summary>
    None,
    /// <summary>A single item may be selected at a time, by left-clicking it</summary>
    Single,
    /// <summary>A set of consecutive items may be selected at once. Left-click to replace selection with a single item.<br/>
    /// Shift+Left-click to select all consecutive items between the current selection source and the clicked item.</summary>
    Contiguous,
    /// <summary>Any number of items may be selected at once. Left-click to replace selection with a single item.<br/>
    /// Ctrl+Left-click to toggle the selection state of the clicked item.</summary>
    Multiple
}

/// <typeparam name="TItemType">The type that the ItemsSource will be bound to.</typeparam>
public class MgListBox<TItemType> : MGElement
{
    #region Outer Border
    private MGComponent<MGBorder> OuterBorderComponent { get; }
    /// <summary><see cref="MgListBox{TItemType}"/>es contain 3 borders:<para/>
    /// 1. <see cref="OuterBorder"/>: Wrapped around the entire <see cref="MgListBox{TItemType}"/><br/>
    /// 2. <see cref="InnerBorder"/>: Wrapped around the <see cref="ItemsPanel"/>, but not the <see cref="TitleComponent"/><br/>
    /// 3. <see cref="TitleBorder"/>: Wrapped around the <see cref="TitleComponent"/></summary>
    public MGBorder OuterBorder { get; }
    public override MGBorder GetBorder() => OuterBorder;

    public IBorderBrush OuterBorderBrush
    {
        get => OuterBorder.BorderBrush;
        set
        {
            if (OuterBorderBrush != value)
            {
                OuterBorder.BorderBrush = value;
                Npc(nameof(OuterBorderBrush));
            }
        }
    }

    public Thickness OuterBorderThickness
    {
        get => OuterBorder.BorderThickness;
        set
        {
            if (!OuterBorderThickness.Equals(value))
            {
                OuterBorder.BorderThickness = value;
                Npc(nameof(OuterBorderThickness));
            }
        }
    }
    #endregion Outer Border

    #region Inner Border
    private MGComponent<MGBorder> InnerBorderComponent { get; }
    /// <summary><see cref="MgListBox{TItemType}"/>es contain 3 borders:<para/>
    /// 1. <see cref="OuterBorder"/>: Wrapped around the entire <see cref="MgListBox{TItemType}"/><br/>
    /// 2. <see cref="InnerBorder"/>: Wrapped around the <see cref="ItemsPanel"/>, but not the <see cref="TitleComponent"/><br/>
    /// 3. <see cref="TitleBorder"/>: Wrapped around the <see cref="TitleComponent"/></summary>
    public MGBorder InnerBorder { get; }

    public IBorderBrush InnerBorderBrush
    {
        get => InnerBorder.BorderBrush;
        set
        {
            if (InnerBorderBrush != value)
            {
                InnerBorder.BorderBrush = value;
                Npc(nameof(InnerBorderBrush));
            }
        }
    }

    public Thickness InnerBorderThickness
    {
        get => InnerBorder.BorderThickness;
        set
        {
            if (!InnerBorderThickness.Equals(value))
            {
                InnerBorder.BorderThickness = value;
                Npc(nameof(InnerBorderThickness));
            }
        }
    }
    #endregion Inner Border

    #region Title
    private MGComponent<MGBorder> TitleComponent { get; }
    /// <summary><see cref="MgListBox{TItemType}"/>es contain 3 borders:<para/>
    /// 1. <see cref="OuterBorder"/>: Wrapped around the entire <see cref="MgListBox{TItemType}"/><br/>
    /// 2. <see cref="InnerBorder"/>: Wrapped around the <see cref="ItemsPanel"/>, but not the <see cref="TitleComponent"/><br/>
    /// 3. <see cref="TitleBorder"/>: Wrapped around the <see cref="TitleComponent"/></summary>
    public MGBorder TitleBorder { get; }
    public MGContentPresenter TitlePresenter { get; }

    public IBorderBrush TitleBorderBrush
    {
        get => TitleBorder.BorderBrush;
        set
        {
            if (TitleBorder.BorderBrush != value)
            {
                TitleBorder.BorderBrush = value;
                Npc(nameof(TitleBorderBrush));
            }
        }
    }

    public Thickness TitleBorderThickness
    {
        get => TitleBorder.BorderThickness;
        set
        {
            if (!TitleBorder.BorderThickness.Equals(value))
            {
                TitleBorder.BorderThickness = value;
                Npc(nameof(TitleBorderThickness));
            }
        }
    }

    public bool IsTitleVisible
    {
        get => TitleBorder.Visibility == Visibility.Visible;
        set
        {
            if (IsTitleVisible != value)
            {
                TitleBorder.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                Npc(nameof(IsTitleVisible));
            }
        }
    }
    #endregion Title

    #region Items Source
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ObservableCollection<MgListBoxItem<TItemType>> _internalItems;
    private ObservableCollection<MgListBoxItem<TItemType>> InternalItems
    {
        get => _internalItems;
        set
        {
            if (_internalItems != value)
            {
                if (InternalItems != null)
                {
                    HandleTemplatedContentRemoved(InternalItems.Select(x => x.Content));
                    InternalItems.CollectionChanged -= ListBoxItems_CollectionChanged;
                }
                _internalItems = value;
                if (InternalItems != null)
                    InternalItems.CollectionChanged += ListBoxItems_CollectionChanged;

                using (ItemsPanel.AllowChangingContentTemporarily())
                {
                    //  Clear all ListBoxItems
                    _ = ItemsPanel.TryRemoveAll();

                    //  Add the new ListBoxItems to the ItemsPanel
                    if (InternalItems != null)
                    {
                        foreach (MgListBoxItem<TItemType> lbi in InternalItems)
                            _ = ItemsPanel.TryAddChild(lbi.ContentPresenter);
                    }
                }

                ClearSelection();
                RefreshRowBackgrounds();

                Npc(nameof(ListBoxItems));
            }
        }
    }
    public IReadOnlyList<MgListBoxItem<TItemType>> ListBoxItems => InternalItems;

    private static void HandleTemplatedContentRemoved(IEnumerable<MGElement> items)
    {
        if (items != null)
        {
            foreach (var item in items)
                item.RemoveDataBindings(true);
        }
    }

    private void ListBoxItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        using (ItemsPanel.AllowChangingContentTemporarily())
        {
            HashSet<MgListBoxItem<TItemType>> removed = new();

            if (e.Action is NotifyCollectionChangedAction.Reset)
            {
                _ = ItemsPanel.TryRemoveAll();
                ClearSelection();
            }
            else if (e.Action is NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                int index = e.NewStartingIndex;
                foreach (MgListBoxItem<TItemType> item in e.NewItems)
                {
                    ItemsPanel.TryInsertChild(index, item.ContentPresenter);
                    index++;
                }

                RefreshRowBackgrounds();
            }
            else if (e.Action is NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (MgListBoxItem<TItemType> item in e.OldItems)
                {
                    if (ItemsPanel.TryRemoveChild(item.ContentPresenter))
                    {
                        removed.Add(item);
                    }
                }

                RefreshRowBackgrounds();
            }
            else if (e.Action is NotifyCollectionChangedAction.Replace)
            {
                List<MgListBoxItem<TItemType>> old = e.OldItems.Cast<MgListBoxItem<TItemType>>().ToList();
                List<MgListBoxItem<TItemType>> @new = e.NewItems.Cast<MgListBoxItem<TItemType>>().ToList();
                for (int i = 0; i < old.Count; i++)
                {
                    if (ItemsPanel.TryReplaceChild(old[i].ContentPresenter, @new[i].ContentPresenter))
                    {
                        removed.Add(old[i]);

                        if (AlternatingRowBackgrounds?.Any() == true)
                            @new[i].ContentPresenter.BackgroundBrush.NormalValue = old[i].ContentPresenter.BackgroundBrush.NormalValue;
                    }
                }
            }
            else if (e.Action is NotifyCollectionChangedAction.Move)
            {
                throw new NotImplementedException();
            }

            //  Ensure none of the removed items are Selected
            if (SelectedItems != null)
            {
                List<MgListBoxItem<TItemType>> newSelectedItems = SelectedItems.Where(x => !removed.Contains(x)).ToList();
                if (newSelectedItems.Count != SelectedItems.Count || !newSelectedItems.SequenceEqual(SelectedItems))
                    SelectedItems = newSelectedItems.AsReadOnly();
            }
            if (SelectionSourceItem != null && removed.Contains(SelectionSourceItem))
                SelectionSourceItem = null;

            HandleTemplatedContentRemoved(removed.Select(x => x.Content));
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ObservableCollection<TItemType> _itemsSource;
    /// <summary>To set this value, use <see cref="SetItemsSource(ICollection{TItemType})"/></summary>
    public ObservableCollection<TItemType> ItemsSource
    {
        get => _itemsSource;
        private set
        {
            if (_itemsSource != value)
            {
                if (ItemsSource != null)
                    ItemsSource.CollectionChanged -= ItemsSource_CollectionChanged;
                _itemsSource = value;
                if (ItemsSource != null)
                    ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;

                if (ItemsSource == null)
                    InternalItems = null;
                else
                {
                    IEnumerable<MgListBoxItem<TItemType>> values = ItemsSource.Select((x, index) => new MgListBoxItem<TItemType>(this, x));
                    this.InternalItems = new ObservableCollection<MgListBoxItem<TItemType>>(values);
                }

                Npc(nameof(ItemsSource));
            }
        }
    }

    private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action is NotifyCollectionChangedAction.Reset)
        {
            HandleTemplatedContentRemoved(InternalItems.Select(x => x.Content));
            InternalItems.Clear();
        }
        else if (e.Action is NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            int currentIndex = e.NewStartingIndex;
            foreach (TItemType item in e.NewItems)
            {
                MgListBoxItem<TItemType> newRowItem = new(this, item);
                InternalItems.Insert(currentIndex, newRowItem);
                currentIndex++;
            }
        }
        else if (e.Action is NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            int currentIndex = e.OldStartingIndex;
            foreach (TItemType item in e.OldItems)
            {
                InternalItems.RemoveAt(currentIndex);
                currentIndex++;
            }
        }
        else if (e.Action is NotifyCollectionChangedAction.Replace)
        {
            List<TItemType> old = e.OldItems.Cast<TItemType>().ToList();
            List<TItemType> @new = e.NewItems.Cast<TItemType>().ToList();
            for (int i = 0; i < old.Count; i++)
            {
                MgListBoxItem<TItemType> oldRowItem = InternalItems[i];
                MgListBoxItem<TItemType> newRowItem = new(this, @new[i]);
                InternalItems[e.OldStartingIndex + i] = newRowItem;
            }
        }
        else if (e.Action is NotifyCollectionChangedAction.Move)
        {
            throw new NotImplementedException();
        }
    }

    /// <param name="value"><see cref="ItemsSource"/> will be set to a copy of this <see cref="ICollection{T}"/> unless the collection is an <see cref="ObservableCollection{T}"/>.<br/>
    /// If you want <see cref="ItemsSource"/> to dynamically update as the collection changes, pass in an <see cref="ObservableCollection{T}"/></param>
    public void SetItemsSource(ICollection<TItemType> value)
    {
        if (value is ObservableCollection<TItemType> observable)
            this.ItemsSource = observable;
        else
            this.ItemsSource = new ObservableCollection<TItemType>(value.ToList());
    }
    #endregion Items Source

    #region Selection
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool _canDeselectByClickingSelectedItem;
    /// <summary>Only relevant if <see cref="SelectionMode"/> is not <see cref="ListBoxSelectionMode.None"/><para/>
    /// If true, allows deselecting a currently-selected item by left-clicking it.<para/>
    /// Note: If <see cref="SelectionMode"/> is <see cref="ListBoxSelectionMode.Multiple"/>, and the Control key is held when clicking an item,<br/>
    /// the user is always able to deselect regardless of this setting.<para/>
    /// Default value: true</summary>
    public bool CanDeselectByClickingSelectedItem
    {
        get => _canDeselectByClickingSelectedItem;
        set
        {
            if (_canDeselectByClickingSelectedItem != value)
            {
                _canDeselectByClickingSelectedItem = value;
                Npc(nameof(CanDeselectByClickingSelectedItem));
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ListBoxSelectionMode _selectionMode;
    public ListBoxSelectionMode SelectionMode
    {
        get => _selectionMode;
        set
        {
            if (_selectionMode != value)
            {
                _selectionMode = value;
                ClearSelection();
                Npc(nameof(SelectionMode));
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private MgListBoxItem<TItemType> _selectionSourceItem;
    /// <summary>Only relevant if <see cref="SelectionMode"/> is <see cref="ListBoxSelectionMode.Contiguous"/>.<para/>
    /// Represents the starting item of the contiguous selection of items.</summary>
    public MgListBoxItem<TItemType> SelectionSourceItem
    {
        get => _selectionSourceItem;
        private set
        {
            if (_selectionSourceItem != value)
            {
                _selectionSourceItem = value;
                Npc(nameof(SelectionSourceItem));
            }
        }
    }

    private readonly EqualityComparer<TItemType> _equalityComparer = EqualityComparer<TItemType>.Default;

    /// <summary>Returns the <see cref="MgListBoxItem{TItemType}.Data"/> of the first item in <see cref="SelectedItems"/>, or default(<typeparamref name="TItemType"/>) if no items are selected.<para/>
    /// Setting this value overwrites the <see cref="SelectedItems"/> with the first <see cref="MgListBoxItem{TItemType}"/> containing the matching data.<br/>
    /// If no matching item is found, clears the selection entirely.</summary>
    public TItemType SelectedValue
    {
        get => SelectedItems.Count == 0 ? default(TItemType) : SelectedItems.First().Data;
        set => SelectItem(value, true);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ReadOnlyCollection<MgListBoxItem<TItemType>> _selectedItems;
    /// <summary>The currently-selected items. This collection is never null: Uses an empty list if setting to null.</summary>
    public ReadOnlyCollection<MgListBoxItem<TItemType>> SelectedItems
    {
        get => _selectedItems;
        set
        {
            if (_selectedItems != value)
            {
                if (SelectedItems != null)
                {
                    foreach (MgListBoxItem<TItemType> item in SelectedItems)
                        item.ContentPresenter.IsSelected = false;
                }

                _selectedItems = value ?? new List<MgListBoxItem<TItemType>>().AsReadOnly();

                if (SelectedItems.Any(x => x == null))
                    throw new ArgumentNullException($"{nameof(MgListBoxItem<object>)}.{nameof(SelectedItems)} cannnot contain null items.");

                foreach (MgListBoxItem<TItemType> item in SelectedItems)
                    item.ContentPresenter.IsSelected = true;

                Npc(nameof(SelectedItems));
                Npc(nameof(SelectedValue));
                SelectionChanged?.Invoke(this, SelectedItems);
            }
        }
    }

    public event EventHandler<ReadOnlyCollection<MgListBoxItem<TItemType>>> SelectionChanged;

    /// <summary>Note: This method clears the selection if <see cref="SelectionMode"/> is <see cref="ListBoxSelectionMode.None"/></summary>
    /// <param name="item">The item to select. Will search for a <see cref="MgListBoxItem{TItemType}"/> whose <see cref="MgListBoxItem{TItemType}.Data"/> matches this value.</param>
    /// <param name="deselectAllIfNotFound">If true, and if no corresponding <see cref="MgListBoxItem{TItemType}"/> is found that matches the given <paramref name="item"/>, the selection will be cleared.</param>
    public void SelectItem(TItemType item, bool deselectAllIfNotFound)
    {
        if (SelectionMode is ListBoxSelectionMode.None)
        {
            ClearSelection();
            return;
        }

        if (SelectedItems?.Count == 1 && _equalityComparer.Equals(SelectedItems.First().Data, item))
            return;

        //  Find ListBoxItem that wraps the Item data
        foreach (MgListBoxItem<TItemType> lbi in ListBoxItems)
        {
            if (_equalityComparer.Equals(lbi.Data, item))
            {
                //  Select it
                this.SelectedItems = new List<MgListBoxItem<TItemType>>() { lbi }.AsReadOnly();
                return;
            }
        }

        if (deselectAllIfNotFound)
            ClearSelection();
    }

    public void ClearSelection()
    {
        SelectionSourceItem = null;
        if (SelectedItems?.Count != 0)
            SelectedItems = new List<MgListBoxItem<TItemType>>().AsReadOnly();
    }
    #endregion Selection

    public MGScrollViewer ScrollViewer { get; }
    public MGStackPanel ItemsPanel { get; }

    /// <summary>Sets the <see cref="TitleBorderBrush"/> to the given <paramref name="brush"/> using the given <paramref name="borderThickness"/>, except with a bottom thickness of 0 to avoid doubled thickness between the title and content.<br/>
    /// Sets the <see cref="InnerBorderBrush"/> to the given <paramref name="brush"/> using the given <paramref name="borderThickness"/></summary>
    public void SetTitleAndContentBorder(IFillBrush brush, int borderThickness)
    {
        TitleBorderBrush = brush?.AsUniformBorderBrush();
        TitleBorderThickness = new(borderThickness, borderThickness, borderThickness, 0);

        InnerBorderBrush = brush?.AsUniformBorderBrush();
        InnerBorderThickness = new(borderThickness);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private MGElement _header;
    /// <summary>Content to display inside the <see cref="TitlePresenter"/>. Only relevant if <see cref="IsTitleVisible"/> is true.</summary>
    public MGElement Header
    {
        get => _header;
        set
        {
            if (_header != value)
            {
                _header = value;
                using (TitlePresenter.AllowChangingContentTemporarily())
                {
                    TitlePresenter.SetContent(Header);
                }
                Npc(nameof(Header));
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Func<TItemType, MGElement> _itemTemplate;
    /// <summary>This function is invoked to instantiate the <see cref="MgListBoxItem{TItemType}.Content"/> of each <see cref="MgListBoxItem{TItemType}"/> in this <see cref="MgListBox{TItemType}"/></summary>
    public Func<TItemType, MGElement> ItemTemplate
    {
        get => _itemTemplate;
        set
        {
            if (_itemTemplate != value)
            {
                _itemTemplate = value;
                Npc(nameof(ItemTemplate));
                ItemTemplateChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler<EventArgs> ItemTemplateChanged;

    public readonly MGUniformBorderBrush DefaultItemBorderBrush = new MGSolidFillBrush(Color.Black * 0.35f).AsUniformBorderBrush();
    public readonly Thickness DefaultItemBorderThickness = new(0, 1);

    public void ApplyDefaultItemContainerStyle(MGBorder item)
    {
        item.BorderBrush = DefaultItemBorderBrush;
        item.BorderThickness = DefaultItemBorderThickness;
        item.Padding = new(6, 4);
        item.BackgroundBrush = GetTheme().ListBoxItemBackground.GetValue(true);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Action<MGBorder> _itemContainerStyle;
    /// <summary>An action that will be invoked on every <see cref="MGBorder"/> that wraps each <see cref="MgListBoxItem{TItemType}"/>'s content.<para/>
    /// See also: <see cref="MgListBoxItem{TItemType}.ContentPresenter"/><para/>
    /// Default value: <see cref="ApplyDefaultItemContainerStyle(MGBorder)"/></summary>
    public Action<MGBorder> ItemContainerStyle
    {
        get => _itemContainerStyle;
        set
        {
            if (_itemContainerStyle != value)
            {
                _itemContainerStyle = value;
                Npc(nameof(ItemContainerStyle));
                ItemContainerStyleChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler<EventArgs> ItemContainerStyleChanged;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ReadOnlyCollection<IFillBrush> _alternatingRowBackgrounds;
    /// <summary>If not null/empty, the row items will cycle through these <see cref="IFillBrush"/> for their backgrounds</summary>
    public ReadOnlyCollection<IFillBrush> AlternatingRowBackgrounds
    {
        get => _alternatingRowBackgrounds;
        set
        {
            if (_alternatingRowBackgrounds != value)
            {
                _alternatingRowBackgrounds = value;
                RefreshRowBackgrounds();
                Npc(nameof(AlternatingRowBackgrounds));
            }
        }
    }

    private void RefreshRowBackgrounds()
    {
        if (InternalItems != null)
        {
            for (int i = 0; i < InternalItems.Count; i++)
            {
                IFillBrush brush = AlternatingRowBackgrounds?.Any() == true ? AlternatingRowBackgrounds[i % AlternatingRowBackgrounds.Count] : null;
                InternalItems[i].ContentPresenter.BackgroundBrush.NormalValue = brush;
            }
        }
    }

    public MgListBox(MgWindow parentWindow)
        : base(parentWindow, MGElementType.ListBox)
    {
        using (BeginInitializing())
        {
            //  Create the outer border
            this.OuterBorder = new(parentWindow, 0, MGSolidFillBrush.Black);
            this.OuterBorderComponent = MGComponentBase.Create(OuterBorder);
            AddComponent(OuterBorderComponent);
            OuterBorder.OnBorderBrushChanged += (sender, e) => { Npc(nameof(OuterBorderBrush)); };
            OuterBorder.OnBorderThicknessChanged += (sender, e) => { Npc(nameof(OuterBorderThickness)); };

            //  Create the title bar
            this.TitleBorder = new(parentWindow);
            TitleBorder.Padding = new(6, 3);
            TitleBorder.BackgroundBrush = GetTheme().TitleBackground.GetValue(true);
            TitleBorder.DefaultTextForeground.SetAll(Color.White);
            TitleBorder.OnBorderBrushChanged += (sender, e) => { Npc(nameof(TitleBorderBrush)); };
            TitleBorder.OnBorderThicknessChanged += (sender, e) => { Npc(nameof(TitleBorderThickness)); };
            this.TitlePresenter = new(parentWindow);
            TitlePresenter.VerticalAlignment = VerticalAlignment.Center;
            TitleBorder.SetContent(TitlePresenter);
            TitleBorder.CanChangeContent = false;
            TitlePresenter.CanChangeContent = false;
            this.TitleComponent = new(TitleBorder, true, false, false, true, false, false, false,
                (availableBounds, componentSize) => ApplyAlignment(availableBounds, HorizontalAlignment.Stretch, VerticalAlignment.Top, componentSize.Size));
            AddComponent(TitleComponent);

            //  Create the inner border
            this.InnerBorder = new(parentWindow);
            this.InnerBorderComponent = new(InnerBorder, true, false, true, true, false, false, false,
                (availableBounds, componentSize) => ApplyAlignment(availableBounds, HorizontalAlignment.Stretch, VerticalAlignment.Stretch, componentSize.Size));
            AddComponent(InnerBorderComponent);
            InnerBorder.OnBorderBrushChanged += (sender, e) => { Npc(nameof(InnerBorderBrush)); };
            InnerBorder.OnBorderThicknessChanged += (sender, e) => { Npc(nameof(InnerBorderThickness)); };

            //  Create the scrollviewer and itemspanel
            this.ItemsPanel = new(parentWindow, Orientation.Vertical);
            ItemsPanel.VerticalAlignment = VerticalAlignment.Top;
            ItemsPanel.CanChangeContent = false;
            this.ScrollViewer = new(parentWindow);
            ScrollViewer.Padding = new(0, 0);
            ScrollViewer.SetContent(ItemsPanel);
            ScrollViewer.CanChangeContent = false;
            InnerBorder.SetContent(ScrollViewer);
            InnerBorder.CanChangeContent = false;

            SetTitleAndContentBorder(MGSolidFillBrush.Black, 1);

            MinHeight = 30;

            AlternatingRowBackgrounds = GetTheme().ListBoxItemAlternatingRowBackgrounds.Select(x => x.GetValue(true)).ToList().AsReadOnly();

            ItemsPanel.BorderThickness = DefaultItemBorderThickness;
            ItemsPanel.BorderBrush = DefaultItemBorderBrush;

            this.ItemContainerStyle = ApplyDefaultItemContainerStyle;
            this.ItemTemplate = (item) => new MGTextBlock(parentWindow, item.ToString()) { Padding = new(1, 0) };

            this.SelectedItems = new List<MgListBoxItem<TItemType>>().AsReadOnly();
            this.SelectionMode = ListBoxSelectionMode.Single;
            this.CanDeselectByClickingSelectedItem = true;

            MouseHandler.LMBReleasedInside += (sender, e) =>
            {
                MgListBoxItem<TItemType> pressedItem = InternalItems?.FirstOrDefault(x => x.ContentPresenter.IsHovered);
                if (pressedItem != null)
                {
                    bool isPressedItemAlreadySelected = SelectedItems?.Contains(pressedItem) == true;
                    bool isShiftDown = InputTracker.Keyboard.IsShiftDown;
                    bool isControlDown = InputTracker.Keyboard.IsControlDown;

                    void SelectSingle()
                    {
                        if (isPressedItemAlreadySelected && CanDeselectByClickingSelectedItem && SelectedItems.Count <= 1)
                        {
                            ClearSelection();
                        }
                        else
                        {
                            SelectionSourceItem = pressedItem;
                            SelectedItems = new List<MgListBoxItem<TItemType>>() { pressedItem }.AsReadOnly();
                        }
                    }

                    void SelectContiguous()
                    {
                        if (!isShiftDown || SelectionSourceItem == null || !InternalItems.Contains(SelectionSourceItem))
                        {
                            SelectSingle();
                        }
                        else
                        {
                            int sourceIndex = InternalItems.IndexOf(SelectionSourceItem);
                            int pressedIndex = InternalItems.IndexOf(pressedItem);

                            int startIndex = Math.Min(sourceIndex, pressedIndex);
                            int endIndex = Math.Max(sourceIndex, pressedIndex);

                            SelectedItems = InternalItems.Skip(startIndex).Take(endIndex - startIndex + 1).ToList().AsReadOnly();
                        }
                    }

                    switch (this.SelectionMode)
                    {
                        case ListBoxSelectionMode.None:
                            break;
                        case ListBoxSelectionMode.Single:
                            SelectSingle();
                            break;
                        case ListBoxSelectionMode.Contiguous:
                            SelectContiguous();
                            break;
                        case ListBoxSelectionMode.Multiple:
                            if (isControlDown)
                            {
                                if (isPressedItemAlreadySelected)
                                    SelectedItems = SelectedItems.Where(x => x != pressedItem).ToList().AsReadOnly();
                                else
                                    SelectedItems = SelectedItems.Append(pressedItem).ToList().AsReadOnly();
                            }
                            else
                                SelectContiguous();
                            break;
                        default: throw new NotImplementedException($"Unrecognized {nameof(ListBoxSelectionMode)}: {nameof(SelectionMode)}");
                    }
                }
            };
        }
    }

    public override void DrawSelf(ElementDrawArgs da, Rectangle layoutBounds)
    {
        base.DrawSelf(da, layoutBounds);
    }

    //  This method is invoked via reflection in MGUI.Core.UI.XAML.Lists.ListBox.ApplyDerivedSettings.
    //  Do not modify the method signature.
    internal void LoadSettings(ListBox settings, bool includeContent)
    {
        MGDesktop desktop = GetDesktop();

        settings.OuterBorder.ApplySettings(this, OuterBorder, false);
        settings.InnerBorder.ApplySettings(this, InnerBorder, false);
        settings.TitleBorder.ApplySettings(this, TitleBorder, false);
        settings.TitlePresenter.ApplySettings(this, TitlePresenter, false);
        settings.ScrollViewer.ApplySettings(this, ScrollViewer, false);
        settings.ItemsPanel.ApplySettings(this, ItemsPanel, false);

        if (settings.Header != null)
            Header = settings.Header.ToElement<MGElement>(SelfOrParentWindow, this);

        if (settings.IsTitleVisible.HasValue)
        {
            IsTitleVisible = settings.IsTitleVisible.Value;
            if (!IsTitleVisible)
            {
                TitleBorderThickness = new(0);
                InnerBorderThickness = new(0);
            }
        }

        if (settings.Items?.Any() == true)
        {
            List<TItemType> tempItems = new();
            Type targetType = typeof(TItemType);
            foreach (object item in settings.Items)
            {
                if (targetType.IsInstanceOfType(item))
                {
                    TItemType value = (TItemType)item;
                    tempItems.Add(value);
                }
            }

            if (tempItems.Any())
            {
                SetItemsSource(tempItems);
            }
        }

        if (settings.CanDeselectByClickingSelectedItem.HasValue)
            CanDeselectByClickingSelectedItem = settings.CanDeselectByClickingSelectedItem.Value;
        if (settings.SelectionMode.HasValue)
            SelectionMode = settings.SelectionMode.Value;

        if (settings.AlternatingRowBackgrounds != null && settings.AlternatingRowBackgrounds.Any())
            AlternatingRowBackgrounds = settings.AlternatingRowBackgrounds.Select(x => x.ToFillBrush(desktop)).ToList().AsReadOnly();
        else
            AlternatingRowBackgrounds = new List<IFillBrush>().AsReadOnly();

        if (settings.ItemContainerStyle != null)
        {
            this.ItemContainerStyle = (border) => { settings.ItemContainerStyle.ApplySettings(this, border, false); };
        }

        if (settings.ItemTemplate != null)
        {
            this.ItemTemplate = (item) => settings.ItemTemplate.GetContent(SelfOrParentWindow, this, item);
        }
    }
}

public class MgListBoxItem<TItemType> : ViewModelBase
{
    public MgListBox<TItemType> ListBox { get; }

    /// <summary>The data object used as a parameter to generate the content of this item.<para/>
    /// See also: <see cref="MgListBox{TItemType}.ItemTemplate"/></summary>
    public TItemType Data { get; }

    /// <summary>The wrapper element that hosts this item's content</summary>
    public MGBorder ContentPresenter { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private MGElement _content;
    /// <summary>This <see cref="MGElement"/> is automatically generated via <see cref="MgListBox{TItemType}.ItemTemplate"/> using this.<see cref="Data"/> as the parameter.<para/>
    /// See also: <see cref="ContentPresenter"/></summary>
    public MGElement Content
    {
        get => _content;
        private set
        {
            if (_content != value)
            {
                _content = value;
                using (ContentPresenter.AllowChangingContentTemporarily())
                {
                    ContentPresenter.SetContent(Content);
                }
                Npc(nameof(Content));
            }
        }
    }

    internal MgListBoxItem(MgListBox<TItemType> listBox, TItemType data)
    {
        this.ListBox = listBox ?? throw new ArgumentNullException(nameof(listBox));
        this.Data = data ?? throw new ArgumentNullException(nameof(data));
        this.ContentPresenter = new(listBox.SelfOrParentWindow);

        listBox.ItemTemplateChanged += (sender, e) =>
        {
            Content?.RemoveDataBindings(true);
            Content = listBox.ItemTemplate?.Invoke(this.Data);
        };
        this.Content = listBox.ItemTemplate?.Invoke(this.Data);
        ContentPresenter.CanChangeContent = false;

        listBox.ItemContainerStyleChanged += (sender, e) => { listBox.ItemContainerStyle?.Invoke(this.ContentPresenter); };
        listBox.ItemContainerStyle?.Invoke(this.ContentPresenter);
    }
}