using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.TerrainReaders;

public sealed class TerrainReader : NeoLemmixDataReader
{
    private readonly UniqueStringSet _uniqueStringSet;
    private readonly List<TerrainData> _allTerrainData;

    private TempTerrainData _currentTerrainData;

    private bool _settingDataForGroup;

    public TerrainReader(
        UniqueStringSet uniqueStringSet,
        List<TerrainData> allTerrainData)
        : base("$TERRAIN")
    {
        _uniqueStringSet = uniqueStringSet;
        _allTerrainData = allTerrainData;

        SetNumberOfTokens(13);

        RegisterTokenAction("STYLE", SetStyle);
        RegisterTokenAction("PIECE", SetPiece);
        RegisterTokenAction("X", SetX);
        RegisterTokenAction("Y", SetY);
        RegisterTokenAction("WIDTH", SetWidth);
        RegisterTokenAction("HEIGHT", SetHeight);
        RegisterTokenAction("NO_OVERWRITE", SetNoOverwrite);
        RegisterTokenAction("ONE_WAY", SetOneWay);
        RegisterTokenAction("ROTATE", SetRotate);
        RegisterTokenAction("FLIP_HORIZONTAL", SetFlipHorizontal);
        RegisterTokenAction("FLIP_VERTICAL", SetFlipVertical);
        RegisterTokenAction("ERASE", SetErase);
        RegisterTokenAction("$END", OnEnd);
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        _currentTerrainData = new TempTerrainData();

        FinishedReading = false;
        return false;
    }

    private void SetStyle(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        if (secondToken[0] == '*')
        {
            _settingDataForGroup = true;
            _currentTerrainData.Style = string.Empty;
        }
        else
        {
            _currentTerrainData.Style = _uniqueStringSet.GetUniqueStringInstance(line[secondTokenIndex..]);
        }
    }

    private void SetPiece(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var item = _uniqueStringSet.GetUniqueStringInstance(secondToken);

        if (_settingDataForGroup)
        {
            _currentTerrainData.GroupName = item;
            _currentTerrainData.TerrainPiece = string.Empty;
        }
        else
        {
            _currentTerrainData.GroupName = null;
            _currentTerrainData.TerrainPiece = item;
        }
    }

    private void SetX(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTerrainData!.X = int.Parse(secondToken);
    }

    private void SetY(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTerrainData!.Y = int.Parse(secondToken);
    }

    private void SetWidth(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTerrainData!.Width = int.Parse(secondToken);
    }

    private void SetHeight(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTerrainData!.Height = int.Parse(secondToken);
    }

    private void SetNoOverwrite(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTerrainData!.NoOverwrite = true;
    }

    private void SetOneWay(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetRotate(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTerrainData.Rotate = true;
    }

    private void SetFlipHorizontal(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTerrainData.FlipHorizontally = true;
    }

    private void SetFlipVertical(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTerrainData.FlipVertically = true;
    }

    private void SetErase(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTerrainData!.Erase = true;
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var dht = new DihedralTransformation(
            _currentTerrainData.FlipHorizontally,
            _currentTerrainData.FlipVertically,
            _currentTerrainData.Rotate);

        var newTerrainData = new TerrainData
        {
            GroupName = _currentTerrainData.GroupName,
            Style = _currentTerrainData.Style ?? string.Empty,
            TerrainPiece = _currentTerrainData.TerrainPiece,

            Position = new Point(_currentTerrainData.X, _currentTerrainData.Y),

            NoOverwrite = _currentTerrainData.NoOverwrite,
            Orientation = dht.Orientation,
            FacingDirection = dht.FacingDirection,
            Erase = _currentTerrainData.Erase,

            Tint = null,

            Width = _currentTerrainData.Width,
            Height = _currentTerrainData.Height,
        };

        _allTerrainData.Add(newTerrainData);
        _settingDataForGroup = false;
        FinishedReading = true;
    }

    private struct TempTerrainData
    {
        public string? GroupName;
        public string? Style;
        public string TerrainPiece;

        public int X;
        public int Y;

        public bool Rotate;
        public bool FlipHorizontally;
        public bool FlipVertically;

        public bool NoOverwrite;
        public bool Erase;

        public int? Width;
        public int? Height;
    }
}