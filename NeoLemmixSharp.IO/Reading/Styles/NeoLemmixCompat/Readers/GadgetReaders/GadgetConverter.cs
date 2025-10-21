using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.GadgetReaders;

internal static class GadgetConverter
{
    public static GadgetArchetypeData ConvertToGadgetArchetypeData(NeoLemmixGadgetArchetypeData neoLemmixGadgetArchetypeData)
    {

        var result = new GadgetArchetypeData
        {
            StyleIdentifier = neoLemmixGadgetArchetypeData.StyleIdentifier,
            PieceIdentifier = neoLemmixGadgetArchetypeData.GadgetPieceIdentifier,
            GadgetName = new GadgetName(neoLemmixGadgetArchetypeData.GadgetPieceIdentifier.ToString()),
            BaseSpriteSize = default,
            SpecificationData = default,
        };

        return result;
    }
}
