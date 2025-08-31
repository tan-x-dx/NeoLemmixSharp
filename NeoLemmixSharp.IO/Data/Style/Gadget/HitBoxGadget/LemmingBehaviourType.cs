using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;

public enum LemmingBehaviourType
{
    None,
    SetLemmingState,
    ClearLemmingStates,
    SetLemmingAction,
    KillLemming,
    ForceLemmingFacingDirection,
    NullifyLemmingFallDistance,
    LemmingMover
}

public static class LemmingBehaviourTypeHelpers
{
    private const int NumberOfEnumValues = 8;

    public static LemmingBehaviourType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingBehaviourType>(rawValue, NumberOfEnumValues);
}
