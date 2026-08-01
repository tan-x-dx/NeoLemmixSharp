using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common;

public enum LemmingSkillType
{
    NoneSkill = -1,
    ClimberSkill,
    FloaterSkill,
    BlockerSkill,
    BomberSkill,
    BuilderSkill,
    BasherSkill,
    MinerSkill,
    DiggerSkill,
    WalkerSkill,
    PlatformerSkill,
    StackerSkill,
    FencerSkill,
    GliderSkill,
    JumperSkill,
    SwimmerSkill,
    ShimmierSkill,
    LasererSkill,
    SliderSkill,
    DisarmerSkill,
    StonerSkill,
    ClonerSkill,
    RotateClockwiseSkill,
    RotateCounterclockwiseSkill,
    RotateHalfSkill,
    AcidLemmingSkill,
    WaterLemmingSkill,
    FastForwardSkill,

    VALUE_MAX
}

public static class LemmingSkillConstants
{
    public const int NumberOfLemmingSkills = (int)LemmingSkillType.VALUE_MAX;

    public static LemmingSkillType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingSkillType>(rawValue, NumberOfLemmingSkills);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidLemmingSkillType(int rawLemmingSkillType)
    {
        return (uint)rawLemmingSkillType < NumberOfLemmingSkills;
    }

    public const string NoneSkillName = "None";
    public const string ClimberSkillName = "Climber";
    public const string FloaterSkillName = "Floater";
    public const string BlockerSkillName = "Blocker";
    public const string BomberSkillName = "Bomber";
    public const string BuilderSkillName = "Builder";
    public const string BasherSkillName = "Basher";
    public const string MinerSkillName = "Miner";
    public const string DiggerSkillName = "Digger";
    public const string WalkerSkillName = "Walker";
    public const string PlatformerSkillName = "Platformer";
    public const string StackerSkillName = "Stacker";
    public const string FencerSkillName = "Fencer";
    public const string GliderSkillName = "Glider";
    public const string JumperSkillName = "Jumper";
    public const string SwimmerSkillName = "Swimmer";
    public const string ShimmierSkillName = "Shimmier";
    public const string LasererSkillName = "Laserer";
    public const string SliderSkillName = "Slider";
    public const string DisarmerSkillName = "Disarmer";
    public const string StonerSkillName = "Stoner";
    public const string ClonerSkillName = "Cloner";
    public const string RotateClockwiseSkillName = "RotateClockwise";
    public const string RotateCounterclockwiseSkillName = "RotateCounterclockwise";
    public const string RotateHalfSkillName = "RotateHalf";
    public const string AcidLemmingSkillName = "Acid Lemming";
    public const string WaterLemmingSkillName = "Water Lemming";
    public const string FastForwardSkillName = "Fast Forward";

    private static readonly Dictionary<string, LemmingSkillType> LemmingSkillNameToTypeLookup = GenerateLemmingSkillNameToTypeLookup();

    private static Dictionary<string, LemmingSkillType> GenerateLemmingSkillNameToTypeLookup()
    {
        var result = new Dictionary<string, LemmingSkillType>(NumberOfLemmingSkills, StringComparer.OrdinalIgnoreCase)
        {
            { ClimberSkillName, LemmingSkillType.ClimberSkill },
            { FloaterSkillName, LemmingSkillType.FloaterSkill },
            { BlockerSkillName, LemmingSkillType.BlockerSkill },
            { BomberSkillName, LemmingSkillType.BomberSkill },
            { BuilderSkillName, LemmingSkillType.BuilderSkill },
            { BasherSkillName, LemmingSkillType.BasherSkill },
            { MinerSkillName, LemmingSkillType.MinerSkill },
            { DiggerSkillName, LemmingSkillType.DiggerSkill },
            { WalkerSkillName, LemmingSkillType.WalkerSkill },
            { PlatformerSkillName, LemmingSkillType.PlatformerSkill },
            { StackerSkillName, LemmingSkillType.StackerSkill },
            { FencerSkillName, LemmingSkillType.FencerSkill },
            { GliderSkillName, LemmingSkillType.GliderSkill },
            { JumperSkillName, LemmingSkillType.JumperSkill },
            { SwimmerSkillName, LemmingSkillType.SwimmerSkill },
            { ShimmierSkillName, LemmingSkillType.ShimmierSkill },
            { LasererSkillName, LemmingSkillType.LasererSkill },
            { SliderSkillName, LemmingSkillType.SliderSkill },
            { DisarmerSkillName, LemmingSkillType.DisarmerSkill },
            { StonerSkillName, LemmingSkillType.StonerSkill },
            { ClonerSkillName, LemmingSkillType.ClonerSkill },
            { RotateClockwiseSkillName, LemmingSkillType.RotateClockwiseSkill },
            { RotateCounterclockwiseSkillName, LemmingSkillType.RotateCounterclockwiseSkill },
            { RotateHalfSkillName, LemmingSkillType.RotateHalfSkill },
            { AcidLemmingSkillName, LemmingSkillType.AcidLemmingSkill },
            { WaterLemmingSkillName, LemmingSkillType.WaterLemmingSkill },
            { FastForwardSkillName, LemmingSkillType.FastForwardSkill },
        };

        if (result.Count != NumberOfLemmingSkills)
            throw new Exception("Need to update this collection with new skills!");

        return result;
    }

    public static bool TryGetLemmingSkillTypeFromName(string lemmingSkillName, out LemmingSkillType lemmingSkillType)
    {
        return LemmingSkillNameToTypeLookup.TryGetValue(lemmingSkillName, out lemmingSkillType);
    }

    public static bool TryGetLemmingSkillTypeFromName(ReadOnlySpan<char> lemmingSkillNameSpan, out LemmingSkillType lemmingSkillType)
    {
        var alternateLookup = LemmingSkillNameToTypeLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        return alternateLookup.TryGetValue(lemmingSkillNameSpan, out lemmingSkillType);
    }
}
