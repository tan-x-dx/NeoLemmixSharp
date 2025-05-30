using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common;

public static class LemmingSkillConstants
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidLemmingSkillId(int lemmingSkillId)
    {
        return (uint)lemmingSkillId < NumberOfLemmingSkills;
    }

    public const int NumberOfLemmingSkills = 31;

    public const string NoneSkillName = "None";
    public const int NoneSkillId = -1;

    public const string ClimberSkillName = "Climber";
    public const int ClimberSkillId = 0;

    public const string FloaterSkillName = "Floater";
    public const int FloaterSkillId = 1;

    public const string BlockerSkillName = "Blocker";
    public const int BlockerSkillId = 2;

    public const string BomberSkillName = "Bomber";
    public const int BomberSkillId = 3;

    public const string BuilderSkillName = "Builder";
    public const int BuilderSkillId = 4;

    public const string BasherSkillName = "Basher";
    public const int BasherSkillId = 5;

    public const string MinerSkillName = "Miner";
    public const int MinerSkillId = 6;

    public const string DiggerSkillName = "Digger";
    public const int DiggerSkillId = 7;

    public const string WalkerSkillName = "Walker";
    public const int WalkerSkillId = 8;

    public const string PlatformerSkillName = "Platformer";
    public const int PlatformerSkillId = 9;

    public const string StackerSkillName = "Stacker";
    public const int StackerSkillId = 10;

    public const string FencerSkillName = "Fencer";
    public const int FencerSkillId = 11;

    public const string GliderSkillName = "Glider";
    public const int GliderSkillId = 12;

    public const string JumperSkillName = "Jumper";
    public const int JumperSkillId = 13;

    public const string SwimmerSkillName = "Swimmer";
    public const int SwimmerSkillId = 14;

    public const string ShimmierSkillName = "Shimmier";
    public const int ShimmierSkillId = 15;

    public const string LasererSkillName = "Laserer";
    public const int LasererSkillId = 16;

    public const string SliderSkillName = "Slider";
    public const int SliderSkillId = 17;

    public const string DisarmerSkillName = "Disarmer";
    public const int DisarmerSkillId = 18;

    public const string StonerSkillName = "Stoner";
    public const int StonerSkillId = 19;

    public const string ClonerSkillName = "Cloner";
    public const int ClonerSkillId = 20;

    public const string RotateClockwiseSkillName = "RotateClockwise";
    public const int RotateClockwiseSkillId = 21;

    public const string RotateCounterclockwiseSkillName = "RotateCounterclockwise";
    public const int RotateCounterclockwiseSkillId = 22;

    public const string RotateHalfSkillName = "RotateHalf";
    public const int RotateHalfSkillId = 23;

    public const string RotateToDownSkillName = "RotateToDown";
    public const int RotateToDownSkillId = 24;

    public const string RotateToRightSkillName = "RotateToRight";
    public const int RotateToRightSkillId = 25;

    public const string RotateToUpSkillName = "RotateToUp";
    public const int RotateToUpSkillId = 26;

    public const string RotateToLeftSkillName = "RotateToLeft";
    public const int RotateToLeftSkillId = 27;

    public const string AcidLemmingSkillName = "Acid Lemming";
    public const int AcidLemmingSkillId = 28;

    public const string WaterLemmingSkillName = "Water Lemming";
    public const int WaterLemmingSkillId = 29;

    public const string FastForwardSkillName = "Fast Forward";
    public const int FastForwardSkillId = 30;
}