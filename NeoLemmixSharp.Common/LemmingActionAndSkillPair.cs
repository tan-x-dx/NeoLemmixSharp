using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

public readonly record struct LemmingActionAndSkillPair(LemmingActionType ActionType, LemmingSkillType SkillType)
{
    public const int NumberOfLemmingActionAndSkillPairs = LemmingActionConstants.NumberOfLemmingActions * LemmingSkillConstants.NumberOfLemmingSkills;
}

public readonly struct LemmingActionAndSkillHasher : IBitBufferCreator<LemmingActionAndSkillPairBitBuffer, LemmingActionAndSkillPair>
{
    [Pure]
    public int NumberOfItems => LemmingActionAndSkillPair.NumberOfLemmingActionAndSkillPairs;

    [Pure]
    public int Hash(LemmingActionAndSkillPair item)
    {
        var result = (int)item.ActionType;
        result *= LemmingSkillConstants.NumberOfLemmingSkills;
        result += (int)item.SkillType;
        return result;
    }

    [Pure]
    public LemmingActionAndSkillPair UnHash(int index)
    {
        var (actionTypeInt, skillTypeInt) = Math.DivRem((uint)index, LemmingSkillConstants.NumberOfLemmingSkills);

        return new LemmingActionAndSkillPair((LemmingActionType)actionTypeInt, (LemmingSkillType)skillTypeInt);
    }

    public void CreateBitBuffer(out LemmingActionAndSkillPairBitBuffer buffer) => buffer = new();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArraySet<LemmingActionAndSkillHasher, LemmingActionAndSkillPairBitBuffer, LemmingActionAndSkillPair> CreateBitArraySet() => new(new LemmingActionAndSkillHasher());
}

[InlineArray(LemmingActionAndSkillPairBitBufferLength)]
public struct LemmingActionAndSkillPairBitBuffer : IBitBuffer
{
    private const int LemmingActionAndSkillPairBitBufferLength = (LemmingActionAndSkillPair.NumberOfLemmingActionAndSkillPairs + BitArrayHelpers.Mask) >>> BitArrayHelpers.Shift;

    private uint _0;

    public readonly int Length => LemmingActionAndSkillPairBitBufferLength;

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, LemmingActionAndSkillPairBitBufferLength);
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, LemmingActionAndSkillPairBitBufferLength);
}
