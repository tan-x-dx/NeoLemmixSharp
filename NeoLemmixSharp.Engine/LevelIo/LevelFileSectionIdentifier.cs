using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.LevelIo.Writing;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.LevelIo;

public enum LevelFileSectionIdentifier
{
    StringDataSection,
    LevelMetadataSection,
    LevelTextDataSection,
    HatchGroupDataSection,
    LevelObjectivesDataSection,
    PrePlacedLemmingDataSection,
    TerrainDataSection,
    TerrainGroupDataSection,
    GadgetDataSection,
}

public readonly struct LevelFileSectionIdentifierHasher :
    ISectionIdentifierHelper<LevelFileSectionIdentifier>
{
    public const int NumberOfBytesForLevelSectionIdentifier = 2;

    private const int NumberOfEnumValues = 9;

    public int NumberOfItems => NumberOfEnumValues;

    [Pure]
    public int Hash(LevelFileSectionIdentifier item) => (int)item;
    [Pure]
    public LevelFileSectionIdentifier UnHash(int index) => (LevelFileSectionIdentifier)index;

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();

    public static LevelFileSectionIdentifier GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LevelFileSectionIdentifier>(rawValue, NumberOfEnumValues);

    public static ReadOnlySpan<byte> GetSectionIdentifierBytes(LevelFileSectionIdentifier sectionIdentifier)
    {
        var index = (int)sectionIdentifier;
        index <<= 1;

        return LevelDataSectionIdentifierBytes
            .Slice(index, NumberOfBytesForLevelSectionIdentifier);
    }

    private static ReadOnlySpan<byte> LevelDataSectionIdentifierBytes =>
    [
        0x26, 0x44,
        0x79, 0xA6,
        0x43, 0xAA,
        0x90, 0xD2,
        0xBE, 0xF4,
        0xFE, 0x77,
        0x60, 0xBB,
        0x7C, 0x5C,
        0x3D, 0x98
    ];
}
