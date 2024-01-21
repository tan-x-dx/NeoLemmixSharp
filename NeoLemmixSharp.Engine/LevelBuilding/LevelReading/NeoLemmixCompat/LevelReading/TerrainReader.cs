using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class TerrainReader : INeoLemmixDataReader
{
    private readonly ICollection<TerrainData> _allTerrainData;
    private readonly Dictionary<string, TerrainArchetypeData> _terrainArchetypes;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$TERRAIN";

    private TerrainData? _currentTerrainData;
    private string? _currentStyle;

    private bool _settingDataForGroup;

    public TerrainReader(
        Dictionary<string, TerrainArchetypeData> terrainArchetypes,
        ICollection<TerrainData>? allTerrainData = null)
    {
        _terrainArchetypes = terrainArchetypes;
        _allTerrainData = allTerrainData ?? new List<TerrainData>();
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentTerrainData = new TerrainData();
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out var firstTokenIndex);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);
        var rest = secondToken.IsEmpty
            ? ReadOnlySpan<char>.Empty
            : line[(1 + firstTokenIndex + firstToken.Length)..];

        switch (firstToken)
        {
            case "STYLE":
                if (secondToken[0] == '*')
                {
                    _settingDataForGroup = true;
                }
                else
                {
                    if (_currentStyle is null)
                    {
                        _currentStyle = rest.GetString();
                    }
                    else
                    {
                        var currentStyleSpan = _currentStyle.AsSpan();
                        if (!currentStyleSpan.SequenceEqual(rest))
                        {
                            _currentStyle = rest.ToString();
                        }
                    }
                }
                break;

            case "PIECE":
                if (_settingDataForGroup)
                {
                    _currentTerrainData!.GroupName = secondToken.GetString();
                }
                else
                {
                    var terrainArchetypeData = GetOrLoadTerrainArchetypeData(rest);
                    _currentTerrainData!.TerrainArchetypeId = terrainArchetypeData.TerrainArchetypeId;
                }
                break;

            case "X":
                _currentTerrainData!.X = int.Parse(secondToken);
                break;

            case "Y":
                _currentTerrainData!.Y = int.Parse(secondToken);
                break;

            case "RGB":
                _currentTerrainData!.Tint = ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "NO_OVERWRITE":
                _currentTerrainData!.NoOverwrite = true;
                break;

            case "ONE_WAY":
                break;

            case "FLIP_VERTICAL":
                _currentTerrainData!.FlipVertical = true;
                break;

            case "FLIP_HORIZONTAL":
                _currentTerrainData!.FlipHorizontal = true;
                break;

            case "ROTATE":
                _currentTerrainData!.Rotate = true;
                break;

            case "ERASE":
                _currentTerrainData!.Erase = true;
                break;

            case "$END":
                _allTerrainData.Add(_currentTerrainData!);
                _currentTerrainData = null;
                _settingDataForGroup = false;
                FinishedReading = true;
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing {IdentifierToken}: [{firstToken}] line: \"{line}\"");
        }

        return false;
    }

    private TerrainArchetypeData GetOrLoadTerrainArchetypeData(ReadOnlySpan<char> piece)
    {
        var currentStyleLength = _currentStyle!.Length;
        Span<char> terrainArchetypeDataKey = stackalloc char[currentStyleLength + piece.Length + 1];

        _currentStyle.AsSpan().CopyTo(terrainArchetypeDataKey);
        piece.CopyTo(terrainArchetypeDataKey[(currentStyleLength + 1)..]);
        terrainArchetypeDataKey[currentStyleLength] = ':';

        if (ReadingHelpers.TryGetWithSpan(_terrainArchetypes, terrainArchetypeDataKey, out var terrainArchetypeData))
            return terrainArchetypeData;

        var terrainPiece = piece.ToString();
        var rootFilePath = Path.Combine(RootDirectoryManager.RootDirectory, "styles", _currentStyle, "terrain", terrainPiece);
        var isSteel = File.Exists(Path.ChangeExtension(rootFilePath, "nxmt"));

        terrainArchetypeData = new TerrainArchetypeData
        {
            TerrainArchetypeId = _terrainArchetypes.Count,
            Style = _currentStyle,
            TerrainPiece = terrainPiece,
            IsSteel = isSteel
        };

        var key = terrainArchetypeDataKey.ToString();
        _terrainArchetypes[key] = terrainArchetypeData;

        return terrainArchetypeData;
    }

    public void ApplyToLevelData(LevelData levelData)
    {
        levelData.AllTerrainData.AddRange(_allTerrainData);
    }

    public void Dispose()
    {
        _allTerrainData.Clear();
    }
}