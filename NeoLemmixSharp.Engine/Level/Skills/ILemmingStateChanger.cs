﻿using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Skills;

public interface ILemmingStateChanger
{
    [Pure]
    int LemmingStateChangerId { get; }

    void SetLemmingState(LemmingState lemmingState, bool status);
    void ToggleLemmingState(LemmingState lemmingState);

    [Pure]
    bool IsApplied(LemmingState lemmingState);
}

public readonly struct LemmingStateChangerHasher : IPerfectHasher<ILemmingStateChanger>, IBitBufferCreator<BitBuffer32>
{
    private const int NumberOfStateChangers = 11;

    public const int ClimberStateChangerId = 0;
    public const int FloaterStateChangerId = 1;
    public const int GliderStateChangerId = 2;
    public const int SwimmerStateChangerId = 3;
    public const int DisarmerStateChangerId = 4;
    public const int SliderStateChangerId = 5;

    public const int ZombieStateChangerId = 6;
    public const int NeutralStateChangerId = 7;
    public const int AcidLemmingStateChangerId = 8;
    public const int WaterStateChangerId = 9;
    public const int FastForwardStateChangerId = 10;

    private static readonly ILemmingStateChanger[] AllLemmingStateChangers = GetLemmingStateChangers();

    private static ILemmingStateChanger[] GetLemmingStateChangers()
    {
        var result = new ILemmingStateChanger[]
        {
            ClimberSkill.Instance,
            FloaterSkill.Instance,
            GliderSkill.Instance,
            SwimmerSkill.Instance,
            DisarmerSkill.Instance,
            SliderSkill.Instance,

            ZombieStateChanger.Instance,
            NeutralStateChanger.Instance,

            AcidLemmingSkill.Instance,
            WaterLemmingSkill.Instance,

            FastForwardSkill.Instance
        };

        if (result.Length != NumberOfStateChangers)
            throw new Exception($"Number of ILemmingStateChanger is actually {result.Length}! Update {nameof(NumberOfStateChangers)}!");

        var hasher = new LemmingStateChangerHasher();
        hasher.ValidateUniqueIds(new ReadOnlySpan<ILemmingStateChanger>(result));
        Array.Sort(result, hasher);

        return result;
    }

    public int NumberOfItems => NumberOfStateChangers;
    public int Hash(ILemmingStateChanger item) => item.LemmingStateChangerId;
    public ILemmingStateChanger UnHash(int index) => AllLemmingStateChangers[index];

    public static StateChangerSet CreateBitArraySet() => new(new LemmingStateChangerHasher(), false);

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
}