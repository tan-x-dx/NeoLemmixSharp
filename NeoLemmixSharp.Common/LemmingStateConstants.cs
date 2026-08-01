namespace NeoLemmixSharp.Common;

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

    NeutralState = 29,
    ZombieState = 30
}

public static class LemmingStateConstants
{
    public const int NumberOfStates = 11;

    public const int ClimberBitIndex = (int)StateType.ClimberState;
    public const int FloaterBitIndex = (int)StateType.FloaterState;
    public const int GliderBitIndex = (int)StateType.GliderState;
    public const int SliderBitIndex = (int)StateType.SliderState;
    public const int SwimmerBitIndex = (int)StateType.SwimmerState;
    public const int DisarmerBitIndex = (int)StateType.DisarmerState;
    public const int AcidLemmingBitIndex = (int)StateType.AcidLemmingState;
    public const int WaterLemmingBitIndex = (int)StateType.WaterState;
    public const int PermanentFastForwardBitIndex = (int)StateType.FastForwardState;
    public const int ZombieBitIndex = (int)StateType.ZombieState;
    public const int NeutralBitIndex = (int)StateType.NeutralState;

    public const int ActiveBitIndex = 31;

    public const uint PermanentSkillBitMask = (1U << ClimberBitIndex) |
                                              (1U << FloaterBitIndex) |
                                              (1U << GliderBitIndex) |
                                              (1U << SliderBitIndex) |
                                              (1U << SwimmerBitIndex) |
                                              (1U << DisarmerBitIndex);

    public const uint LiquidAffinityBitMask = (1U << AcidLemmingBitIndex) |
                                              (1U << WaterLemmingBitIndex) |
                                              (1U << SwimmerBitIndex);

    public const uint SpecialFallingBehaviourBitMask = (1U << FloaterBitIndex) |
                                                       (1U << GliderBitIndex);

    public const uint AssignableSkillBitMask = (1U << ActiveBitIndex) |
                                               (1U << NeutralBitIndex) |
                                               (1U << ZombieBitIndex);
}
