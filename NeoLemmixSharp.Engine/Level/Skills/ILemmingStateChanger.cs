using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public interface ILemmingStateChanger
{
    int LemmingStateChangerId { get; }

    void SetLemmingState(LemmingState lemmingState, bool status);
    void ToggleLemmingState(LemmingState lemmingState);

    bool IsApplied(LemmingState lemmingState);
}

public sealed class LemmingStateChangerHelper : IPerfectHasher<ILemmingStateChanger>
{
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

    private static readonly ILemmingStateChanger[] AllLemmingStateChangers =
    [
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
    ];

    private static readonly LemmingStateChangerHelper Instance = new();

    public int NumberOfItems => AllLemmingStateChangers.Length;
    public int Hash(ILemmingStateChanger item) => item.LemmingStateChangerId;

    public ILemmingStateChanger UnHash(int index) => AllLemmingStateChangers[index];

    public static StateChangerSet CreateSimpleSet() => new(Instance, false);
}