global using LevelParameterSet = NeoLemmixSharp.Common.Util.Collections.SimpleSet<NeoLemmixSharp.Engine.Level.LevelParameters>;
using NeoLemmixSharp.Common.Util.Collections;
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
    EnableFrameControl,
}

public static class LevelParameterHelpers
{
    private const int NumberOfLevelParameters = 8;

    private sealed class LevelParametersHasher : IPerfectHasher<LevelParameters>
    {
        public int NumberOfItems => NumberOfLevelParameters;

        public int Hash(LevelParameters item) => (int)item;

        public LevelParameters UnHash(int index) => (LevelParameters)index;
    }

    public static LevelParameterSet CreateSimpleSet() => new(new LevelParametersHasher(), false);

    public static int GetLemmingCountDownTimer(this LevelParameterSet parameters, Lemming lemming)
    {
        var timedBombers = parameters.Contains(LevelParameters.TimedBombers);

        if (timedBombers)
            return lemming.IsFastForward
                ? LevelConstants.DefaultFastForwardLemmingCountDownActionTicks
                : LevelConstants.DefaultCountDownActionTicks;

        return 1; // I.e. the next frame
    }
}