using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.TerrainReaders;

public sealed class TerrainArchetypeDataReader : INeoLemmixDataReader
{
    private readonly TerrainArchetypeData _terrainArchetypeData;

    public bool FinishedReading => false;
    public string IdentifierToken => string.Empty;

    public TerrainArchetypeDataReader(TerrainArchetypeData terrainArchetypeData)
    {
        _terrainArchetypeData = terrainArchetypeData;
    }

    public bool MatchesToken(ReadOnlySpan<char> token)
    {
        // Always choose this reader when dealing with terrain archetype data
        return true;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        ReadNextLine(line);
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        ReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

        switch (firstToken)
        {
            case "STEEL":
                _terrainArchetypeData.IsSteel = true;
                break;

            case "RESIZE_HORIZONTAL":
                _terrainArchetypeData.ResizeType |= ResizeType.ResizeHorizontal;
                break;

            case "RESIZE_VERTICAL":
                _terrainArchetypeData.ResizeType |= ResizeType.ResizeVertical;
                break;

            case "RESIZE_BOTH":
                _terrainArchetypeData.ResizeType = ResizeType.ResizeBoth;
                break;

            case "NINE_SLICE_LEFT":
                _terrainArchetypeData.ResizeType |= ResizeType.ResizeHorizontal;
                _terrainArchetypeData.NineSliceLeft = int.Parse(secondToken);
                break;

            case "NINE_SLICE_TOP":
                _terrainArchetypeData.ResizeType |= ResizeType.ResizeVertical;
                _terrainArchetypeData.NineSliceTop = int.Parse(secondToken);
                break;

            case "NINE_SLICE_RIGHT":
                _terrainArchetypeData.ResizeType |= ResizeType.ResizeHorizontal;
                _terrainArchetypeData.NineSliceRight = int.Parse(secondToken);
                break;

            case "NINE_SLICE_BOTTOM":
                _terrainArchetypeData.ResizeType |= ResizeType.ResizeVertical;
                _terrainArchetypeData.NineSliceBottom = int.Parse(secondToken);
                break;

            case "DEFAULT_WIDTH":
                _terrainArchetypeData.DefaultWidth = int.Parse(secondToken);
                break;

            case "DEFAULT_HEIGHT":
                _terrainArchetypeData.DefaultHeight = int.Parse(secondToken);
                break;

            default:
                ReadingHelpers.ThrowUnknownTokenException("Terrain Archetype Data", firstToken, line);
                break;
        }

        return false;
    }
}