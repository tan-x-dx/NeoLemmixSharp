using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.PositionTracking;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

public static class EngineConstants
{
    #region Engine Specifications

    public const int GameplayTicksPerSecond = 17;
    public const int FastForwardSpeedMultiplier = 3;
    public const int EngineTicksPerSecond = GameplayTicksPerSecond * FastForwardSpeedMultiplier;

    private const long FramesPerSecondInTicks = (long)(TimeSpan.TicksPerMillisecond * (1000d / (double)EngineTicksPerSecond));
    public static readonly TimeSpan FramesPerSecondTimeSpan = TimeSpan.FromTicks(FramesPerSecondInTicks);

    public const int DoubleTapFrameCountMax = 17;

    public const int PageTransitionDurationInFrames = 4;

    public const Keys FullscreenKey = Keys.F12;

    #endregion

    #region Engine Constants

    public const int NumberOfFacingDirections = 2;

    public const int RightFacingDirectionId = 0;
    public const string RightFacingDirectionName = "Right";

    public const int LeftFacingDirectionId = 1;
    public const string LeftFacingDirectionName = "Left";

    public const int NumberOfOrientations = 4;

    public const int DownOrientationRotNum = 0;
    public const string DownOrientationName = "Down";
    public const float DownOrientationRotationAngle = 0.0f;

    public const int LeftOrientationRotNum = 1;
    public const string LeftOrientationName = "Left";
    public const float LeftOrientationRotationAngle = MathF.PI * 1.5f;

    public const int UpOrientationRotNum = 2;
    public const string UpOrientationName = "Up";
    public const float UpOrientationRotationAngle = MathF.PI;

    public const int RightOrientationRotNum = 3;
    public const string RightOrientationName = "Right";
    public const float RightOrientationRotationAngle = MathF.PI * 0.5f;

    public const int MaxNumberOfTeams = 6;
    public const int ClassicTeamId = 0;

    public const int InfiniteSkillCount = 100;

    public const int InitialLemmingHatchReleaseCountDown = 20;

    public const int CursorSizeInPixels = 16;
    public const int HalfCursorSizeInPixels = CursorSizeInPixels / 2;

    public const int MaxFallDistance = 62;

    public const int DefaultCountDownTimer = 5;
    public const int DefaultCountDownActionTicks = DefaultCountDownTimer * GameplayTicksPerSecond;
    public const int DefaultFastForwardLemmingCountDownActionTicks = FastForwardSpeedMultiplier * DefaultCountDownActionTicks;

    public const int ParticleFrameCount = 51;
    public const int NumberOfParticles = 80;

    public const int MinAllowedSpawnInterval = 4;
    public const int MaxAllowedSpawnInterval = 102;

    public const int MaxNumberOfLemmings = 2000;
    public const int MaxTimeLimitInSeconds = 99 * 60 + 59; // 99 minutes, 59 seconds
    public const int MaxLevelWidth = 2400;
    public const int MaxLevelHeight = 2400;

    public const int FloaterGliderStartCycleFrame = 9;

    /// <summary>
    /// A lemming falls 3 pixels each frame
    /// </summary>
    public const int DefaultFallStep = 3;

    /// <summary>
    /// A lemming can step up a maximum of 6 pixels
    /// </summary>
    public const int MaxStepUp = 6;

    #endregion

    #region Level IO Constants

    /// <summary>
    /// Assumption: if there are infinite skills available of a certain type,
    /// there'll probably be around this number of actual usages.
    /// </summary>
    public const int AssumedSkillUsageForInfiniteSkillCounts = 40;
    /// <summary>
    /// Assumption: if there are skill pickups in a level,
    /// there'll probably be around this number of skills added.
    /// </summary>
    public const int AssumedSkillCountsFromPickups = 10;

    /// <summary>
    /// If a style has not been used for this many levels, remove it from the cache
    /// </summary>
    public const int NumberOfLevelsToKeepStyle = 4;

    /// <summary>
    /// Assumption: a level will probably depend on this number or fewer styles
    /// </summary>
    public const int AssumedInitialStyleCapacity = 6;

    /// <summary>
    /// Assumption: a level will probably have this number of unique terrain pieces or fewer
    /// </summary>
    public const int AssumedNumberOfTerrainArchetypeDataInLevel = 32;

    /// <summary>
    /// Assumption: a level will probably have this number of unique gadget pieces or fewer
    /// </summary>
    public const int AssumedNumberOfGadgetArchetypeDataInLevel = 16;

    /// <summary>
    /// Assumption: a style will probably define this number of unique terrain pieces or fewer
    /// </summary>
    public const int AssumedNumberOfTerrainArchetypeDataInStyle = 64;

