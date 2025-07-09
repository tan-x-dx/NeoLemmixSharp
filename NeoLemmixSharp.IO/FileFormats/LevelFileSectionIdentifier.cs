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
}

internal readonly struct LevelFileSectionIdentifierHasher :
    ISectionIdentifierHelper<LevelFileSectionIdentifier>
{
    private const int NumberOfEnumValues = 10;

    public int NumberOfItems => NumberOfEnumValues;

    [Pure]
    public int Hash(LevelFileSectionIdentifier item) => (int)item;
    [Pure]
    public LevelFileSectionIdentifier UnHash(int index) => (LevelFileSectionIdentifier)index;

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();

    public static LevelFileSectionIdentifier GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LevelFileSectionIdentifier>(rawValue, NumberOfEnumValues);

    public static ushort GetSectionIdentifier(LevelFileSectionIdentifier sectionIdentifier) => sectionIdentifier switch
    {
        LevelFileSectionIdentifier.StringDataSection => 0x2644,
        LevelFileSectionIdentifier.LevelMetadataSection => 0x79A6,
        LevelFileSectionIdentifier.LevelTextDataSection => 0x43AA,
        LevelFileSectionIdentifier.LevelObjectivesDataSection => 0x90D2,
        LevelFileSectionIdentifier.TribeDataSection => 0xBEF4,
        LevelFileSectionIdentifier.HatchGroupDataSection => 0xFE77,
        LevelFileSectionIdentifier.PrePlacedLemmingDataSection => 0x60BB,
        LevelFileSectionIdentifier.TerrainDataSection => 0x7C5C,
        LevelFileSectionIdentifier.TerrainGroupDataSection => 0x3D98,
        LevelFileSectionIdentifier.GadgetDataSection => 0x2FCD,

        _ => Helpers.ThrowUnknownEnumValueException<LevelFileSectionIdentifier, ushort>(sectionIdentifier)
    };
}
