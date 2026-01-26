using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level;

public static class LevelParameterHelpers
{
    public static uint GetLemmingCountDownTimer(this LevelParameterSet parameters, Lemming lemming)
    {
        var timedBombers = parameters.Contains(LevelParameters.TimedBombers);

        uint result = 1;
        if (!timedBombers)
            return result; // I.e. the next frame

        result = EngineConstants.DefaultCountDownActionTicks;
        if (lemming.IsFastForward)
            result = EngineConstants.DefaultFastForwardLemmingCountDownActionTicks;

        return result;
    }
}
