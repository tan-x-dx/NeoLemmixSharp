﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level;

public enum LevelParameters
{
    TimedBombers,
    EnablePause,
    EnableNuke,
    EnableFastForward,
    EnableDirectionSelect,
    EnableClearPhysics,
    EnableSkillShadows,
    EnableFrameControl
}

public static class LevelParameterHelpers
{
    public static int GetLemmingCountDownTimer(this LevelParameterSet parameters, Lemming lemming)
    {
        var timedBombers = parameters.Contains(LevelParameters.TimedBombers);

        if (timedBombers)
            return lemming.IsFastForward
                ? EngineConstants.DefaultFastForwardLemmingCountDownActionTicks
                : EngineConstants.DefaultCountDownActionTicks;

        return 1; // I.e. the next frame
    }
}