using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.TerrainReaders;

public sealed class TerrainReader : NeoLemmixDataReader
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

    public TerrainReader(
        Dictionary<string, TerrainArchetypeData> terrainArchetypes,
        List<TerrainData> allTerrainData)
        : base("$TERRAIN")
    {
        _terrainArchetypes = terrainArchetypes;
        _allTerrainData = allTerrainData;

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
        _currentTerrainData = new TerrainData();
        _rotate = false;
        _flipHorizontally = false;
        _flipVertically = false;

        FinishedReading = false;
        return false;
    }

    private void SetStyle(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
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
    }

    private void SetPiece(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        if (_settingDataForGroup)
        {
            _currentTerrainData!.GroupName = secondToken.ToString();
        }
        else
        {
            var terrainArchetypeData = GetOrLoadTerrainArchetypeData(line.TrimAfterIndex(secondTokenIndex));
            _currentTerrainData!.TerrainArchetypeId = terrainArchetypeData.TerrainArchetypeId;
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
        _rotate = true;
    }

    private void SetFlipHorizontal(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _flipHorizontally = true;
    }

    private void SetFlipVertical(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _flipVertically = true;
    }

    private void SetErase(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTerrainData!.Erase = true;
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var (rotNum, flip) = DihedralTransformation.Simplify(_flipHorizontally, _flipVertically, _rotate);
        _currentTerrainData!.RotNum = rotNum;
        _currentTerrainData.Flip = flip;

        _allTerrainData.Add(_currentTerrainData);
        _currentTerrainData = null;
        _settingDataForGroup = false;
        FinishedReading = true;
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
        var currentStyleLength = _currentStyle!.Length;

        // Safeguard against potential stack overflow.
        // Will almost certainly be a small buffer
        // allocated on the stack, but still...
        var bufferSize = currentStyleLength + piece.Length + 1;
        Span<char> archetypeDataKeySpan = bufferSize > NxlvReadingHelpers.MaxStackallocSize
            ? new char[bufferSize]
            : stackalloc char[bufferSize];

        _currentStyle.CopyTo(archetypeDataKeySpan);
        archetypeDataKeySpan[currentStyleLength] = ':';
        piece.CopyTo(archetypeDataKeySpan[(currentStyleLength + 1)..]);

        var alternateLookup = _terrainArchetypes.GetAlternateLookup<ReadOnlySpan<char>>();

        ref var dictionaryEntry = ref CollectionsMarshal.GetValueRefOrAddDefault(
            alternateLookup,
            archetypeDataKeySpan,
            out var exists);

        if (!exists)
        {
            dictionaryEntry = CreateNewTerrainArchetypeData(piece);
        }

        return dictionaryEntry!;
    }

    private TerrainArchetypeData CreateNewTerrainArchetypeData(ReadOnlySpan<char> piece)
    {
        var terrainPiece = piece.ToString();
        var terrainArchetypeData = new TerrainArchetypeData
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

        var dataReaders = new NeoLemmixDataReader[]
        {
            new TerrainArchetypeDataReader(terrainArchetypeData)
        };

        using var dataReaderList = new DataReaderList(rootFilePath, dataReaders);
        dataReaderList.ReadFile();
    }
}