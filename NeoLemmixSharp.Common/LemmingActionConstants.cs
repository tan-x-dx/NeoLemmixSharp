using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common;

public static class LemmingActionConstants
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidLemmingActionId(int lemmingActionId)
    {
        return (uint)lemmingActionId < NumberOfLemmingActions;
    }

    public const int NumberOfLemmingActions = 34;
    public const int LongestActionNameLength = 11;

    public const string NoneActionName = "None";
    public const int NoneActionId = -1;

    public const string WalkerActionName = "Walker";
    public const string WalkerActionSpriteFileName = "walker";
    public const int WalkerActionId = 0;
    public const int WalkerAnimationFrames = 8;
    public const int MaxWalkerPhysicsFrames = 4;

    public const string ClimberActionName = "Climber";
    public const string ClimberActionSpriteFileName = "climber";
    public const int ClimberActionId = 1;
    public const int ClimberAnimationFrames = 8;
    public const int MaxClimberPhysicsFrames = 8;

    public const string FloaterActionName = "Floater";
    public const string FloaterActionSpriteFileName = "floater";
    public const int FloaterActionId = 2;
    public const int FloaterAnimationFrames = 17;
    public const int MaxFloaterPhysicsFrames = 17;

    public const string BlockerActionName = "Blocker";
    public const string BlockerActionSpriteFileName = "blocker";
    public const int BlockerActionId = 3;
    public const int BlockerAnimationFrames = 16;
    public const int MaxBlockerPhysicsFrames = 16;

    public const string BuilderActionName = "Builder";
    public const string BuilderActionSpriteFileName = "builder";
    public const int BuilderActionId = 4;
    public const int BuilderAnimationFrames = 16;
    public const int MaxBuilderPhysicsFrames = 16;

    public const string BasherActionName = "Basher";
    public const string BasherActionSpriteFileName = "basher";
    public const int BasherActionId = 5;
    public const int BasherAnimationFrames = 32;
    public const int MaxBasherPhysicsFrames = 16;

    public const string MinerActionName = "Miner";
    public const string MinerActionSpriteFileName = "miner";
    public const int MinerActionId = 6;
    public const int MinerAnimationFrames = 24;
    public const int MaxMinerPhysicsFrames = 24;

    public const string DiggerActionName = "Digger";
    public const string DiggerActionSpriteFileName = "digger";
    public const int DiggerActionId = 7;
    public const int DiggerAnimationFrames = 16;
    public const int MaxDiggerPhysicsFrames = 16;

    public const string PlatformerActionName = "Platformer";
    public const string PlatformerActionSpriteFileName = "platformer";
    public const int PlatformerActionId = 8;
    public const int PlatformerAnimationFrames = 16;
    public const int MaxPlatformerPhysicsFrames = 16;

    public const string StackerActionName = "Stacker";
    public const string StackerActionSpriteFileName = "stacker";
    public const int StackerActionId = 9;
    public const int StackerAnimationFrames = 8;
    public const int MaxStackerPhysicsFrames = 8;

    public const string FencerActionName = "Fencer";
    public const string FencerActionSpriteFileName = "fencer";
    public const int FencerActionId = 10;
    public const int FencerAnimationFrames = 16;
    public const int MaxFencerPhysicsFrames = 16;

    public const string GliderActionName = "Glider";
    public const string GliderActionSpriteFileName = "glider";
    public const int GliderActionId = 11;
    public const int GliderAnimationFrames = 17;
    public const int MaxGliderPhysicsFrames = 17;

    public const string JumperActionName = "Jumper";
    public const string JumperActionSpriteFileName = "jumper";
    public const int JumperActionId = 12;
    public const int JumperAnimationFrames = 3;
    public const int MaxJumperPhysicsFrames = 13;

    public const string SwimmerActionName = "Swimmer";
    public const string SwimmerActionSpriteFileName = "swimmer";
    public const int SwimmerActionId = 13;
    public const int SwimmerAnimationFrames = 8;
    public const int MaxSwimmerPhysicsFrames = 8;

    public const string ShimmierActionName = "Shimmier";
    public const string ShimmierActionSpriteFileName = "shimmier";
    public const int ShimmierActionId = 14;
    public const int ShimmierAnimationFrames = 20;
    public const int MaxShimmierPhysicsFrames = 20;

    public const string LasererActionName = "Laserer";
    public const string LasererActionSpriteFileName = "laserer";
    public const int LasererActionId = 15;
    public const int LasererAnimationFrames = 1;
    public const int MaxLasererPhysicsFrames = 12; // It's, ironically, this high for rendering purposes 

    public const string SliderActionName = "Slider";
    public const string SliderActionSpriteFileName = "slider";
    public const int SliderActionId = 16;
    public const int SliderAnimationFrames = 3;
    public const int MaxSliderPhysicsFrames = 1;

    public const string FallerActionName = "Faller";
    public const string FallerActionSpriteFileName = "faller";
    public const int FallerActionId = 17;
    public const int FallerAnimationFrames = 4;
    public const int MaxFallerPhysicsFrames = 4;

    public const string AscenderActionName = "Ascender";
    public const string AscenderActionSpriteFileName = "ascender";
    public const int AscenderActionId = 18;
    public const int AscenderAnimationFrames = 1;
    public const int MaxAscenderPhysicsFrames = 1;

    public const string ShruggerActionName = "Shrugger";
    public const string ShruggerActionSpriteFileName = "shrugger";
    public const int ShruggerActionId = 19;
    public const int ShruggerAnimationFrames = 8;
    public const int MaxShruggerPhysicsFrames = 8;

    public const string DrownerActionName = "Drowner";
    public const string DrownerActionSpriteFileName = "drowner";
    public const int DrownerActionId = 20;
    public const int DrownerAnimationFrames = 16;
    public const int MaxDrownerPhysicsFrames = 16;

    public const string HoisterActionName = "Hoister";
    public const string HoisterActionSpriteFileName = "hoister";
    public const int HoisterActionId = 21;
    public const int HoisterAnimationFrames = 8;
    public const int MaxHoisterPhysicsFrames = 8;

    public const string DehoisterActionName = "Dehoister";
    public const string DehoisterActionSpriteFileName = "dehoister";
    public const int DehoisterActionId = 22;
    public const int DehoisterAnimationFrames = 7;
    public const int MaxDehoisterPhysicsFrames = 7;

    public const string ReacherActionName = "Reacher";
    public const string ReacherActionSpriteFileName = "reacher";
    public const int ReacherActionId = 23;
    public const int ReacherAnimationFrames = 6;
    public const int MaxReacherPhysicsFrames = 8;

    public const string DisarmerActionName = "Disarmer";
    public const string DisarmerActionSpriteFileName = "disarmer";
    public const int DisarmerActionId = 24;
    public const int DisarmerAnimationFrames = 16;
    public const int MaxDisarmerPhysicsFrames = 16;

    public const string ExiterActionName = "Exiter";
    public const string ExiterActionSpriteFileName = "exiter";
    public const int ExiterActionId = 25;
    public const int ExiterAnimationFrames = 8;
    public const int MaxExiterPhysicsFrames = 8;

    public const string ExploderActionName = "Exploder";
    public const string ExploderActionSpriteFileName = "bomber";
    public const int ExploderActionId = 26;
    public const int ExploderAnimationFrames = 1;
    public const int MaxExploderPhysicsFrames = 1;

    public const string OhNoerActionName = "Oh Noer";
    public const string OhNoerActionSpriteFileName = "ohnoer";
    public const int OhNoerActionId = 27;
    public const int OhNoerAnimationFrames = 16;
    public const int MaxOhNoerPhysicsFrames = 16;

    public const string SplatterActionName = "Splatter";
    public const string SplatterActionSpriteFileName = "splatter";
    public const int SplatterActionId = 28;
    public const int SplatterAnimationFrames = 16;
    public const int MaxSplatterPhysicsFrames = 16;

    public const string StonerActionName = "Stoner";
    public const string StonerActionSpriteFileName = "stoner";
    public const int StonerActionId = 29;
    public const int StonerAnimationFrames = 1;
    public const int MaxStonerPhysicsFrames = 1;

    public const string VaporiserActionName = "Vaporiser";
    public const string VaporiserActionSpriteFileName = "vaporiser";
    public const int VaporiserActionId = 30;
    public const int VaporiserAnimationFrames = 16;
    public const int MaxVaporizerPhysicsFrames = 14;

    public const string RotateClockwiseActionName = "Rotator";
    public const string RotateClockwiseActionSpriteFileName = "rotate_90cw";
    public const int RotateClockwiseActionId = 31;
    public const int RotateClockwiseAnimationFrames = 9;
    public const int MaxRotateClockwisePhysicsFrames = 9;

    public const string RotateCounterclockwiseActionName = "Rotator";
    public const string RotateCounterclockwiseActionSpriteFileName = "rotate_90ccw";
    public const int RotateCounterclockwiseActionId = 32;
    public const int RotateCounterclockwiseAnimationFrames = 9;
    public const int MaxRotateCounterclockwisePhysicsFrames = 9;

    public const string RotateHalfActionName = "Rotator";
    public const string RotateHalfActionSpriteFileName = "rotate_180cw";
    public const int RotateHalfActionId = 33;
    public const int RotateHalfAnimationFrames = 15;
    public const int MaxRotateHalfPhysicsFrames = 15;

    #region Cursor Priority Levels

    public const int NonPermanentSkillPriority = 4;
    public const int PermanentSkillPriority = 3;
    public const int NonWalkerMovementPriority = 2;
    public const int WalkerMovementPriority = 1;
    public const int NoPriority = 0;

    #endregion

    private static readonly Dictionary<string, int> LemmingActionNameToIdLookup = GenerateLemmingActionNameToIdLookup();

    private static Dictionary<string, int> GenerateLemmingActionNameToIdLookup()
    {
        var result = new Dictionary<string, int>(NumberOfLemmingActions, StringComparer.OrdinalIgnoreCase)
        {
            { WalkerActionSpriteFileName, WalkerActionId },
            { ClimberActionSpriteFileName, ClimberActionId },
            { FloaterActionSpriteFileName, FloaterActionId },
            { BlockerActionSpriteFileName, BlockerActionId },
            { BuilderActionSpriteFileName, BuilderActionId },
            { BasherActionSpriteFileName, BasherActionId },
            { MinerActionSpriteFileName, MinerActionId },
            { DiggerActionSpriteFileName, DiggerActionId },
            { PlatformerActionSpriteFileName, PlatformerActionId },
            { StackerActionSpriteFileName, StackerActionId },
            { FencerActionSpriteFileName, FencerActionId },
            { GliderActionSpriteFileName, GliderActionId },
            { JumperActionSpriteFileName, JumperActionId },
            { SwimmerActionSpriteFileName, SwimmerActionId },
            { ShimmierActionSpriteFileName, ShimmierActionId },
            { LasererActionSpriteFileName, LasererActionId },
            { SliderActionSpriteFileName, SliderActionId },
            { FallerActionSpriteFileName, FallerActionId },
            { AscenderActionSpriteFileName, AscenderActionId },
            { ShruggerActionSpriteFileName, ShruggerActionId },
            { DrownerActionSpriteFileName, DrownerActionId },
            { HoisterActionSpriteFileName, HoisterActionId },
            { DehoisterActionSpriteFileName, DehoisterActionId },
            { ReacherActionSpriteFileName, ReacherActionId },
            { DisarmerActionSpriteFileName, DisarmerActionId },
            { ExiterActionSpriteFileName, ExiterActionId },
            { ExploderActionSpriteFileName, ExploderActionId },
            { OhNoerActionSpriteFileName, OhNoerActionId },
            { SplatterActionSpriteFileName, SplatterActionId },
            { StonerActionSpriteFileName, StonerActionId },
            { VaporiserActionSpriteFileName, VaporiserActionId },
            { RotateClockwiseActionSpriteFileName, RotateClockwiseActionId },
            { RotateCounterclockwiseActionSpriteFileName, RotateCounterclockwiseActionId },
            { RotateHalfActionSpriteFileName, RotateHalfActionId }
        };

        if (result.Count != NumberOfLemmingActions)
            throw new Exception("Need to update this collection with new actions!");

        return result;
    }

    public static bool TryGetLemmingActionIdFromName(string lemmingActionName, out int lemmingActionId)
    {
        return LemmingActionNameToIdLookup.TryGetValue(lemmingActionName, out lemmingActionId);
    }

    public static bool TryGetLemmingActionIdFromName(ReadOnlySpan<char> lemmingActionNameSpan, out int lemmingActionId)
    {
        var alternateLookup = LemmingActionNameToIdLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        return alternateLookup.TryGetValue(lemmingActionNameSpan, out lemmingActionId);
    }

    private static readonly LemmingActionLookupData[] LemmingActionIdToStringLookup = GenerateLemmingActionIdToStringLookup();

    private static LemmingActionLookupData[] GenerateLemmingActionIdToStringLookup()
    {
        var result = new LemmingActionLookupData[NumberOfLemmingActions];
        var count = 0;

        SetData(WalkerActionId, WalkerActionName, WalkerActionSpriteFileName, WalkerAnimationFrames);
        SetData(ClimberActionId, ClimberActionName, ClimberActionSpriteFileName, ClimberAnimationFrames);
        SetData(FloaterActionId, FloaterActionName, FloaterActionSpriteFileName, FloaterAnimationFrames);
        SetData(BlockerActionId, BlockerActionName, BlockerActionSpriteFileName, BlockerAnimationFrames);
        SetData(BuilderActionId, BuilderActionName, BuilderActionSpriteFileName, BuilderAnimationFrames);
        SetData(BasherActionId, BasherActionName, BasherActionSpriteFileName, BasherAnimationFrames);
        SetData(MinerActionId, MinerActionName, MinerActionSpriteFileName, MinerAnimationFrames);
        SetData(DiggerActionId, DiggerActionName, DiggerActionSpriteFileName, DiggerAnimationFrames);
        SetData(PlatformerActionId, PlatformerActionName, PlatformerActionSpriteFileName, PlatformerAnimationFrames);
        SetData(StackerActionId, StackerActionName, StackerActionSpriteFileName, StackerAnimationFrames);
        SetData(FencerActionId, FencerActionName, FencerActionSpriteFileName, FencerAnimationFrames);
        SetData(GliderActionId, GliderActionName, GliderActionSpriteFileName, GliderAnimationFrames);
        SetData(JumperActionId, JumperActionName, JumperActionSpriteFileName, JumperAnimationFrames);
        SetData(SwimmerActionId, SwimmerActionName, SwimmerActionSpriteFileName, SwimmerAnimationFrames);
        SetData(ShimmierActionId, ShimmierActionName, ShimmierActionSpriteFileName, ShimmierAnimationFrames);
        SetData(LasererActionId, LasererActionName, LasererActionSpriteFileName, LasererAnimationFrames);
        SetData(SliderActionId, SliderActionName, SliderActionSpriteFileName, SliderAnimationFrames);
        SetData(FallerActionId, FallerActionName, FallerActionSpriteFileName, FallerAnimationFrames);
        SetData(AscenderActionId, AscenderActionName, AscenderActionSpriteFileName, AscenderAnimationFrames);
        SetData(ShruggerActionId, ShruggerActionName, ShruggerActionSpriteFileName, ShruggerAnimationFrames);
        SetData(DrownerActionId, DrownerActionName, DrownerActionSpriteFileName, DrownerAnimationFrames);
        SetData(HoisterActionId, HoisterActionName, HoisterActionSpriteFileName, HoisterAnimationFrames);
        SetData(DehoisterActionId, DehoisterActionName, DehoisterActionSpriteFileName, DehoisterAnimationFrames);
        SetData(ReacherActionId, ReacherActionName, ReacherActionSpriteFileName, ReacherAnimationFrames);
        SetData(DisarmerActionId, DisarmerActionName, DisarmerActionSpriteFileName, DisarmerAnimationFrames);
        SetData(ExiterActionId, ExiterActionName, ExiterActionSpriteFileName, ExiterAnimationFrames);
        SetData(ExploderActionId, ExploderActionName, ExploderActionSpriteFileName, ExploderAnimationFrames);
        SetData(OhNoerActionId, OhNoerActionName, OhNoerActionSpriteFileName, OhNoerAnimationFrames);
        SetData(SplatterActionId, SplatterActionName, SplatterActionSpriteFileName, SplatterAnimationFrames);
        SetData(StonerActionId, StonerActionName, StonerActionSpriteFileName, StonerAnimationFrames);
        SetData(VaporiserActionId, VaporiserActionName, VaporiserActionSpriteFileName, VaporiserAnimationFrames);
        SetData(RotateClockwiseActionId, RotateClockwiseActionName, RotateClockwiseActionSpriteFileName, RotateClockwiseAnimationFrames);
        SetData(RotateCounterclockwiseActionId, RotateCounterclockwiseActionName, RotateCounterclockwiseActionSpriteFileName, RotateCounterclockwiseAnimationFrames);
        SetData(RotateHalfActionId, RotateHalfActionName, RotateHalfActionSpriteFileName, RotateHalfAnimationFrames);

        if (count != NumberOfLemmingActions)
            throw new Exception("Need to update this collection with new actions!");

        return result;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetData(int index, string lemmingActionName, string lemmingActionFileName, int numberOfAnimationFrames)
        {
            result[index] = new LemmingActionLookupData(lemmingActionName, lemmingActionFileName, numberOfAnimationFrames);
            count++;
        }
    }

    public static LemmingActionLookupData GetLemmingActionDataFromId(int lemmingActionId) => LemmingActionIdToStringLookup[lemmingActionId];

    [DebuggerDisplay("{LemmingActionName}")]
    public readonly struct LemmingActionLookupData(string lemmingActionName, string lemmingActionFileName, int numberOfAnimationFrames)
    {
        public readonly string LemmingActionName = lemmingActionName;
        public readonly string LemmingActionFileName = lemmingActionFileName;
        public readonly int NumberOfAnimationFrames = numberOfAnimationFrames;
    }
}

