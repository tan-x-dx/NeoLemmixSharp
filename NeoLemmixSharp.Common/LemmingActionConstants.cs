using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common;

public enum LemmingActionType
{
    NoneAction = -1,
    WalkerAction,
    ClimberAction,
    FloaterAction,
    BlockerAction,
    BuilderAction,
    BasherAction,
    MinerAction,
    DiggerAction,
    PlatformerAction,
    StackerAction,
    FencerAction,
    GliderAction,
    JumperAction,
    SwimmerAction,
    ShimmierAction,
    LasererAction,
    SliderAction,
    FallerAction,
    AscenderAction,
    ShruggerAction,
    DrownerAction,
    HoisterAction,
    DehoisterAction,
    ReacherAction,
    DisarmerAction,
    ExiterAction,
    ExploderAction,
    OhNoerAction,
    SplatterAction,
    StonerAction,
    VaporiserAction,
    RotateClockwiseAction,
    RotateCounterclockwiseAction,
    RotateHalfAction,

    VALUE_MAX
}

public enum CursorSelectionPriority
{
    NoneActionPriority = -1,
    NoPriority,
    WalkerMovementPriority,
    NonWalkerMovementPriority,
    PermanentSkillPriority,
    NonPermanentSkillPriority,
}

public static class LemmingActionConstants
{
    public const int NumberOfLemmingActions = (int)LemmingActionType.VALUE_MAX;

    public static LemmingActionType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingActionType>(rawValue, NumberOfLemmingActions);

    public const int LongestActionNameLength = 11;

    public const string NoneActionName = "None";

    public const string WalkerActionName = "Walker";
    public const string WalkerActionSpriteFileName = "walker";
    public const int WalkerAnimationFrames = 8;
    public const int MaxWalkerPhysicsFrames = 4;

    public const string ClimberActionName = "Climber";
    public const string ClimberActionSpriteFileName = "climber";
    public const int ClimberAnimationFrames = 8;
    public const int MaxClimberPhysicsFrames = 8;

    public const string FloaterActionName = "Floater";
    public const string FloaterActionSpriteFileName = "floater";
    public const int FloaterAnimationFrames = 17;
    public const int MaxFloaterPhysicsFrames = 17;

    public const string BlockerActionName = "Blocker";
    public const string BlockerActionSpriteFileName = "blocker";
    public const int BlockerAnimationFrames = 16;
    public const int MaxBlockerPhysicsFrames = 16;

    public const string BuilderActionName = "Builder";
    public const string BuilderActionSpriteFileName = "builder";
    public const int BuilderAnimationFrames = 16;
    public const int MaxBuilderPhysicsFrames = 16;

    public const string BasherActionName = "Basher";
    public const string BasherActionSpriteFileName = "basher";
    public const int BasherAnimationFrames = 32;
    public const int MaxBasherPhysicsFrames = 16;

    public const string MinerActionName = "Miner";
    public const string MinerActionSpriteFileName = "miner";
    public const int MinerAnimationFrames = 24;
    public const int MaxMinerPhysicsFrames = 24;

    public const string DiggerActionName = "Digger";
    public const string DiggerActionSpriteFileName = "digger";
    public const int DiggerAnimationFrames = 16;
    public const int MaxDiggerPhysicsFrames = 16;

    public const string PlatformerActionName = "Platformer";
    public const string PlatformerActionSpriteFileName = "platformer";
    public const int PlatformerAnimationFrames = 16;
    public const int MaxPlatformerPhysicsFrames = 16;

    public const string StackerActionName = "Stacker";
    public const string StackerActionSpriteFileName = "stacker";
    public const int StackerAnimationFrames = 8;
    public const int MaxStackerPhysicsFrames = 8;

    public const string FencerActionName = "Fencer";
    public const string FencerActionSpriteFileName = "fencer";
    public const int FencerAnimationFrames = 16;
    public const int MaxFencerPhysicsFrames = 16;

    public const string GliderActionName = "Glider";
    public const string GliderActionSpriteFileName = "glider";
    public const int GliderAnimationFrames = 17;
    public const int MaxGliderPhysicsFrames = 17;

    public const string JumperActionName = "Jumper";
    public const string JumperActionSpriteFileName = "jumper";
    public const int JumperAnimationFrames = 3;
    public const int MaxJumperPhysicsFrames = 13;

    public const string SwimmerActionName = "Swimmer";
    public const string SwimmerActionSpriteFileName = "swimmer";
    public const int SwimmerAnimationFrames = 8;
    public const int MaxSwimmerPhysicsFrames = 8;

