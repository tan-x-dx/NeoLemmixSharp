namespace NeoLemmixSharp.Common;

public static class LemmingStateConstants
{
    public const int NumberOfStates = 11;

    public const int ClimberBitIndex = 0;
    public const int FloaterBitIndex = 1;
    public const int GliderBitIndex = 2;
    public const int SliderBitIndex = 3;
    public const int SwimmerBitIndex = 4;
    public const int DisarmerBitIndex = 5;
    public const int AcidLemmingBitIndex = 20;
    public const int WaterLemmingBitIndex = 21;
    public const int PermanentFastForwardBitIndex = 28;
    public const int ZombieBitIndex = 29;
    public const int NeutralBitIndex = 30;

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