    #endregion

    #region Replay Snapshot Constants

    private const int NumberOfSecondsBetweenSnapshots = 2;
    public const int RewindSnapshotInterval = NumberOfSecondsBetweenSnapshots * EngineTicksPerSecond;
    private const int InitialNumberOfSecondsOfSnapshotData = 4 * 60;
    public const int InitialSnapshotDataBufferMultiplier = InitialNumberOfSecondsOfSnapshotData * EngineTicksPerSecond;

    #endregion

    #region Default Colours

    public const uint MinimumSubstantialAlphaValue = 0x80;

    private static ReadOnlySpan<uint> RawExplosionParticleColorsHexValues =>
    [
        0xf0e04040,
        0xf000b000,
        0xf0d0d0f0,
        0xf02020f0,
        0xc0e04040,
        0xc000b000,
        0xc0d0d0f0,
        0xc02020f0
    ];

    public static ReadOnlySpan<Color> GetExplosionParticleColors() => MemoryMarshal.Cast<uint, Color>(RawExplosionParticleColorsHexValues);
    public const int NumberOfExplosionParticleColors = 8;
    public const int NumberOfExplosionParticleColorsMask = NumberOfExplosionParticleColors - 1;

    public static readonly Color ClassicLevelBackgroundColor = new(0xff000032);

    public const int CursorRadius = 6;
    public static readonly Color CursorColor1 = new(0xffb0b0b0);
    public static readonly Color CursorColor2 = new(0xff0000b0);
    public static readonly Color CursorColor3 = new(0xff606060);

    public static readonly Color PanelBlue = new(0xffb00000);
    public static readonly Color PanelGreen = new(0xff00b000);
    public static readonly Color PanelCyan = new(0xffb0b000);
    public static readonly Color PanelRed = new(0xff0000b0);
    public static readonly Color PanelMagenta = new(0xffb000b0);
    public static readonly Color PanelYellow = new(0xff00b0b0);

    #endregion

    #region Control Panel Strings

    public const string NeutralControlPanelString = "NEUTRAL";
    public const string ZombieControlPanelString = "ZOMBIE";
    public const string NeutralZombieControlPanelString = "N-ZOMBIE";
    public const string AthleteControlPanelString2Skills = "ATHLETE";
    public const string AthleteControlPanelString3Skills = "TRIATHLETE";
    public const string AthleteControlPanelString4Skills = "TETRATHLETE";
    public const string AthleteControlPanelString5Skills = "PENTATHLETE";

    #endregion

    #region Position Tracking Data

    public const ChunkSize LemmingPositionChunkSize = ChunkSize.ChunkSize16;
    public const ChunkSize GadgetPositionChunkSize = ChunkSize.ChunkSize64;

    #endregion

    #region Lemming Brick Constants

    public const int NumberOfBuilderBricks = 12;
    public const int NumberOfPlatformerBricks = 12;
    public const int NumberOfStackerBricks = 8;
    public const int NumberOfRemainingBricksToPlaySound = 3;

    #endregion

    #region Cursor Priority Levels

    public const int NonPermanentSkillPriority = 4;
    public const int PermanentSkillPriority = 3;
    public const int NonWalkerMovementPriority = 2;
    public const int WalkerMovementPriority = 1;
    public const int NoPriority = 0;

    #endregion

    #region Lemming State Constants

    public const int ClimberBitIndex = 0;
    public const int FloaterBitIndex = 1;
    public const int GliderBitIndex = 2;
    public const int SliderBitIndex = 3;
    public const int SwimmerBitIndex = 4;
    public const int DisarmerBitIndex = 5;

    public const int AcidLemmingBitIndex = 20;
    public const int WaterLemmingBitIndex = 21;

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

    public const int PermanentFastForwardBitIndex = 28;

    public const int ActiveBitIndex = 29;
    public const int NeutralBitIndex = 30;
    public const int ZombieBitIndex = 31;

    public const uint AssignableSkillBitMask = (1U << ActiveBitIndex) |
                                               (1U << NeutralBitIndex) |
                                               (1U << ZombieBitIndex);

    #endregion

    #region Lemming Action Constants

    public const int NumberOfLemmingActions = 34;
    public const int LongestActionNameLength = 11;

    public static bool IsValidLemmingActionId(int lemmingActionId)
    {
        return (uint)lemmingActionId < NumberOfLemmingActions;
    }

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

    #endregion

    #region Lemming Skill Constants

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

    #endregion

    #region Menu Constants

    public const string LevelLoadingDisplayString = "Loading";
    public const string LevelLoadingErrorOccurredDisplayString = "ERROR OCCURRED WHEN LOADING";

    #endregion
}