public static class LemmingActionBounds
{
    public static readonly RectangularRegion StandardLemmingBounds = new(new Point(-3, -10), new Point(3, 0));

    public static readonly RectangularRegion BlockerActionBounds = new(new Point(-6, -10), new Point(6, 0));
    public static readonly RectangularRegion ClimberActionBounds = new(new Point(-6, -10), new Point(0, 0));
    public static readonly RectangularRegion DiggerLemmingBounds = new(new Point(-3, -5), new Point(3, 0));
    public static readonly RectangularRegion DisarmerLemmingBounds = new(new Point(-3, -8), new Point(3, 0));
    public static readonly RectangularRegion GliderActionBounds = new(new Point(-3, -12), new Point(3, 0));
    public static readonly RectangularRegion HoisterActionBounds = new(new Point(-5, -10), new Point(1, 1));
    public static readonly RectangularRegion JumperActionBounds = new(new Point(-1, -9), new Point(3, 0));
    public static readonly RectangularRegion MinerActionBounds = new(new Point(-2, -10), new Point(4, 0));
    public static readonly RectangularRegion PlatformerActionBounds = new(new Point(-3, -5), new Point(3, 0));
    public static readonly RectangularRegion ReacherActionBounds = new(new Point(-3, -9), new Point(3, 0));
    public static readonly RectangularRegion ShimmierActionBounds = new(new Point(-3, -9), new Point(3, 2));
    public static readonly RectangularRegion SplatterActionBounds = new(new Point(-3, -6), new Point(3, 0));
    public static readonly RectangularRegion SwimmerActionBounds = new(new Point(-7, -4), new Point(5, 0));
    public static readonly RectangularRegion VaporiserActionBounds = new(new Point(-3, -12), new Point(3, 2));

