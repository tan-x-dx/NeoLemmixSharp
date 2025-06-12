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
    public const int NumberOfBytesForStyleSectionIdentifier = 2;

    private const int NumberOfEnumValues = 4;

    public int NumberOfItems => NumberOfEnumValues;

    [Pure]
    public int Hash(StyleFileSectionIdentifier item) => (int)item;
    [Pure]
    public StyleFileSectionIdentifier UnHash(int index) => (StyleFileSectionIdentifier)index;

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();

    public static StyleFileSectionIdentifier GetEnumValue(uint rawValue) => Helpers.GetEnumValue<StyleFileSectionIdentifier>(rawValue, NumberOfEnumValues);

    public static ReadOnlySpan<byte> GetSectionIdentifierBytes(StyleFileSectionIdentifier sectionIdentifier)
    {
        var index = (int)sectionIdentifier;
        index *= NumberOfBytesForStyleSectionIdentifier;

        return StyleDataSectionIdentifierBytes
            .Slice(index, NumberOfBytesForStyleSectionIdentifier);
    }

    private static ReadOnlySpan<byte> StyleDataSectionIdentifierBytes =>
    [
        0x9B, 0x70,
        0x35, 0xBF,
        0x1A, 0x47,
        0x8C, 0x92
    ];
}