    public const string ShimmierActionName = "Shimmier";
    public const string ShimmierActionSpriteFileName = "shimmier";
    public const int ShimmierAnimationFrames = 20;
    public const int MaxShimmierPhysicsFrames = 20;

    public const string LasererActionName = "Laserer";
    public const string LasererActionSpriteFileName = "laserer";
    public const int LasererAnimationFrames = 1;
    public const int MaxLasererPhysicsFrames = 12; // It's, ironically, this high for rendering purposes 

    public const string SliderActionName = "Slider";
    public const string SliderActionSpriteFileName = "slider";
    public const int SliderAnimationFrames = 3;
    public const int MaxSliderPhysicsFrames = 1;

    public const string FallerActionName = "Faller";
    public const string FallerActionSpriteFileName = "faller";
    public const int FallerAnimationFrames = 4;
    public const int MaxFallerPhysicsFrames = 4;

    public const string AscenderActionName = "Ascender";
    public const string AscenderActionSpriteFileName = "ascender";
    public const int AscenderAnimationFrames = 1;
    public const int MaxAscenderPhysicsFrames = 1;

    public const string ShruggerActionName = "Shrugger";
    public const string ShruggerActionSpriteFileName = "shrugger";
    public const int ShruggerAnimationFrames = 8;
    public const int MaxShruggerPhysicsFrames = 8;

    public const string DrownerActionName = "Drowner";
    public const string DrownerActionSpriteFileName = "drowner";
    public const int DrownerAnimationFrames = 16;
    public const int MaxDrownerPhysicsFrames = 16;

    public const string HoisterActionName = "Hoister";
    public const string HoisterActionSpriteFileName = "hoister";
    public const int HoisterAnimationFrames = 8;
    public const int MaxHoisterPhysicsFrames = 8;

    public const string DehoisterActionName = "Dehoister";
    public const string DehoisterActionSpriteFileName = "dehoister";
    public const int DehoisterAnimationFrames = 7;
    public const int MaxDehoisterPhysicsFrames = 7;

    public const string ReacherActionName = "Reacher";
    public const string ReacherActionSpriteFileName = "reacher";
    public const int ReacherAnimationFrames = 6;
    public const int MaxReacherPhysicsFrames = 8;

    public const string DisarmerActionName = "Disarmer";
    public const string DisarmerActionSpriteFileName = "disarmer";
    public const int DisarmerAnimationFrames = 16;
    public const int MaxDisarmerPhysicsFrames = 16;

    public const string ExiterActionName = "Exiter";
    public const string ExiterActionSpriteFileName = "exiter";
    public const int ExiterAnimationFrames = 8;
    public const int MaxExiterPhysicsFrames = 8;

    public const string ExploderActionName = "Exploder";
    public const string ExploderActionSpriteFileName = "bomber";
    public const int ExploderAnimationFrames = 1;
    public const int MaxExploderPhysicsFrames = 1;

    public const string OhNoerActionName = "Oh Noer";
    public const string OhNoerActionSpriteFileName = "ohnoer";
    public const int OhNoerAnimationFrames = 16;
    public const int MaxOhNoerPhysicsFrames = 16;

    public const string SplatterActionName = "Splatter";
    public const string SplatterActionSpriteFileName = "splatter";
    public const int SplatterAnimationFrames = 16;
    public const int MaxSplatterPhysicsFrames = 16;

    public const string StonerActionName = "Stoner";
    public const string StonerActionSpriteFileName = "stoner";
    public const int StonerAnimationFrames = 1;
    public const int MaxStonerPhysicsFrames = 1;

    public const string VaporiserActionName = "Vaporiser";
    public const string VaporiserActionSpriteFileName = "vaporiser";
    public const int VaporiserAnimationFrames = 16;
    public const int MaxVaporizerPhysicsFrames = 14;

    public const string RotateClockwiseActionName = "Rotator";
    public const string RotateClockwiseActionSpriteFileName = "rotate_90cw";
    public const int RotateClockwiseAnimationFrames = 9;
    public const int MaxRotateClockwisePhysicsFrames = 9;

    public const string RotateCounterclockwiseActionName = "Rotator";
    public const string RotateCounterclockwiseActionSpriteFileName = "rotate_90ccw";
    public const int RotateCounterclockwiseAnimationFrames = 9;
    public const int MaxRotateCounterclockwisePhysicsFrames = 9;

