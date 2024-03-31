using MGUI.Core.UI.Containers;
using MGUI.Core.UI.Containers.Grids;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;

#if UseWPF
using System.Windows.Markup;
#else
using Portable.Xaml.Markup;
#endif

namespace MGUI.Core.UI.XAML;

[ContentProperty(nameof(Children))]
public abstract class MultiContentHost : Element
{
    [Browsable(false)]
    public List<Element> Children { get; set; } = new();

    protected internal override IEnumerable<Element> GetChildren() => Children;
}

[ContentProperty(nameof(Content))]
public abstract class SingleContentHost : Element
{
    [Category("Data")]
    public Element Content { get; set; }

    protected internal override void ApplyDerivedSettings(MGElement parent, MGElement element, bool includeContent)
    {
        if (includeContent && Content != null)
        {
            var typedElement = (MGSingleContentHost)element;
            typedElement.SetContent(Content.ToElement<MGElement>(element.SelfOrParentWindow, element));
        }
    }

    protected internal override IEnumerable<Element> GetChildren()
    {
        if (Content != null)
            yield return Content;
    }
}

public struct ColumnDefinition
{
    [Category("Layout")]
    public GridLength Length { get; set; }
    [Category("Layout")]
    public int? MinWidth { get; set; }
    [Category("Layout")]
    public int? MaxWidth { get; set; }
    public readonly override string ToString() => $"{nameof(ColumnDefinition)}: {Length}";
}

public struct RowDefinition
{
    [Category("Layout")]
    public GridLength Length { get; set; }
    [Category("Layout")]
    public int? MinHeight { get; set; }
    [Category("Layout")]
    public int? MaxHeight { get; set; }
    public readonly override string ToString() => $"{nameof(RowDefinition)}: {Length}";
}

public class GridSplitter : Element
{
    public override MGElementType ElementType => MGElementType.GridSplitter;

    [Category("Layout")]
    public int? Size { get; set; }
    [Category("Layout")]
    public Size? TickSize { get; set; }
    [Category("Appearance")]
    public FillBrush Foreground { get; set; }

    protected override MGElement CreateElementInstance(MgWindow window, MGElement parent) => new MGGridSplitter(window);

    protected internal override void ApplyDerivedSettings(MGElement parent, MGElement element, bool includeContent)
    {
        var desktop = element.GetDesktop();

        var gridSplitter = (MGGridSplitter)element;

        if (Size.HasValue)
            gridSplitter.Size = Size.Value;
        if (TickSize.HasValue)
            gridSplitter.TickSize = TickSize.Value.ToSize();
        if (Foreground != null)
            gridSplitter.Foreground.NormalValue = Foreground.ToFillBrush(desktop);
    }

    protected internal override IEnumerable<Element> GetChildren() => Enumerable.Empty<Element>();
}

public class Grid : MultiContentHost
{
    public override MGElementType ElementType => MGElementType.Grid;

    [Category("Layout")]
    public string RowLengths { get; set; }
    [Category("Layout")]
    public string ColumnLengths { get; set; }
    [Category("Layout")]
    public List<RowDefinition> RowDefinitions { get; set; } = new();
    [Category("Layout")]
    public List<ColumnDefinition> ColumnDefinitions { get; set; } = new();

    [Category("Behavior")]
    public GridSelectionMode? SelectionMode { get; set; }
    [Category("Behavior")]
    public bool? CanDeselectByClickingSelectedCell { get; set; }
    [Category("Appearance")]
    public FillBrush SelectionBackground { get; set; }
    [Category("Appearance")]
    public FillBrush SelectionOverlay { get; set; }

    [Category("Appearance")]
    public GridLineIntersection? GridLineIntersectionHandling { get; set; }
    [Category("Appearance")]
    public GridLinesVisibility? GridLinesVisibility { get; set; }
    [Category("Layout")]
    public int? GridLineMargin { get; set; }
    [Category("Appearance")]
    public FillBrush HorizontalGridLineBrush { get; set; }
    [Category("Appearance")]
    public FillBrush VerticalGridLineBrush { get; set; }

