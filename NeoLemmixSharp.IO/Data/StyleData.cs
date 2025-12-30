using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Data;

public sealed class StyleData
{
    internal int NumberOfLevelsSinceLastUsed;

    public StyleIdentifier Identifier { get; }
    public string Name { get; internal set; } = string.Empty;
    public string Author { get; internal set; } = string.Empty;
    public string Description { get; internal set; } = string.Empty;
    public FileFormatType FileFormatType { get; }

    internal StyleData(
        StyleIdentifier identifier,
        FileFormatType fileFormatType)
    {
        Identifier = identifier;
        FileFormatType = fileFormatType;
    }

    public ThemeData ThemeData { get; internal set; } = null!;
    internal Dictionary<PieceIdentifier, TerrainArchetypeData> TerrainArchetypeDataLookup { get; } = new(IoConstants.AssumedNumberOfTerrainArchetypeDataInStyle);
    internal Dictionary<PieceIdentifier, GadgetArchetypeData> GadgetArchetypeDataLookup { get; } = new(IoConstants.AssumedNumberOfGadgetArchetypeDataInStyle);

    public Dictionary<PieceIdentifier, TerrainArchetypeData>.ValueCollection AllDefinedTerrainArchetypeData => TerrainArchetypeDataLookup.Values;
    public Dictionary<PieceIdentifier, GadgetArchetypeData>.ValueCollection AllDefinedGadgetArchetypeData => GadgetArchetypeDataLookup.Values;
}
