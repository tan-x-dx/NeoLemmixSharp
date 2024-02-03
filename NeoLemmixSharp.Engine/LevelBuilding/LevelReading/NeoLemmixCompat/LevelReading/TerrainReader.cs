using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class TerrainReader : INeoLemmixDataReader
{
    private readonly List<TerrainData> _allTerrainData;
    private readonly Dictionary<string, TerrainArchetypeData> _terrainArchetypes;

    private TerrainData? _currentTerrainData;
    private string? _currentStyle;

    private bool _settingDataForGroup;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$TERRAIN";

    public TerrainReader(
        Dictionary<string, TerrainArchetypeData> terrainArchetypes,
        List<TerrainData>? allTerrainData = null)
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
        ReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

        var currentTerrainData = _currentTerrainData!;

        switch (firstToken)
        {
            case "STYLE":
                var rest = ReadingHelpers.TrimAfterIndex(line, secondTokenIndex);
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
                    currentTerrainData.GroupName = secondToken.GetString();
                }
                else
                {
                    var terrainArchetypeData = GetOrLoadTerrainArchetypeData(ReadingHelpers.TrimAfterIndex(line, secondTokenIndex));
                    currentTerrainData.TerrainArchetypeId = terrainArchetypeData.TerrainArchetypeId;
                }
                break;

            case "X":
                currentTerrainData.X = int.Parse(secondToken);
                break;

            case "Y":
                currentTerrainData.Y = int.Parse(secondToken);
                break;

            case "RGB":
                currentTerrainData.Tint = ReadingHelpers.ParseHex<uint>(secondToken);
                break;

            case "NO_OVERWRITE":
                currentTerrainData.NoOverwrite = true;
                break;

            case "ONE_WAY":
                break;

            case "FLIP_VERTICAL":
                currentTerrainData.FlipVertical = true;
                break;

            case "FLIP_HORIZONTAL":
                currentTerrainData.FlipHorizontal = true;
                break;

            case "ROTATE":
                currentTerrainData.Rotate = true;
                break;

            case "ERASE":
                currentTerrainData.Erase = true;
                break;

            case "$END":
                _allTerrainData.Add(currentTerrainData);
                _currentTerrainData = null;
                _settingDataForGroup = false;
                FinishedReading = true;
                break;

            default:
                ReadingHelpers.ThrowUnknownTokenException("Gadget Archetype Data", firstToken, line);
                break;
        }

        return false;
    }

    private TerrainArchetypeData GetOrLoadTerrainArchetypeData(ReadOnlySpan<char> piece)
    {
        ref var terrainArchetypeData = ref ReadingHelpers.GetArchetypeDataRef(
            _currentStyle!,
            piece,
            _terrainArchetypes,
            out var exists);

        if (exists)
            return terrainArchetypeData!;

        var terrainPiece = piece.ToString();
        var rootFilePath = Path.Combine(RootDirectoryManager.RootDirectory, "styles", _currentStyle!, "terrain", terrainPiece);
        var isSteel = File.Exists(Path.ChangeExtension(rootFilePath, "nxmt"));

        terrainArchetypeData = new TerrainArchetypeData
        {
            TerrainArchetypeId = _terrainArchetypes.Count - 1,
            Style = _currentStyle,
            TerrainPiece = terrainPiece,
            IsSteel = isSteel
        };

        return terrainArchetypeData;
    }

    public void ApplyToLevelData(LevelData levelData)
    {
        levelData.AllTerrainData.AddRange(_allTerrainData);
    }

    public void Dispose()
    {
    }
}