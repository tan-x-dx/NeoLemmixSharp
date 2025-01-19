using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.TerrainReaders;

public sealed class TerrainArchetypeDataReader : NeoLemmixDataReader
{
    private readonly TerrainArchetypeData _terrainArchetypeData;

    public TerrainArchetypeDataReader(
        TerrainArchetypeData terrainArchetypeData)
        : base(string.Empty)
    {
        _terrainArchetypeData = terrainArchetypeData;

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
        _terrainArchetypeData.IsSteel = true;
    }

    private void SetResizeHorizontal(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _terrainArchetypeData.ResizeType |= ResizeType.ResizeHorizontal;
    }

    private void SetResizeVertical(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _terrainArchetypeData.ResizeType |= ResizeType.ResizeVertical;
    }

    private void SetResizeBoth(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _terrainArchetypeData.ResizeType = ResizeType.ResizeBoth;
    }

    private void SetNineSliceLeft(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _terrainArchetypeData.ResizeType |= ResizeType.ResizeHorizontal;
        _terrainArchetypeData.NineSliceLeft = int.Parse(secondToken);
    }

    private void SetNineSliceTop(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _terrainArchetypeData.ResizeType |= ResizeType.ResizeVertical;
        _terrainArchetypeData.NineSliceTop = int.Parse(secondToken);
    }

    private void SetNineSliceRight(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _terrainArchetypeData.ResizeType |= ResizeType.ResizeHorizontal;
        _terrainArchetypeData.NineSliceRight = int.Parse(secondToken);
    }

    private void SetNineSliceBottom(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _terrainArchetypeData.ResizeType |= ResizeType.ResizeVertical;
        _terrainArchetypeData.NineSliceBottom = int.Parse(secondToken);
    }

    private void SetDefaultWidth(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _terrainArchetypeData.DefaultWidth = int.Parse(secondToken);
    }

    private void SetDefaultHeight(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _terrainArchetypeData.DefaultHeight = int.Parse(secondToken);
    }
}