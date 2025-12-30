using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Common.Enums;

public enum LemmingCriteriaType
{
    LemmingOrientation,
    LemmingFacingDirection,
    LemmingAction,
    RequiredLemmingState,
    DisallowedLemmingState,
    LemmingTribe
}

public readonly struct LemmingCriteriaTypeHasher : IEnumIdentifierHelper<BitBuffer32, LemmingCriteriaType>
{
    private const int NumberOfEnumValues = 6;

    public int NumberOfItems => NumberOfEnumValues;
    public int Hash(LemmingCriteriaType item) => (int)item;
    public LemmingCriteriaType UnHash(int index) => (LemmingCriteriaType)index;
    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
    public static LemmingCriteriaType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingCriteriaType>(rawValue, NumberOfEnumValues);

    public static BitArraySet<LemmingCriteriaTypeHasher, BitBuffer32, LemmingCriteriaType> CreateBitSet() => new(new LemmingCriteriaTypeHasher());
}
