using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Skills;

public interface ILemmingState
{
    public enum StateType
    {
        ClimberState,
        FloaterState,
        GliderState,
        SwimmerState,
        DisarmerState,
        SliderState,

        AcidLemmingState,
        WaterState,
        FastForwardState,

        NeutralState,
        ZombieState,
    }

    private static readonly ILemmingState[] AllLemmingStateChangers = GetLemmingStates();
    public static ReadOnlySpan<ILemmingState> AllItems => new(AllLemmingStateChangers);

    private static ILemmingState[] GetLemmingStates()
    {
        var result = new ILemmingState[]
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

        if (result.Length != LemmingStateConstants.NumberOfStates)
            throw new Exception($"Number of {nameof(ILemmingState)}s is actually {result.Length}! Update {nameof(LemmingStateConstants.NumberOfStates)}!");

        var hasher = new LemmingStateChangerHasher();
        hasher.AssertUniqueIds(new ReadOnlySpan<ILemmingState>(result));
        Array.Sort(result, hasher);

        return result;
    }

    [Pure]
    StateType LemmingStateType { get; }

    void SetLemmingState(LemmingState lemmingState, bool status);
    void ToggleLemmingState(LemmingState lemmingState);

    [Pure]
    bool IsApplied(LemmingState lemmingState);

    public static LemmingStateSet CreateBitArraySet() => new(new LemmingStateChangerHasher());

    public readonly struct LemmingStateChangerHasher : IPerfectHasher<ILemmingState>, IBitBufferCreator<BitBuffer32>
    {
        public int NumberOfItems => LemmingStateConstants.NumberOfStates;
        public int Hash(ILemmingState item) => (int)item.LemmingStateType;
        public ILemmingState UnHash(int index) => AllLemmingStateChangers[index];

        public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
    }
}
