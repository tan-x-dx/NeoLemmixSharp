using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Data.Level;

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

public readonly struct LevelParameterHasher : IEnumIdentifierHelper<LevelParameters, BitBuffer32>
{
    private const int NumberOfEnumValues = 8;

    public static LevelParameters GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LevelParameters>(rawValue, NumberOfEnumValues);

    public int NumberOfItems => NumberOfEnumValues;

    [Pure]
    public int Hash(LevelParameters item) => (int)item;
    [Pure]
    public LevelParameters UnHash(int index) => (LevelParameters)index;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArraySet<LevelParameterHasher, BitBuffer32, LevelParameters> CreateBitArraySet() => new(new LevelParameterHasher());
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArrayDictionary<LevelParameterHasher, BitBuffer32, LevelParameters, TValue> CreateBitArrayDictionary<TValue>() => new(new LevelParameterHasher());

    public void CreateBitBuffer(int numberOfItems, out BitBuffer32 buffer) => buffer = new();
}