    public const string RotateHalfActionName = "Rotator";
    public const string RotateHalfActionSpriteFileName = "rotate_180cw";
    public const int RotateHalfAnimationFrames = 15;
    public const int MaxRotateHalfPhysicsFrames = 15;

    private static readonly Dictionary<string, LemmingActionType> LemmingActionNameToIdLookup = GenerateLemmingActionNameToIdLookup();

    private static Dictionary<string, LemmingActionType> GenerateLemmingActionNameToIdLookup()
    {
        var result = new Dictionary<string, LemmingActionType>(NumberOfLemmingActions, StringComparer.OrdinalIgnoreCase)
        {
            { WalkerActionSpriteFileName, LemmingActionType.WalkerAction },
            { ClimberActionSpriteFileName, LemmingActionType.ClimberAction },
            { FloaterActionSpriteFileName, LemmingActionType.FloaterAction },
            { BlockerActionSpriteFileName, LemmingActionType.BlockerAction },
            { BuilderActionSpriteFileName, LemmingActionType.BuilderAction },
            { BasherActionSpriteFileName, LemmingActionType.BasherAction },
            { MinerActionSpriteFileName, LemmingActionType.MinerAction },
            { DiggerActionSpriteFileName, LemmingActionType.DiggerAction },
            { PlatformerActionSpriteFileName, LemmingActionType.PlatformerAction },
            { StackerActionSpriteFileName, LemmingActionType.StackerAction },
            { FencerActionSpriteFileName, LemmingActionType.FencerAction },
            { GliderActionSpriteFileName, LemmingActionType.GliderAction },
            { JumperActionSpriteFileName, LemmingActionType.JumperAction },
            { SwimmerActionSpriteFileName, LemmingActionType.SwimmerAction },
            { ShimmierActionSpriteFileName, LemmingActionType.ShimmierAction },
            { LasererActionSpriteFileName, LemmingActionType.LasererAction },
            { SliderActionSpriteFileName, LemmingActionType.SliderAction },
            { FallerActionSpriteFileName, LemmingActionType.FallerAction },
            { AscenderActionSpriteFileName, LemmingActionType.AscenderAction },
            { ShruggerActionSpriteFileName, LemmingActionType.ShruggerAction },
            { DrownerActionSpriteFileName, LemmingActionType.DrownerAction },
            { HoisterActionSpriteFileName, LemmingActionType.HoisterAction },
            { DehoisterActionSpriteFileName, LemmingActionType.DehoisterAction },
            { ReacherActionSpriteFileName, LemmingActionType.ReacherAction },
            { DisarmerActionSpriteFileName, LemmingActionType.DisarmerAction },
            { ExiterActionSpriteFileName, LemmingActionType.ExiterAction },
            { ExploderActionSpriteFileName, LemmingActionType.ExploderAction },
            { OhNoerActionSpriteFileName, LemmingActionType.OhNoerAction },
            { SplatterActionSpriteFileName, LemmingActionType.SplatterAction },
            { StonerActionSpriteFileName, LemmingActionType.StonerAction },
            { VaporiserActionSpriteFileName, LemmingActionType.VaporiserAction },
            { RotateClockwiseActionSpriteFileName, LemmingActionType.RotateClockwiseAction },
            { RotateCounterclockwiseActionSpriteFileName, LemmingActionType.RotateCounterclockwiseAction },
            { RotateHalfActionSpriteFileName, LemmingActionType.RotateHalfAction }
        };

        if (result.Count != NumberOfLemmingActions)
            throw new Exception("Need to update this collection with new actions!");

        return result;
    }

    public static bool TryGetLemmingActionTypeFromName(string lemmingActionName, out LemmingActionType lemmingActionType)
    {
        return LemmingActionNameToIdLookup.TryGetValue(lemmingActionName, out lemmingActionType);
    }

    public static bool TryGetLemmingActionTypeFromName(ReadOnlySpan<char> lemmingActionNameSpan, out LemmingActionType lemmingActionType)
    {
        var alternateLookup = LemmingActionNameToIdLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        return alternateLookup.TryGetValue(lemmingActionNameSpan, out lemmingActionType);
    }

    private static readonly LemmingActionLookupData[] LemmingActionTypeToStringLookup = GenerateLemmingActionTypeToStringLookup();

