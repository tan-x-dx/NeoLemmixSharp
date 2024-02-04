using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.TerrainReading;

public sealed class TerrainReader : INeoLemmixDataReader
{
    private readonly List<TerrainData> _allTerrainData;
    private readonly Dictionary<string, TerrainArchetypeData> _terrainArchetypes;

    private TerrainData? _currentTerrainData;
    private string? _currentStyle;
    private string? _currentFolder;

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
                var rest = line.TrimAfterIndex(secondTokenIndex);
                if (secondToken[0] == '*')
                {
                    _settingDataForGroup = true;
                }
                else
                {
                    if (_currentStyle is null)
                    {
                        SetCurrentStyle(rest);
                    }
                    else
                    {
                        var currentStyleSpan = _currentStyle.AsSpan();
                        if (!currentStyleSpan.SequenceEqual(rest))
                        {
                            SetCurrentStyle(rest);
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
                    var terrainArchetypeData = GetOrLoadTerrainArchetypeData(line.TrimAfterIndex(secondTokenIndex));
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

    private void SetCurrentStyle(ReadOnlySpan<char> style)
    {
        _currentStyle = style.GetString();
        _currentFolder = Path.Combine(
            RootDirectoryManager.RootDirectory, 
            NeoLemmixFileExtensions.StyleFolderName,
            _currentStyle, 
            NeoLemmixFileExtensions.TerrainFolderName);
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
        terrainArchetypeData = new TerrainArchetypeData
        {
            TerrainArchetypeId = _terrainArchetypes.Count - 1,
            Style = _currentStyle,
            TerrainPiece = terrainPiece
        };

        ProcessTerrainArchetypeData(terrainArchetypeData);

        return terrainArchetypeData;
    }

    private void ProcessTerrainArchetypeData(TerrainArchetypeData terrainArchetypeData)
    {
        var rootFilePath = Path.Combine(_currentFolder!, terrainArchetypeData.TerrainPiece!);
        rootFilePath = Path.ChangeExtension(rootFilePath, NeoLemmixFileExtensions.TerrainFileExtension);

        if (!File.Exists(rootFilePath))
            return;

        using var dataReaderList = new DataReaderList();

        dataReaderList.Add(new TerrainArchetypeDataReader(terrainArchetypeData));

        dataReaderList.ReadFile(rootFilePath);
    }

    public void ApplyToLevelData(LevelData levelData)
    {
        levelData.AllTerrainData.AddRange(_allTerrainData);
    }

    public void Dispose()
    {
    }
}