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
    public static TimeSpan FramesPerSecondTimeSpan => TimeSpan.FromTicks(FramesPerSecondInTicks);

    public const int DoubleTapFrameCountMax = 17;

    public const int PageTransitionDurationInFrames = 4;

    public const Keys FullscreenKey = Keys.F12;

    #endregion

    #region Engine Constants

    public const int MaxNumberOfTribes = 6;
    public const int ClassicTribeId = 0;

    public const int TrivialSkillLimit = 1_000_000_000;
    public const int InfiniteSkillCount = 100;
    public const int TrivialTimeLimitInSeconds = 1_000_000_000;
    public const int MaxFiniteSkillCount = InfiniteSkillCount - 1;

    public const int InitialLemmingHatchReleaseCountDown = 20;

    public const int CursorSizeInPixels = 16;
    public const int HalfCursorSizeInPixels = CursorSizeInPixels / 2;

    public const int MaxFallDistance = 62;

    public const int DefaultCountDownTimer = 5;
    public const int DefaultCountDownActionTicks = DefaultCountDownTimer * GameplayTicksPerSecond;
    public const int DefaultFastForwardLemmingCountDownActionTicks = FastForwardSpeedMultiplier * DefaultCountDownActionTicks;

    public const int ParticleFrameCount = 51;
    public const int NumberOfParticles = 80;

    public const uint MinAllowedSpawnInterval = 4;
    public const uint MaxAllowedSpawnInterval = 102;

    public const int MaxNumberOfLemmings = 2048;
    public const int MaxTimeLimitInSeconds = 99 * 60 + 59; // 99 minutes, 59 seconds
    public const int MaxLevelWidth = 2400;
    public const int MaxLevelHeight = 2400;

    public const int FloaterGliderStartCycleFrame = 9;

    public const int SkillCountChangeBehaviourOverrideValueNullTribe = MaxNumberOfTribes + 1;
    public const int SkillCountChangeBehaviourNoOverrideValue = MaxNumberOfTribes + 2;

    public const int TrivialBehaviourTriggerLimit = 1_000_000_000;
    public const int ExpectedNumberOfHatchGadgetStates = 4;
    public const int MaxAllowedNumberOfGadgetStates = 16;
    public const int MaxAllowedNumberOfGadgetTriggers = 64;

    /// <summary>
    /// A lemming falls 3 pixels each frame
    /// </summary>
    public const int DefaultFallStep = 3;

    /// <summary>
    /// A lemming can step up a maximum of 6 pixels
    /// </summary>
    public const int MaxStepUp = 6;

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

    public static Color ClassicLevelBackgroundColor => new(0xff000032);

    public const int CursorRadius = 6;
    public static Color PanelRed => new(PanelRedValue);
    public static Color PanelGreen => new(PanelGreenValue);
    public static Color PanelYellow => new(PanelGreenValue);
    public static Color PanelBlue => new(PanelBlueValue);
    public static Color PanelMagenta => new(PanelMagentaValue);
    public static Color PanelCyan => new(PanelCyanValue);
    public static Color PanelWhite => new(PanelWhiteValue);

    public const uint PanelRedValue = 0xff0000b0;
    public const uint PanelGreenValue = 0xff00b000;
    public const uint PanelYellowValue = 0xff00b0b0;
    public const uint PanelBlueValue = 0xffb00000;
    public const uint PanelMagentaValue = 0xffb000b0;
    public const uint PanelCyanValue = 0xffb0b000;
    public const uint PanelWhiteValue = 0xffb0b0b0;

    public static Color CursorColor1 => PanelWhite;
    public static Color CursorColor2 => PanelRed;
    public static Color CursorColor3 => new(0xff606060);

    #endregion

    #region Gadget Constants

    public const int NumberOfAllowedStatesForFunctionalGadgets = 2;

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

    #region Menu Constants

    public const string LevelLoadingDisplayString = "Loading";
    public const string LevelLoadingErrorOccurredDisplayString = "ERROR OCCURRED WHEN LOADING";

    #endregion
}
