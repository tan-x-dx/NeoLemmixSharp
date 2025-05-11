using NeoLemmixSharp.Engine.LevelIo.Data.Style.Gadget;
using NeoLemmixSharp.Engine.LevelIo.Data.Style.Terrain;

namespace NeoLemmixSharp.Engine.LevelIo.Data.Style;

public sealed class StyleData
{
    private int _numberOfLevelsSinceLastUsed;
    public ref int NumberOfLevelsSinceLastUsed => ref _numberOfLevelsSinceLastUsed;

    public StyleIdentifier Identifier { get; }

    public StyleData(StyleIdentifier identifier)
    {
        Identifier = identifier;
    }

    public Dictionary<PieceIdentifier, TerrainArchetypeData> TerrainArchetypeData { get; } = [];
    public Dictionary<PieceIdentifier, GadgetArchetypeData> GadgetArchetypeData { get; } = [];
}
