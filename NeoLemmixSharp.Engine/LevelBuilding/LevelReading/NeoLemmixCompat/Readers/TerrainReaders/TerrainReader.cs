using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.TerrainReaders;

public sealed class TerrainReader : INeoLemmixDataReader
{
    private readonly List<TerrainData> _allTerrainData;
    private readonly Dictionary<string, TerrainArchetypeData> _terrainArchetypes;

    private TerrainData? _currentTerrainData;
    private string? _currentStyle;
    private string? _currentFolder;

    private bool _rotate;
    private bool _flipHorizontally;
    private bool _flipVertically;

    private bool _settingDataForGroup;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$TERRAIN";

    public TerrainReader(
        Dictionary<string, TerrainArchetypeData> terrainArchetypes,
        List<TerrainData> allTerrainData)
    {
        _terrainArchetypes = terrainArchetypes;
        _allTerrainData = allTerrainData;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentTerrainData = new TerrainData();
        _rotate = false;
        _flipHorizontally = false;
        _flipVertically = false;

        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

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
                    currentTerrainData.GroupName = secondToken.ToString();
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

            case "WIDTH":
                currentTerrainData.Width = int.Parse(secondToken);
                break;

            case "HEIGHT":
                currentTerrainData.Height = int.Parse(secondToken);
                break;

            case "NO_OVERWRITE":
                currentTerrainData.NoOverwrite = true;
                break;

            case "ONE_WAY":
                break;

            case "ROTATE":
                _rotate = true;
                break;

            case "FLIP_HORIZONTAL":
                _flipHorizontally = true;
                break;

            case "FLIP_VERTICAL":
                _flipVertically = true;
                break;

            case "ERASE":
                currentTerrainData.Erase = true;
                break;

            case "$END":
                var (rotNum, flip) = DihedralTransformation.Simplify(_flipHorizontally, _flipVertically, _rotate);
                currentTerrainData.RotNum = rotNum;
                currentTerrainData.Flip = flip;

                _allTerrainData.Add(currentTerrainData);
                _currentTerrainData = null;
                _settingDataForGroup = false;
                FinishedReading = true;
                break;

            default:
                NxlvReadingHelpers.ThrowUnknownTokenException("Gadget Archetype Data", firstToken, line);
                break;
        }

        return false;
    }

    private void SetCurrentStyle(ReadOnlySpan<char> style)
    {
        _currentStyle = style.ToString();
        _currentFolder = Path.Combine(
            RootDirectoryManager.RootDirectory,
            NeoLemmixFileExtensions.StyleFolderName,
            _currentStyle,
            NeoLemmixFileExtensions.TerrainFolderName);
    }

    private TerrainArchetypeData GetOrLoadTerrainArchetypeData(ReadOnlySpan<char> piece)
    {
        ref var terrainArchetypeData = ref NxlvReadingHelpers.GetArchetypeDataRef(
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

        var dataReaders = new INeoLemmixDataReader[]
        {
            new TerrainArchetypeDataReader(terrainArchetypeData)
        };

        using var dataReaderList = new DataReaderList(rootFilePath, dataReaders);
        dataReaderList.ReadFile();
    }
}