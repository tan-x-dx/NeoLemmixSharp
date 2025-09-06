using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum LevelTimerObservationType
{
    SecondsElapsed,
    SecondsRemaining
}

public static class LevelTimerObservationTypeHelpers
{
    private const int NumberOfEnumValues = 2;

    public static LevelTimerObservationType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LevelTimerObservationType>(rawValue, NumberOfEnumValues);
}
