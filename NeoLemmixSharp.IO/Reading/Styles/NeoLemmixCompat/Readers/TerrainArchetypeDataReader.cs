using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers;

internal sealed class TerrainArchetypeDataReader : NeoLemmixDataReader
{
    private readonly string _terrainPieceFilePath;
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
        string terrainPieceFilePath,
        StyleIdentifier styleIdentifier,
        PieceIdentifier terrainPieceIdentifier)
        : base(string.Empty)
    {
        _terrainPieceFilePath = terrainPieceFilePath;
        _styleIdentifier = styleIdentifier;
        _terrainPieceIdentifier = terrainPieceIdentifier;

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
        Name = GetPieceName(),

        IsSteel = _isSteel,
        ResizeType = _resizeType,

        NineSliceData = GetNineSliceData(),

        DefaultSize = new Size(_defaultWidth, _defaultHeight)
    };

    private string GetPieceName()
    {
        var pieceName = Path.GetFileNameWithoutExtension(_terrainPieceFilePath);
        return pieceName;
    }

    private RectangularRegion GetNineSliceData()
    {
        var texture = GetTerrainTexture();

        var nineSliceWidth = texture.Width - (_nineSliceRight + _nineSliceLeft);
        var nineSliceHeight = texture.Height - (_nineSliceBottom + _nineSliceTop);

        var nineSlicePosition = new Point(_nineSliceTop, _nineSliceLeft);
        var nineSliceSize = new Size(nineSliceWidth, nineSliceHeight);

        return new RectangularRegion(nineSlicePosition, nineSliceSize);
    }

    private Texture2D GetTerrainTexture()
    {
        // Need to load the texture here to get its dimensions, since that data is not present in the NeoLemmix config file
        var pngFilePath = RootDirectoryManager.GetCorrespondingImageFile(_terrainPieceFilePath);
        return TextureCache.GetOrLoadTexture(pngFilePath, _styleIdentifier, _terrainPieceIdentifier, TextureType.TerrainSprite);
    }
}
