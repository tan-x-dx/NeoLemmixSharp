using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.IO.FileFormats;

internal enum LevelFileSectionIdentifier
{
    StringDataSection,
    LevelMetadataSection,
    LevelTextDataSection,
    LevelObjectivesDataSection,
    TribeDataSection,
    HatchGroupDataSection,
    PrePlacedLemmingDataSection,
    TerrainDataSection,
    TerrainGroupDataSection,
    GadgetDataSection,
    GadgetBehaviourDataSection,
    GadgetTriggerDataSection,

    VALUE_MAX
}

internal readonly struct LevelFileSectionIdentifierHasher : IEnumIdentifierHelper<BitBuffer32, LevelFileSectionIdentifier>
{
    private const int NumberOfEnumValues = (int)LevelFileSectionIdentifier.VALUE_MAX;
    private const int IdentifierBytesMultiplier = (429 * 8) + 5; // Needs to be === 5 mod 8
    private const int IdentifierBytesIncrement = 29879; // Prime

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
    public int Hash(LevelFileSectionIdentifier item) => (int)item;
    [Pure]
    public LevelFileSectionIdentifier UnHash(int index) => (LevelFileSectionIdentifier)index;

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();

    public static LevelFileSectionIdentifier GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LevelFileSectionIdentifier>(rawValue, NumberOfEnumValues);

    public static ushort GetSectionIdentifierBytes(LevelFileSectionIdentifier sectionIdentifier)
    {
        if ((uint)sectionIdentifier < NumberOfEnumValues)
            return SectionIdentifierBytes.At((int)sectionIdentifier);

        return Helpers.ThrowUnknownEnumValueException<LevelFileSectionIdentifier, ushort>(sectionIdentifier);
    }
}
