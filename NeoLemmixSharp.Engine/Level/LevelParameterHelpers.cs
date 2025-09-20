using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level;

public static class LevelParameterHelpers
{
    public static uint GetLemmingCountDownTimer(this LevelParameterSet parameters, Lemming lemming)
    {
        var timedBombers = parameters.Contains(LevelParameters.TimedBombers);

        if (timedBombers)
        {
            if (lemming.IsFastForward)
                return EngineConstants.DefaultFastForwardLemmingCountDownActionTicks;

            return EngineConstants.DefaultCountDownActionTicks;
        }

        return 1; // I.e. the next frame
    }
}
