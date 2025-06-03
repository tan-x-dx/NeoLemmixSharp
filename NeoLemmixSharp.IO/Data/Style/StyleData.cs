using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Data.Style;

internal sealed class StyleData
{
    internal int NumberOfLevelsSinceLastUsed;

    internal StyleIdentifier Identifier { get; }
    internal FileFormatType FileFormatType { get; }

    internal StyleData(StyleIdentifier identifier, FileFormatType fileFormatType)
    {
        Identifier = identifier;
        FileFormatType = fileFormatType;
    }

    internal ThemeData ThemeData { get; set; } = null!;
    internal Dictionary<PieceIdentifier, TerrainArchetypeData> TerrainArchetypeData { get; } = new(IoConstants.AssumedNumberOfTerrainArchetypeDataInStyle);
    internal Dictionary<PieceIdentifier, GadgetArchetypeData> GadgetArchetypeData { get; } = new(IoConstants.AssumedNumberOfGadgetArchetypeDataInStyle);
}
