using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum LevelTimerObservationType
{
    SecondsElapsed,
    SecondsRemaining,

    VALUE_MAX
}

public static class LevelTimerObservationTypeHelpers
{
    private const int NumberOfEnumValues = (int)LevelTimerObservationType.VALUE_MAX;

    public static LevelTimerObservationType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LevelTimerObservationType>(rawValue, NumberOfEnumValues);
}
