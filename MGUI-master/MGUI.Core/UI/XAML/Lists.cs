using MGUI.Core.UI.Containers.Grids;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;

#if UseWPF
using System.Windows.Markup;
#else
using Portable.Xaml.Markup;
#endif

namespace MGUI.Core.UI.XAML;

[ContentProperty(nameof(Items))]
public class ListBox : MultiContentHost
{
    public override MGElementType ElementType => MGElementType.ListBox;

    /// <summary>The generic type that will be used when instantiating <see cref="MGListBox{TItemType}"/>.<para/>
    /// To set this value from a XAML string, you must define the namespace the type belongs to, then use the x:Type Markup Extension<br/>
    /// (See: <see href="https://learn.microsoft.com/en-us/dotnet/desktop/xaml-services/xtype-markup-extension"/>)<para/>
    /// Example:
    /// <code>&lt;ListBox xmlns:System="clr-namespace:System;assembly=mscorlib" ItemType="{x:Type System:Double}" /&gt;</code><para/>
    /// Default value: <code>typeof(object)</code></summary>
    [Category("Data")]
    public Type ItemType { get; set; } = typeof(object);

    #region Borders
    [Category("Border")]
    public Border OuterBorder { get; set; } = new();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Border")]
    public BorderBrush OuterBorderBrush { get => OuterBorder.BorderBrush; set => OuterBorder.BorderBrush = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public BorderBrush OuterBb { get => OuterBorderBrush; set => OuterBorderBrush = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Border")]
    public Thickness? OuterBorderThickness { get => OuterBorder.BorderThickness; set => OuterBorder.BorderThickness = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public Thickness? OuterBt { get => OuterBorderThickness; set => OuterBorderThickness = value; }

    [Category("Border")]
    public Border InnerBorder { get; set; } = new();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Border")]
    public BorderBrush InnerBorderBrush { get => InnerBorder.BorderBrush; set => InnerBorder.BorderBrush = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public BorderBrush InnerBb { get => InnerBorderBrush; set => InnerBorderBrush = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Border")]
    public Thickness? InnerBorderThickness { get => InnerBorder.BorderThickness; set => InnerBorder.BorderThickness = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public Thickness? InnerBt { get => InnerBorderThickness; set => InnerBorderThickness = value; }

    [Category("Border")]
    public Border TitleBorder { get; set; } = new();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Border")]
    public BorderBrush TitleBorderBrush { get => TitleBorder.BorderBrush; set => TitleBorder.BorderBrush = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public BorderBrush TitleBb { get => TitleBorderBrush; set => TitleBorderBrush = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Border")]
    public Thickness? TitleBorderThickness { get => TitleBorder.BorderThickness; set => TitleBorder.BorderThickness = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public Thickness? TitleBt { get => TitleBorderThickness; set => TitleBorderThickness = value; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Border")]
    public BorderBrush ItemsPanelBorderBrush { get => ItemsPanel.BorderBrush; set => ItemsPanel.BorderBrush = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public BorderBrush ItemsPanelBb { get => ItemsPanelBorderBrush; set => ItemsPanelBorderBrush = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Border")]
    public Thickness? ItemsPanelBorderThickness { get => ItemsPanel.BorderThickness; set => ItemsPanel.BorderThickness = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public Thickness? ItemsPanelBt { get => ItemsPanelBorderThickness; set => ItemsPanelBorderThickness = value; }
    #endregion Borders

    [Category("Title")]
    public ContentPresenter TitlePresenter { get; set; } = new();
    [Category("Title")]
    public bool? IsTitleVisible { get; set; }

    public ScrollViewer ScrollViewer { get; set; } = new();
    public StackPanel ItemsPanel { get; set; } = new();

    [Category("Title")]
    public Element Header { get; set; }

    [Category("Data")]
    public List<object> Items { get; set; } = [];

    [Category("Behavior")]
    public bool? CanDeselectByClickingSelectedItem { get; set; }
    [Category("Behavior")]
    public ListBoxSelectionMode? SelectionMode { get; set; }

    [Category("Appearance")]
    public List<FillBrush> AlternatingRowBackgrounds { get; set; } = [];

    [Category("Data")]
    public ContentTemplate ItemTemplate { get; set; }

    [Category("Appearance")]
    public Border ItemContainerStyle { get; set; } = null;

    protected override MGElement CreateElementInstance(MGWindow window, MGElement parent)
    {
        var genericType = typeof(MGListBox<>).MakeGenericType([ItemType]);
        var element = Activator.CreateInstance(genericType, [window]);
        return element as MGElement;
    }

    protected internal override void ApplyDerivedSettings(MGElement parent, MGElement element, bool includeContent)
    {
        var genericType = typeof(MGListBox<>).MakeGenericType([ItemType]);
        var method = genericType.GetMethod(nameof(MGListBox<object>.LoadSettings), BindingFlags.Instance | BindingFlags.NonPublic);
        method!.Invoke(element, [this, includeContent]);
    }

    protected internal override IEnumerable<Element> GetChildren()
    {
        foreach (var element in base.GetChildren())
            yield return element;

        yield return OuterBorder;
        yield return InnerBorder;
        yield return TitleBorder;
        yield return TitlePresenter;
        yield return ScrollViewer;
        yield return ItemsPanel;

        if (Header != null)
            yield return Header;
        if (ItemTemplate?.Content != null)
            yield return ItemTemplate.Content;
        if (ItemContainerStyle != null)
            yield return ItemContainerStyle;
    }
}

[ContentProperty(nameof(Columns))]
public class ListView : MultiContentHost
{
    public override MGElementType ElementType => MGElementType.ListView;

    /// <summary>The generic type that will be used when instantiating <see cref="MGListView{TItemType}"/>.<para/>
    /// To set this value from a XAML string, you must define the namespace the type belongs to, then use the x:Type Markup Extension<br/>
    /// (See: <see href="https://learn.microsoft.com/en-us/dotnet/desktop/xaml-services/xtype-markup-extension"/>)<para/>
    /// Example:
    /// <code>&lt;ListView xmlns:System="clr-namespace:System;assembly=mscorlib" ItemType="{x:Type System:Double}" /&gt;</code><para/>
    /// Default value: <code>typeof(object)</code></summary>
    [Category("Data")]
    public Type ItemType { get; set; } = typeof(object);

    [Category("Layout")]
    public List<ListViewColumn> Columns { get; set; } = [];
    [Category("Layout")]
    public int? RowHeight { get; set; }

    public Grid HeaderGrid { get; set; } = new();
    public ScrollViewer ScrollViewer { get; set; } = new();
    public Grid DataGrid { get; set; } = new();

    [Category("Behavior")]
    public GridSelectionMode? SelectionMode { get; set; }

    protected override MGElement CreateElementInstance(MGWindow window, MGElement parent)
    {
        var genericType = typeof(MGListView<>).MakeGenericType([ItemType]);
        var element = Activator.CreateInstance(genericType, [window]);
        return element as MGElement;
    }

    protected internal override void ApplyDerivedSettings(MGElement parent, MGElement element, bool includeContent)
    {
        var genericType = typeof(MGListView<>).MakeGenericType([ItemType]);
        var method = genericType.GetMethod(nameof(MGListView<object>.LoadSettings), BindingFlags.Instance | BindingFlags.NonPublic);
        method!.Invoke(element, [this, includeContent]);
    }

    protected internal override IEnumerable<Element> GetChildren()
    {
        foreach (var element in base.GetChildren())
            yield return element;

        yield return HeaderGrid;
        yield return ScrollViewer;
        yield return DataGrid;
    }
}

[ContentProperty(nameof(Header))]
public class ListViewColumn
{
    [Category("Layout")]
    public ListViewColumnWidth Width { get; set; }
    [Category("Appearance")]
    public Element Header { get; set; }
    [Category("Appearance")]
    public ContentTemplate CellTemplate { get; set; }
}

[TypeConverter(typeof(ListViewColumnWidthStringConverter))]
public class ListViewColumnWidth
{
    [Category("Layout")]
    public int? WidthPixels { get; set; }
    [Category("Layout")]
    public double? WidthWeight { get; set; }

    public UI.ListViewColumnWidth ToWidth()
    {
        if (WidthPixels.HasValue)
            return new UI.ListViewColumnWidth(WidthPixels.Value);
        if (WidthWeight.HasValue)
            return new UI.ListViewColumnWidth(WidthWeight.Value);

        throw new InvalidOperationException($"{nameof(ListViewColumnWidth)} must define either an integral value for {nameof(WidthPixels)} or a double value for {nameof(WidthWeight)}");
    }

    public override string ToString() => $"{nameof(ListViewColumnWidth)}: {(WidthPixels.HasValue ? WidthPixels + "px" : WidthWeight + "*")}";

    public static ListViewColumnWidth Parse(string value)
    {
        if (value.EndsWith('*'))
        {
            var weightString = value.AsSpan()[..(value.Length - 1)];
            var weight = weightString == string.Empty ? 1.0 : double.Parse(weightString);
            return new ListViewColumnWidth { WidthWeight = weight };
        }
        else
        {
            var pixelsString = value.Replace("px", "", StringComparison.CurrentCultureIgnoreCase);
            var pixels = int.Parse(pixelsString);
            return new ListViewColumnWidth { WidthPixels = pixels };
        }
    }
}

public class ListViewColumnWidthStringConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (sourceType == typeof(string))
            return true;
        return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            var width = ListViewColumnWidth.Parse(stringValue);
            return width;
        }

        return base.ConvertFrom(context, culture, value);
    }
}