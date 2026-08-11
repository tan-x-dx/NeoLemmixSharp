namespace NeoLemmixSharp.Common;

public enum LemmingAbilityType
{
    ClimberAbility,
    FloaterAbility,
    GliderAbility,
    SwimmerAbility,
    DisarmerAbility,
    SliderAbility,

    AcidLemmingAbility,
    WaterLemmingAbility,
    FastForwardAbility,

    NeutralAbility = 29,
    ZombieAbility = 30
}

public static class LemmingAbilityConstants
{
    public const int NumberOfAbilities = 11;

    public const int ClimberBitIndex = (int)LemmingAbilityType.ClimberAbility;
    public const int FloaterBitIndex = (int)LemmingAbilityType.FloaterAbility;
    public const int GliderBitIndex = (int)LemmingAbilityType.GliderAbility;
    public const int SliderBitIndex = (int)LemmingAbilityType.SliderAbility;
    public const int SwimmerBitIndex = (int)LemmingAbilityType.SwimmerAbility;
    public const int DisarmerBitIndex = (int)LemmingAbilityType.DisarmerAbility;
    public const int AcidLemmingBitIndex = (int)LemmingAbilityType.AcidLemmingAbility;
    public const int WaterLemmingBitIndex = (int)LemmingAbilityType.WaterLemmingAbility;
    public const int PermanentFastForwardBitIndex = (int)LemmingAbilityType.FastForwardAbility;
    public const int ZombieBitIndex = (int)LemmingAbilityType.ZombieAbility;
    public const int NeutralBitIndex = (int)LemmingAbilityType.NeutralAbility;

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