    public static RectangularRegion GetBounds(int lemmingActionId) => lemmingActionId switch
    {
        LemmingActionConstants.BlockerActionId => BlockerActionBounds,
        LemmingActionConstants.ClimberActionId => ClimberActionBounds,
        LemmingActionConstants.SliderActionId => ClimberActionBounds,
        LemmingActionConstants.DiggerActionId => DiggerLemmingBounds,
        LemmingActionConstants.DisarmerActionId => DisarmerLemmingBounds,
        LemmingActionConstants.GliderActionId => GliderActionBounds,
        LemmingActionConstants.HoisterActionId => HoisterActionBounds,
        LemmingActionConstants.DehoisterActionId => HoisterActionBounds,
        LemmingActionConstants.JumperActionId => JumperActionBounds,
        LemmingActionConstants.MinerActionId => MinerActionBounds,
        LemmingActionConstants.PlatformerActionId => PlatformerActionBounds,
        LemmingActionConstants.ReacherActionId => ReacherActionBounds,
        LemmingActionConstants.ShimmierActionId => ShimmierActionBounds,
        LemmingActionConstants.SplatterActionId => SplatterActionBounds,
        LemmingActionConstants.SwimmerActionId => SwimmerActionBounds,
        LemmingActionConstants.VaporiserActionId => VaporiserActionBounds,

        _ => StandardLemmingBounds,
    };
}
