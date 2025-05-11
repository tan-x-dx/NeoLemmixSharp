using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;

namespace NeoLemmixSharp.IO.Data.Style;

public sealed class StyleData
{
    private int _numberOfLevelsSinceLastUsed;
    public ref int NumberOfLevelsSinceLastUsed => ref _numberOfLevelsSinceLastUsed;

    public StyleIdentifier Identifier { get; }

    public StyleData(StyleIdentifier identifier)
    {
        Identifier = identifier;
    }

    public Dictionary<PieceIdentifier, TerrainArchetypeData> TerrainArchetypeData { get; } = new(EngineConstants.AssumedNumberOfTerrainArchetypeDataInStyle);
    public Dictionary<PieceIdentifier, GadgetArchetypeData> GadgetArchetypeData { get; } = [];
}