    [Category("Layout")]
    public int? RowSpacing { get; set; }
    [Category("Layout")]
    public int? ColumnSpacing { get; set; }

    protected override MGElement CreateElementInstance(MgWindow window, MGElement parent) => new MGGrid(window);

    protected internal override void ApplyDerivedSettings(MGElement parent, MGElement element, bool includeContent)
    {
        var desktop = element.GetDesktop();

        var grid = (MGGrid)element;

        if (RowLengths != null)
        {
            grid.AddRows(ConstrainedGridLength.ParseMultiple(RowLengths));
        }

        if (ColumnLengths != null)
        {
            grid.AddColumns(ConstrainedGridLength.ParseMultiple(ColumnLengths));
        }

        foreach (var rowDefinition in RowDefinitions)
        {
            var rd = grid.AddRow(rowDefinition.Length);
            rd.SetSizeConstraints(rowDefinition.MinHeight, rowDefinition.MaxHeight);
        }

        foreach (var columnDefinition in ColumnDefinitions)
        {
            var cd = grid.AddColumn(columnDefinition.Length);
            cd.SetSizeConstraints(columnDefinition.MinWidth, columnDefinition.MaxWidth);
        }

        if (SelectionMode.HasValue)
            grid.SelectionMode = SelectionMode.Value;
        if (CanDeselectByClickingSelectedCell.HasValue)
            grid.CanDeselectByClickingSelectedCell = CanDeselectByClickingSelectedCell.Value;
        if (SelectionBackground != null)
            grid.SelectionBackground = SelectionBackground.ToFillBrush(desktop);
        if (SelectionOverlay != null)
            grid.SelectionOverlay = SelectionOverlay.ToFillBrush(desktop);

        if (GridLineIntersectionHandling.HasValue)
            grid.GridLineIntersectionHandling = GridLineIntersectionHandling.Value;
        if (GridLinesVisibility.HasValue)
            grid.GridLinesVisibility = GridLinesVisibility.Value;
        if (GridLineMargin.HasValue)
            grid.GridLineMargin = GridLineMargin.Value;
        if (HorizontalGridLineBrush != null)
            grid.HorizontalGridLineBrush = HorizontalGridLineBrush.ToFillBrush(desktop);
        if (VerticalGridLineBrush != null)
            grid.VerticalGridLineBrush = VerticalGridLineBrush.ToFillBrush(desktop);

        if (RowSpacing.HasValue)
            grid.RowSpacing = RowSpacing.Value;
        if (ColumnSpacing.HasValue)
            grid.ColumnSpacing = ColumnSpacing.Value;

        if (includeContent && Children.Count > 0)
        {
            if (string.IsNullOrEmpty(RowLengths) && RowDefinitions.Count == 0)
                grid.AddRow(GridLength.Auto);
            if (string.IsNullOrEmpty(ColumnLengths) && ColumnDefinitions.Count == 0)
                grid.AddColumn(GridLength.Auto);

            foreach (var child in Children)
            {
                var childElement = child.ToElement<MGElement>(grid.SelfOrParentWindow, grid);
                grid.TryAddChild(child.GridRow, child.GridColumn, new GridSpan(child.GridRowSpan, child.GridColumnSpan, child.GridAffectsMeasure), childElement);
            }
        }
    }
}

public class UniformGrid : MultiContentHost
{
    public override MGElementType ElementType => MGElementType.UniformGrid;

    [Category("Layout")]
    public int? Rows { get; set; }
    [Category("Layout")]
    public int? Columns { get; set; }

    [Category("Layout")]
    public Size? CellSize { get; set; }
    [Category("Layout")]
    public int? HeaderRowHeight { get; set; }
    [Category("Layout")]
    public int? HeaderColumnWidth { get; set; }

    [Category("Behavior")]
    public GridSelectionMode? SelectionMode { get; set; }
    [Category("Behavior")]
    public bool? CanDeselectByClickingSelectedCell { get; set; }
    [Category("Appearance")]
    public FillBrush SelectionBackground { get; set; }
    [Category("Appearance")]
    public FillBrush SelectionOverlay { get; set; }

