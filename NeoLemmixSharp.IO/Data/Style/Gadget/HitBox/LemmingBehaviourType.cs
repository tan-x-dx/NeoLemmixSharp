using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

public enum LemmingBehaviourType
{
    ChangeLemmingState,
    ChangeLemmingAction,
    KillLemming,
    ForceFacingDirection,
    NullifyFallDistance,
    LemmingMover
}

public static class LemmingBehaviourTypeHelpers
{
    private const int NumberOfEnumValues = 6;

    public static LemmingBehaviourType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingBehaviourType>(rawValue, NumberOfEnumValues);
    public static GadgetBehaviourType ToGadgetBehaviourType(this LemmingBehaviourType type) => type switch
    {
        LemmingBehaviourType.ChangeLemmingState => GadgetBehaviourType.LemmingChangeState,
        LemmingBehaviourType.ChangeLemmingAction => GadgetBehaviourType.LemmingChangeAction,
        LemmingBehaviourType.KillLemming => GadgetBehaviourType.LemmingKill,
        LemmingBehaviourType.ForceFacingDirection => GadgetBehaviourType.LemmingForceFacingDirection,
        LemmingBehaviourType.NullifyFallDistance => GadgetBehaviourType.LemmingNullifyFallDistance,
        LemmingBehaviourType.LemmingMover => GadgetBehaviourType.LemmingMove,

        _ => Helpers.ThrowUnknownEnumValueException<LemmingBehaviourType, GadgetBehaviourType>(type)
    };
}
