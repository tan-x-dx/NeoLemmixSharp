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

    VALUE_MAX
}

internal readonly struct StyleFileSectionIdentifierHasher : IEnumIdentifierHelper<BitBuffer32, StyleFileSectionIdentifier>
{
    private const int NumberOfEnumValues = (int)StyleFileSectionIdentifier.VALUE_MAX;
    private const int IdentifierBytesMultiplier = (716 * 8) + 5; // Needs to be === 5 mod 8
    private const int IdentifierBytesIncrement = 51631; // Prime

    private static readonly ushort[] SectionIdentifierBytes = GenerateSectionIdentifierBytes();

    private static ushort[] GenerateSectionIdentifierBytes()
    {
        var result = new ushort[NumberOfEnumValues];

        var lcg = new LinearCongruentialGenerator<LcgModulo65536>(IdentifierBytesMultiplier, IdentifierBytesIncrement);

        for (var i = 0; i < NumberOfEnumValues; i++)
        {
            lcg.MoveNext();

            result[i] = (ushort)lcg.Current;
        }

        return result;
    }

    public int NumberOfItems => NumberOfEnumValues;

    [Pure]
    public int Hash(StyleFileSectionIdentifier item) => (int)item;
    [Pure]
    public StyleFileSectionIdentifier UnHash(int index) => (StyleFileSectionIdentifier)index;

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();

    public static StyleFileSectionIdentifier GetEnumValue(uint rawValue) => Helpers.GetEnumValue<StyleFileSectionIdentifier>(rawValue, NumberOfEnumValues);

    public static ushort GetSectionIdentifierBytes(StyleFileSectionIdentifier sectionIdentifier)
    {
        if ((uint)sectionIdentifier < NumberOfEnumValues)
            return SectionIdentifierBytes.At((int)sectionIdentifier);

        return Helpers.ThrowUnknownEnumValueException<StyleFileSectionIdentifier, ushort>(sectionIdentifier);
    }
}