    [Category("Appearance")]
    public GridLineIntersection? GridLineIntersectionHandling { get; set; }
    [Category("Appearance")]
    public GridLinesVisibility? GridLinesVisibility { get; set; }
    [Category("Layout")]
    public int? GridLineMargin { get; set; }
    [Category("Appearance")]
    public FillBrush HorizontalGridLineBrush { get; set; }
    [Category("Appearance")]
    public FillBrush VerticalGridLineBrush { get; set; }

    [Category("Layout")]
    public int? RowSpacing { get; set; }
    [Category("Layout")]
    public int? ColumnSpacing { get; set; }

    [Category("Appearance")]
    public FillBrush CellBackground { get; set; }
    [Category("Behavior")]
    public bool? DrawEmptyCells { get; set; }

    /// <summary>If true, the Row and Column values of each child element will be automatically assigned in order.<para/>
    /// For example, if there are 2 columns, 3 rows:<br/>
    /// 1st child will be placed in Row=0,Column=0. 2nd child will be placed in Row=0,Column=1. 3rd child will be placed in Row=1,Column=0 etc.<para/>
    /// Explicitly setting the child element's row or column to a non-zero value will override this behavior.</summary>
    [Category("Behavior")]
    public bool? AutoAssignCells { get; set; }

    protected override MGElement CreateElementInstance(MgWindow window, MGElement parent) => new MGUniformGrid(window, Rows ?? 0, Columns ?? 0, CellSize?.ToSize() ?? MonoGame.Extended.Size.Empty);

    protected internal override void ApplyDerivedSettings(MGElement parent, MGElement element, bool includeContent)
    {
        var desktop = element.GetDesktop();

        var grid = (MGUniformGrid)element;

        if (Rows.HasValue)
            grid.Rows = Rows.Value;
        if (Columns.HasValue)
            grid.Columns = Columns.Value;
        if (CellSize.HasValue)
            grid.CellSize = CellSize.Value.ToSize();
        if (HeaderRowHeight.HasValue)
            grid.HeaderRowHeight = HeaderRowHeight.Value;
        if (HeaderColumnWidth.HasValue)
            grid.HeaderColumnWidth = HeaderColumnWidth.Value;

        if (SelectionMode.HasValue)
            grid.SelectionMode = SelectionMode.Value;
        if (CanDeselectByClickingSelectedCell.HasValue)
            grid.CanDeselectByClickingSelectedCell = CanDeselectByClickingSelectedCell.Value;
        if (SelectionBackground != null)
            grid.SelectionBackground = SelectionBackground.ToFillBrush(desktop);
        if (SelectionOverlay != null)
            grid.SelectionOverlay = SelectionOverlay.ToFillBrush(desktop);

        if (GridLineIntersectionHandling.HasValue)
            grid.GridLineIntersectionHandling = GridLineIntersectionHandling.Value;
        if (GridLinesVisibility.HasValue)
            grid.GridLinesVisibility = GridLinesVisibility.Value;
        if (GridLineMargin.HasValue)
            grid.GridLineMargin = GridLineMargin.Value;
        if (HorizontalGridLineBrush != null)
            grid.HorizontalGridLineBrush = HorizontalGridLineBrush.ToFillBrush(desktop);
        if (VerticalGridLineBrush != null)
            grid.VerticalGridLineBrush = VerticalGridLineBrush.ToFillBrush(desktop);

        if (RowSpacing.HasValue)
            grid.RowSpacing = RowSpacing.Value;
        if (ColumnSpacing.HasValue)
            grid.ColumnSpacing = ColumnSpacing.Value;

        if (CellBackground != null)
            grid.CellBackground.NormalValue = CellBackground.ToFillBrush(desktop);
        if (DrawEmptyCells.HasValue)
            grid.DrawEmptyCells = DrawEmptyCells.Value;

        if (includeContent)
        {
            //  Try to calculate the length of the other dimension if only the Rows or only the Columns are specified
            var numChildren = Children.Count;
            if (Rows.HasValue && !Columns.HasValue)
            {
                Columns = (int)Math.Ceiling(numChildren / (double)Rows.Value);
                grid.Columns = Columns.Value;
            }
            else if (Columns.HasValue && !Rows.HasValue)
            {
                Rows = (int)Math.Ceiling(numChildren / (double)Columns.Value);
                grid.Rows = Rows.Value;
            }

            //  Add each child to the grid
            var counter = 0;
            foreach (var child in Children)
            {
                var childElement = child.ToElement<MGElement>(grid.SelfOrParentWindow, grid);

                var row = child.GridRow;
                var column = child.GridColumn;
                if (AutoAssignCells.HasValue && AutoAssignCells.Value)
                {
                    if (row == 0)
                        row = counter / grid.Columns;
                    if (column == 0)
                        column = counter % grid.Columns;
                }

                grid.TryAddChild(row, column, childElement);
                counter++;
            }
        }
    }
}

