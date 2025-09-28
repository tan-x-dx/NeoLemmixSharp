using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Data;

internal sealed class StyleData
{
    internal int NumberOfLevelsSinceLastUsed;

    internal StyleIdentifier Identifier { get; }
    internal string Name { get; set; } = string.Empty;
    internal string Author { get; set; } = string.Empty;
    internal string Description { get; set; } = string.Empty;
    internal FileFormatType FileFormatType { get; }

    internal StyleData(
        StyleIdentifier identifier,
        FileFormatType fileFormatType)
    {
        Identifier = identifier;
        FileFormatType = fileFormatType;
    }

    internal ThemeData ThemeData { get; set; } = null!;
    internal Dictionary<PieceIdentifier, TerrainArchetypeData> TerrainArchetypeDataLookup { get; } = new(IoConstants.AssumedNumberOfTerrainArchetypeDataInStyle);
    internal Dictionary<PieceIdentifier, GadgetArchetypeData> GadgetArchetypeDataLookup { get; } = new(IoConstants.AssumedNumberOfGadgetArchetypeDataInStyle);
}
