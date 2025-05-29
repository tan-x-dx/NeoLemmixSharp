using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Skills;

public interface ILemmingStateChanger
{
    private const int NumberOfStateChangers = 11;

    public enum StateChangerType
    {
        ClimberStateChanger,
        FloaterStateChanger,
        GliderStateChanger,
        SwimmerStateChanger,
        DisarmerStateChanger,
        SliderStateChanger,

        AcidLemmingStateChanger,
        WaterStateChanger,
        FastForwardStateChanger,

        NeutralStateChanger,
        ZombieStateChanger,
    }

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

            AcidLemmingSkill.Instance,
            WaterLemmingSkill.Instance,
            FastForwardSkill.Instance,

            NeutralStateChanger.Instance,
            ZombieStateChanger.Instance,
        };

        if (result.Length != NumberOfStateChangers)
            throw new Exception($"Number of ILemmingStateChangers is actually {result.Length}! Update {nameof(NumberOfStateChangers)}!");

        var hasher = new LemmingStateChangerHasher();
        hasher.AssertUniqueIds(new ReadOnlySpan<ILemmingStateChanger>(result));
        Array.Sort(result, hasher);

        return result;
    }

    [Pure]
    StateChangerType LemmingStateChangerType { get; }

    void SetLemmingState(LemmingState lemmingState, bool status);
    void ToggleLemmingState(LemmingState lemmingState);

    [Pure]
    bool IsApplied(LemmingState lemmingState);

    public static StateChangerSet CreateBitArraySet() => new(new LemmingStateChangerHasher());

    public readonly struct LemmingStateChangerHasher : IPerfectHasher<ILemmingStateChanger>, IBitBufferCreator<BitBuffer32>
    {
        public int NumberOfItems => NumberOfStateChangers;
        public int Hash(ILemmingStateChanger item) => (int)item.LemmingStateChangerType;
        public ILemmingStateChanger UnHash(int index) => AllLemmingStateChangers[index];

        public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
    }
}