using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Skills;

public interface ILemmingAbilityChanger
{
    private static readonly ILemmingAbilityChanger[] AllLemmingAbilityChangers = GetLemmingAbilityChangers();
    public static ReadOnlySpan<ILemmingAbilityChanger> AllItems => new(AllLemmingAbilityChangers);

    private static ILemmingAbilityChanger[] GetLemmingAbilityChangers()
    {
        var result = new ILemmingAbilityChanger[]
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

        if (result.Length != LemmingAbilityConstants.NumberOfAbilities)
            throw new Exception($"Number of {nameof(ILemmingAbilityChanger)}s is actually {result.Length}! Update {nameof(LemmingAbilityConstants.NumberOfAbilities)}!");

        var hasher = new LemmingAbilityChangerHasher();
        hasher.AssertUniqueIds(new ReadOnlySpan<ILemmingAbilityChanger>(result));
        Array.Sort(result, hasher);

        return result;
    }

    [Pure]
    LemmingAbilityType LemmingAbilityType { get; }

    void SetLemmingAbility(Lemming lemming, bool status);
    void ToggleLemmingAbility(Lemming lemming);

    [Pure]
    bool LemmingHasAbility(Lemming lemming);

    public static LemmingAbilitySet CreateBitArraySet() => new(new LemmingAbilityChangerHasher());

    public readonly struct LemmingAbilityChangerHasher : IBitBufferCreator<BitBuffer32, ILemmingAbilityChanger>
    {
        public int NumberOfItems => LemmingAbilityConstants.NumberOfAbilities;
        public int Hash(ILemmingAbilityChanger item) => (int)item.LemmingAbilityType;
        public ILemmingAbilityChanger UnHash(int index) => AllLemmingAbilityChangers.At(index);

        public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
    }
}
