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
    public int NumberOfItems => 9;

    [Pure]
    public int Hash(LevelFileSectionIdentifier item) => (int)item;
    [Pure]
    public LevelFileSectionIdentifier UnHash(int index) => (LevelFileSectionIdentifier)index;

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();

    public static LevelFileSectionIdentifier GetEnumValue(int rawValue)
    {
        var enumValue = (LevelFileSectionIdentifier)rawValue;

        return enumValue switch
        {
            LevelFileSectionIdentifier.StringDataSection => LevelFileSectionIdentifier.StringDataSection,
            LevelFileSectionIdentifier.LevelMetadataSection => LevelFileSectionIdentifier.LevelMetadataSection,
            LevelFileSectionIdentifier.LevelTextDataSection => LevelFileSectionIdentifier.LevelTextDataSection,
            LevelFileSectionIdentifier.HatchGroupDataSection => LevelFileSectionIdentifier.HatchGroupDataSection,
            LevelFileSectionIdentifier.LevelObjectivesDataSection => LevelFileSectionIdentifier.LevelObjectivesDataSection,
            LevelFileSectionIdentifier.PrePlacedLemmingDataSection => LevelFileSectionIdentifier.PrePlacedLemmingDataSection,
            LevelFileSectionIdentifier.TerrainDataSection => LevelFileSectionIdentifier.TerrainDataSection,
            LevelFileSectionIdentifier.TerrainGroupDataSection => LevelFileSectionIdentifier.TerrainGroupDataSection,
            LevelFileSectionIdentifier.GadgetDataSection => LevelFileSectionIdentifier.GadgetDataSection,

            _ => Helpers.ThrowUnknownEnumValueException<LevelFileSectionIdentifier>(rawValue)
        };
    }
}