    private static LemmingActionLookupData[] GenerateLemmingActionTypeToStringLookup()
    {
        var result = new LemmingActionLookupData[NumberOfLemmingActions];
        var count = 0;

        SetData(LemmingActionType.WalkerAction, WalkerActionName, WalkerActionSpriteFileName, WalkerAnimationFrames);
        SetData(LemmingActionType.ClimberAction, ClimberActionName, ClimberActionSpriteFileName, ClimberAnimationFrames);
        SetData(LemmingActionType.FloaterAction, FloaterActionName, FloaterActionSpriteFileName, FloaterAnimationFrames);
        SetData(LemmingActionType.BlockerAction, BlockerActionName, BlockerActionSpriteFileName, BlockerAnimationFrames);
        SetData(LemmingActionType.BuilderAction, BuilderActionName, BuilderActionSpriteFileName, BuilderAnimationFrames);
        SetData(LemmingActionType.BasherAction, BasherActionName, BasherActionSpriteFileName, BasherAnimationFrames);
        SetData(LemmingActionType.MinerAction, MinerActionName, MinerActionSpriteFileName, MinerAnimationFrames);
        SetData(LemmingActionType.DiggerAction, DiggerActionName, DiggerActionSpriteFileName, DiggerAnimationFrames);
        SetData(LemmingActionType.PlatformerAction, PlatformerActionName, PlatformerActionSpriteFileName, PlatformerAnimationFrames);
        SetData(LemmingActionType.StackerAction, StackerActionName, StackerActionSpriteFileName, StackerAnimationFrames);
        SetData(LemmingActionType.FencerAction, FencerActionName, FencerActionSpriteFileName, FencerAnimationFrames);
        SetData(LemmingActionType.GliderAction, GliderActionName, GliderActionSpriteFileName, GliderAnimationFrames);
        SetData(LemmingActionType.JumperAction, JumperActionName, JumperActionSpriteFileName, JumperAnimationFrames);
        SetData(LemmingActionType.SwimmerAction, SwimmerActionName, SwimmerActionSpriteFileName, SwimmerAnimationFrames);
        SetData(LemmingActionType.ShimmierAction, ShimmierActionName, ShimmierActionSpriteFileName, ShimmierAnimationFrames);
        SetData(LemmingActionType.LasererAction, LasererActionName, LasererActionSpriteFileName, LasererAnimationFrames);
        SetData(LemmingActionType.SliderAction, SliderActionName, SliderActionSpriteFileName, SliderAnimationFrames);
        SetData(LemmingActionType.FallerAction, FallerActionName, FallerActionSpriteFileName, FallerAnimationFrames);
        SetData(LemmingActionType.AscenderAction, AscenderActionName, AscenderActionSpriteFileName, AscenderAnimationFrames);
        SetData(LemmingActionType.ShruggerAction, ShruggerActionName, ShruggerActionSpriteFileName, ShruggerAnimationFrames);
        SetData(LemmingActionType.DrownerAction, DrownerActionName, DrownerActionSpriteFileName, DrownerAnimationFrames);
        SetData(LemmingActionType.HoisterAction, HoisterActionName, HoisterActionSpriteFileName, HoisterAnimationFrames);
        SetData(LemmingActionType.DehoisterAction, DehoisterActionName, DehoisterActionSpriteFileName, DehoisterAnimationFrames);
        SetData(LemmingActionType.ReacherAction, ReacherActionName, ReacherActionSpriteFileName, ReacherAnimationFrames);
        SetData(LemmingActionType.DisarmerAction, DisarmerActionName, DisarmerActionSpriteFileName, DisarmerAnimationFrames);
        SetData(LemmingActionType.ExiterAction, ExiterActionName, ExiterActionSpriteFileName, ExiterAnimationFrames);
        SetData(LemmingActionType.ExploderAction, ExploderActionName, ExploderActionSpriteFileName, ExploderAnimationFrames);
        SetData(LemmingActionType.OhNoerAction, OhNoerActionName, OhNoerActionSpriteFileName, OhNoerAnimationFrames);
        SetData(LemmingActionType.SplatterAction, SplatterActionName, SplatterActionSpriteFileName, SplatterAnimationFrames);
        SetData(LemmingActionType.StonerAction, StonerActionName, StonerActionSpriteFileName, StonerAnimationFrames);
        SetData(LemmingActionType.VaporiserAction, VaporiserActionName, VaporiserActionSpriteFileName, VaporiserAnimationFrames);
        SetData(LemmingActionType.RotateClockwiseAction, RotateClockwiseActionName, RotateClockwiseActionSpriteFileName, RotateClockwiseAnimationFrames);
        SetData(LemmingActionType.RotateCounterclockwiseAction, RotateCounterclockwiseActionName, RotateCounterclockwiseActionSpriteFileName, RotateCounterclockwiseAnimationFrames);
        SetData(LemmingActionType.RotateHalfAction, RotateHalfActionName, RotateHalfActionSpriteFileName, RotateHalfAnimationFrames);

        if (count != NumberOfLemmingActions)
            throw new Exception("Need to update this collection with new actions!");

        return result;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetData(LemmingActionType index, string lemmingActionName, string lemmingActionFileName, int numberOfAnimationFrames)
        {
            result[(int)index] = new LemmingActionLookupData(lemmingActionName, lemmingActionFileName, numberOfAnimationFrames);
            count++;
        }
    }

