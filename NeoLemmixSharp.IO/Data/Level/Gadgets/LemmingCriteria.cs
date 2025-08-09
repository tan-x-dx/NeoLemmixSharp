using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets;

public enum LemmingCriteria
{
    LemmingOrientation,
    LemmingFacingDirection,
    LemmingAction,
    LemmingState,
    LemmingTribe
}

public readonly struct LemmingCriteriaHasher : IEnumIdentifierHelper<LemmingCriteria, BitBuffer32>
{
    private const int NumberOfEnumValues = 5;

    public int NumberOfItems => NumberOfEnumValues;
    public int Hash(LemmingCriteria item) => (int)item;
    public LemmingCriteria UnHash(int index) => (LemmingCriteria)index;
    public void CreateBitBuffer(int numberOfItems, out BitBuffer32 buffer) => buffer = new();
    public static LemmingCriteria GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingCriteria>(rawValue, NumberOfEnumValues);

    public static BitArraySet<LemmingCriteriaHasher, BitBuffer32, LemmingCriteria> CreateBitSet() => new(new LemmingCriteriaHasher());
}
