using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Terrain;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers.TerrainReaders;

internal sealed class TerrainArchetypeDataReader : NeoLemmixDataReader
{
    private readonly StyleIdentifier _styleIdentifier;
    private readonly PieceIdentifier _terrainPieceIdentifier;

    private bool _isSteel;
    private ResizeType _resizeType = ResizeType.None;

    private int _nineSliceRight;
    private int _nineSliceTop;
    private int _nineSliceLeft;
    private int _nineSliceBottom;

    private int _defaultWidth;
    private int _defaultHeight;

    public TerrainArchetypeDataReader(
        string styleName,
        string terrainPieceName)
        : base(string.Empty)
    {
        _styleIdentifier = new StyleIdentifier(styleName);
        _terrainPieceIdentifier = new PieceIdentifier(terrainPieceName);

        SetNumberOfTokens(10);

        RegisterTokenAction("STEEL", SetSteel);
        RegisterTokenAction("RESIZE_HORIZONTAL", SetResizeHorizontal);
        RegisterTokenAction("RESIZE_VERTICAL", SetResizeVertical);
        RegisterTokenAction("RESIZE_BOTH", SetResizeBoth);
        RegisterTokenAction("NINE_SLICE_LEFT", SetNineSliceLeft);
        RegisterTokenAction("NINE_SLICE_TOP", SetNineSliceTop);
        RegisterTokenAction("NINE_SLICE_RIGHT", SetNineSliceRight);
        RegisterTokenAction("NINE_SLICE_BOTTOM", SetNineSliceBottom);
        RegisterTokenAction("DEFAULT_WIDTH", SetDefaultWidth);
        RegisterTokenAction("DEFAULT_HEIGHT", SetDefaultHeight);
    }

    public override bool ShouldProcessSection(ReadOnlySpan<char> token)
    {
        // Always choose this reader when dealing with terrain archetype data
        return true;
    }

    public override bool BeginReading(ReadOnlySpan<char> line) => true;

    private void SetSteel(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _isSteel = true;
    }

    private void SetResizeHorizontal(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _resizeType |= ResizeType.ResizeHorizontal;
    }

    private void SetResizeVertical(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _resizeType |= ResizeType.ResizeVertical;
    }

    private void SetResizeBoth(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _resizeType = ResizeType.ResizeBoth;
    }

    private void SetNineSliceLeft(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _resizeType |= ResizeType.ResizeHorizontal;
        _nineSliceLeft = int.Parse(secondToken);
    }

    private void SetNineSliceTop(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _resizeType |= ResizeType.ResizeVertical;
        _nineSliceTop = int.Parse(secondToken);
    }

    private void SetNineSliceRight(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _resizeType |= ResizeType.ResizeHorizontal;
        _nineSliceRight = int.Parse(secondToken);
    }

    private void SetNineSliceBottom(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _resizeType |= ResizeType.ResizeVertical;
        _nineSliceBottom = int.Parse(secondToken);
    }

    private void SetDefaultWidth(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _defaultWidth = int.Parse(secondToken);
    }

    private void SetDefaultHeight(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _defaultHeight = int.Parse(secondToken);
    }

    public TerrainArchetypeData CreateTerrainArchetypeData() => new()
    {
        StyleIdentifier = _styleIdentifier,
        PieceIdentifier = _terrainPieceIdentifier,
        Name = string.Empty,

        IsSteel = _isSteel,
        ResizeType = _resizeType,

        NineSliceData = default,//new RectangularRegion(_nineSliceBottom, _nineSliceLeft, _nineSliceTop, _nineSliceRight),

        DefaultSize = new Size(_defaultWidth, _defaultHeight)
    };
}