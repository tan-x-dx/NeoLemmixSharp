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

    public const string DefaultStyleIdentifier = "default";

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

    public const int MaxNumberOfTribes = 6;
    public const int ClassicTribeId = 0;

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
    /// Assumption: a level will probably have this number of unique gadgets or fewer
    /// </summary>
    public const int AssumedNumberOfGadgetArchetypeDataInLevel = 20;

    /// <summary>
    /// Assumption: a style will probably define this number of unique terrain pieces or fewer
    /// </summary>
    public const int AssumedNumberOfTerrainArchetypeDataInStyle = 64;

    /// <summary>
    /// Assumption: a style will probably define this number of unique gadgets or fewer
    /// </summary>
    public const int AssumedNumberOfGadgetArchetypeDataInStyle = 16;

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

    #region Menu Constants

    public const string LevelLoadingDisplayString = "Loading";
    public const string LevelLoadingErrorOccurredDisplayString = "ERROR OCCURRED WHEN LOADING";

    #endregion
}