    public static LemmingActionLookupData GetLemmingActionDataFromId(LemmingActionType lemmingActionType) => LemmingActionTypeToStringLookup[(int)lemmingActionType];

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
    private static readonly RectangularRegion[] _lemmingActionBounds = GetLemmingActionBounds();

    private static RectangularRegion[] GetLemmingActionBounds()
    {
        var lemmingActionBounds = new RectangularRegion[LemmingActionConstants.NumberOfLemmingActions];

        var span = new Span<RectangularRegion>(lemmingActionBounds);
        span.Fill(StandardLemmingBounds);

        span.At((int)LemmingActionType.ClimberAction) = ClimberActionBounds;
        span.At((int)LemmingActionType.BlockerAction) = BlockerActionBounds;
        span.At((int)LemmingActionType.MinerAction) = MinerActionBounds;
        span.At((int)LemmingActionType.DiggerAction) = DiggerActionBounds;
        span.At((int)LemmingActionType.PlatformerAction) = PlatformerActionBounds;
        span.At((int)LemmingActionType.GliderAction) = GliderActionBounds;
        span.At((int)LemmingActionType.JumperAction) = JumperActionBounds;
        span.At((int)LemmingActionType.SwimmerAction) = SwimmerActionBounds;
        span.At((int)LemmingActionType.ShimmierAction) = ShimmierActionBounds;
        span.At((int)LemmingActionType.SliderAction) = ClimberActionBounds;
        span.At((int)LemmingActionType.HoisterAction) = HoisterActionBounds;
        span.At((int)LemmingActionType.DehoisterAction) = HoisterActionBounds;
        span.At((int)LemmingActionType.ReacherAction) = ReacherActionBounds;
        span.At((int)LemmingActionType.DisarmerAction) = DisarmerActionBounds;
        span.At((int)LemmingActionType.SplatterAction) = SplatterActionBounds;
        span.At((int)LemmingActionType.VaporiserAction) = VaporiserActionBounds;

        return lemmingActionBounds;
    }

    private static RectangularRegion StandardLemmingBounds => new(new Point(-3, -10), new Point(3, 0));

    private static RectangularRegion BlockerActionBounds => new(new Point(-6, -10), new Point(6, 0));
    private static RectangularRegion ClimberActionBounds => new(new Point(-6, -10), new Point(0, 0));
    private static RectangularRegion DiggerActionBounds => new(new Point(-3, -5), new Point(3, 0));
    private static RectangularRegion DisarmerActionBounds => new(new Point(-3, -8), new Point(3, 0));
    private static RectangularRegion GliderActionBounds => new(new Point(-3, -12), new Point(3, 0));
    private static RectangularRegion HoisterActionBounds => new(new Point(-5, -10), new Point(1, 1));
    private static RectangularRegion JumperActionBounds => new(new Point(-1, -9), new Point(3, 0));
    private static RectangularRegion MinerActionBounds => new(new Point(-2, -10), new Point(4, 0));
    private static RectangularRegion PlatformerActionBounds => new(new Point(-3, -5), new Point(3, 0));
    private static RectangularRegion ReacherActionBounds => new(new Point(-3, -9), new Point(3, 0));
    private static RectangularRegion ShimmierActionBounds => new(new Point(-3, -9), new Point(3, 2));
    private static RectangularRegion SplatterActionBounds => new(new Point(-3, -6), new Point(3, 0));
    private static RectangularRegion SwimmerActionBounds => new(new Point(-7, -4), new Point(5, 0));
    private static RectangularRegion VaporiserActionBounds => new(new Point(-3, -12), new Point(3, 2));

    public static RectangularRegion GetBounds(LemmingActionType actionType)
    {
        if ((uint)actionType < LemmingActionConstants.NumberOfLemmingActions)
            return _lemmingActionBounds.At((int)actionType);

        return StandardLemmingBounds;
    }
}
