using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Data.Style;

internal sealed class StyleData
{
    private int _numberOfLevelsSinceLastUsed;
    internal ref int NumberOfLevelsSinceLastUsed => ref _numberOfLevelsSinceLastUsed;

    internal StyleIdentifier Identifier { get; }
    internal FileFormatType FileFormatType { get; }

    internal StyleData(StyleIdentifier identifier, FileFormatType fileFormatType)
    {
        Identifier = identifier;
        FileFormatType = fileFormatType;
    }

    internal Dictionary<PieceIdentifier, TerrainArchetypeData> TerrainArchetypeData { get; } = new(EngineConstants.AssumedNumberOfTerrainArchetypeDataInStyle);
    internal Dictionary<PieceIdentifier, GadgetArchetypeData> GadgetArchetypeData { get; } = new(EngineConstants.AssumedNumberOfGadgetArchetypeDataInStyle);
}
