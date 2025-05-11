using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArraySet<LevelParameterHasher, BitBuffer32, LevelParameters> CreateBitArraySet(bool fullSet = false) => new(new LevelParameterHasher(), fullSet);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArrayDictionary<LevelParameterHasher, BitBuffer32, LevelParameters, TValue> CreateBitArrayDictionary<TValue>() => new(new LevelParameterHasher());

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
}
