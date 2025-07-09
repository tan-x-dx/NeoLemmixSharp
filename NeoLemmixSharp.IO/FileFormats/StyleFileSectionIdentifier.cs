using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.IO.FileFormats;

internal enum StyleFileSectionIdentifier
{
    StringDataSection,
    ThemeDataSection,
    TerrainArchetypeDataSection,
    GadgetArchetypeDataSection,
}

internal readonly struct StyleFileSectionIdentifierHasher :
    ISectionIdentifierHelper<StyleFileSectionIdentifier>
{
    private const int NumberOfEnumValues = 4;

    public int NumberOfItems => NumberOfEnumValues;

    [Pure]
    public int Hash(StyleFileSectionIdentifier item) => (int)item;
    [Pure]
    public StyleFileSectionIdentifier UnHash(int index) => (StyleFileSectionIdentifier)index;

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();

    public static StyleFileSectionIdentifier GetEnumValue(uint rawValue) => Helpers.GetEnumValue<StyleFileSectionIdentifier>(rawValue, NumberOfEnumValues);

    public static ushort GetSectionIdentifier(StyleFileSectionIdentifier sectionIdentifier) => sectionIdentifier switch
    {
        StyleFileSectionIdentifier.StringDataSection => 0x9B70,
        StyleFileSectionIdentifier.ThemeDataSection => 0x35BF,
        StyleFileSectionIdentifier.TerrainArchetypeDataSection => 0x1A47,
        StyleFileSectionIdentifier.GadgetArchetypeDataSection => 0x8C92,

        _ => Helpers.ThrowUnknownEnumValueException<StyleFileSectionIdentifier, ushort>(sectionIdentifier)
    };
}
