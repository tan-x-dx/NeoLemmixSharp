using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.LevelIo;

public enum StyleFileSectionIdentifier
{
    TerrainArchetypeDataSection,
    GadgetArchetypeDataSection,
}

public readonly struct StyleFileSectionIdentifierHasher :
    IPerfectHasher<StyleFileSectionIdentifier>,
    IBitBufferCreator<BitBuffer32>,
    IEnumVerifier<StyleFileSectionIdentifier>
{
    public int NumberOfItems => 2;

    [Pure]
    public int Hash(StyleFileSectionIdentifier item) => (int)item;
    [Pure]
    public StyleFileSectionIdentifier UnHash(int index) => (StyleFileSectionIdentifier)index;

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();

    public static StyleFileSectionIdentifier GetEnumValue(int rawValue)
    {
        var enumValue = (StyleFileSectionIdentifier)rawValue;

        return enumValue switch
        {
            StyleFileSectionIdentifier.TerrainArchetypeDataSection => StyleFileSectionIdentifier.TerrainArchetypeDataSection,
            StyleFileSectionIdentifier.GadgetArchetypeDataSection => StyleFileSectionIdentifier.GadgetArchetypeDataSection,

            _ => Helpers.ThrowUnknownEnumValueException<StyleFileSectionIdentifier>(rawValue)
        };
    }
}