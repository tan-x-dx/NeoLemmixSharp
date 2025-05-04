using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
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
    IPerfectHasher<LevelFileSectionIdentifier>,
    IBitBufferCreator<BitBuffer32>,
    IEnumVerifier<LevelFileSectionIdentifier>
{
    private const int NumberOfEnumValues = 9;

    public int NumberOfItems => NumberOfEnumValues;

    [Pure]
    public int Hash(LevelFileSectionIdentifier item) => (int)item;
    [Pure]
    public LevelFileSectionIdentifier UnHash(int index) => (LevelFileSectionIdentifier)index;

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();

    public LevelFileSectionIdentifier GetEnumValue(int rawValue) => Helpers.GetEnumValue<LevelFileSectionIdentifier>(rawValue, NumberOfEnumValues);
}