public class DockPanel : MultiContentHost
{
    public override MGElementType ElementType => MGElementType.DockPanel;

    public bool? LastChildFill { get; set; }

    protected override MGElement CreateElementInstance(MgWindow window, MGElement parent) => new MGDockPanel(window);

    protected internal override void ApplyDerivedSettings(MGElement parent, MGElement element, bool includeContent)
    {
        var dockPanel = (MGDockPanel)element;

        if (LastChildFill.HasValue)
            dockPanel.LastChildFill = LastChildFill.Value;

        if (includeContent)
        {
            foreach (var child in Children)
            {
                var childElement = child.ToElement<MGElement>(dockPanel.ParentWindow, dockPanel);
                dockPanel.TryAddChild(childElement, child.Dock);
            }
        }
    }
}

public class StackPanel : MultiContentHost
{
    public override MGElementType ElementType => MGElementType.StackPanel;

    [Category("Border")]
    public Border Border { get; set; } = new();

    [Category("Border")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public BorderBrush BorderBrush { get => Border.BorderBrush; set => Border.BorderBrush = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public BorderBrush Bb { get => BorderBrush; set => BorderBrush = value; }

    [Category("Border")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Thickness? BorderThickness { get => Border.BorderThickness; set => Border.BorderThickness = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public Thickness? Bt { get => BorderThickness; set => BorderThickness = value; }

    [Category("Layout")]
    public Orientation? Orientation { get; set; }
    [Category("Layout")]
    public int? Spacing { get; set; }

    protected override MGElement CreateElementInstance(MgWindow window, MGElement parent) => new MGStackPanel(window, Orientation ?? UI.Orientation.Vertical);

    protected internal override void ApplyDerivedSettings(MGElement parent, MGElement element, bool includeContent)
    {
        var stackPanel = (MGStackPanel)element;
        Border.ApplySettings(parent, stackPanel.BorderComponent.Element, false);

        if (Orientation.HasValue)
            stackPanel.Orientation = Orientation.Value;
        if (Spacing.HasValue)
            stackPanel.Spacing = Spacing.Value;

        if (includeContent)
        {
            foreach (var child in Children)
            {
                var childElement = child.ToElement<MGElement>(stackPanel.ParentWindow, stackPanel);
                stackPanel.TryAddChild(childElement);
            }
        }
    }
}

public class OverlayPanel : MultiContentHost
{
    public override MGElementType ElementType => MGElementType.OverlayPanel;

    protected override MGElement CreateElementInstance(MgWindow window, MGElement parent) => new MGOverlayPanel(window);

    protected internal override void ApplyDerivedSettings(MGElement parent, MGElement element, bool includeContent)
    {
        var overlayPanel = (MGOverlayPanel)element;

        if (includeContent)
        {
            foreach (var child in Children)
            {
                var childElement = child.ToElement<MGElement>(overlayPanel.ParentWindow, overlayPanel);
                overlayPanel.TryAddChild(childElement, child.Offset.ToThickness(), child.ZIndex);
            }
        }
    }
}