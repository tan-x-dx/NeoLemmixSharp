using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitBuffers;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;

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

public readonly struct LevelParameterHasher : IPerfectHasher<LevelParameters>, IBitBufferCreator<BitBuffer32>
{
    public int NumberOfItems => 8;

    [Pure]
    public int Hash(LevelParameters item) => (int)item;
    [Pure]
    public LevelParameters UnHash(int index) => (LevelParameters)index;

    [Pure]
    public static LevelParameterSet CreateBitArraySet(bool fullSet = false) => new(new LevelParameterHasher(), fullSet);
    [Pure]
    public static BitArrayDictionary<LevelParameterHasher, BitBuffer32, LevelParameters, TValue> CreateBitArrayDictionary<TValue>() => new(new LevelParameterHasher());

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
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