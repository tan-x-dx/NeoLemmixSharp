using Microsoft.Xna.Framework.Input;

namespace NeoLemmixSharp.Common;

public static class EngineConstants
{
    public const int StandardTicksPerSecond = 17;
    public const int FastForwardSpeedMultiplier = 3;
    public const int TicksPerSecond = StandardTicksPerSecond * FastForwardSpeedMultiplier;

    private const long FramesPerSecondInTicks = (long)(TimeSpan.TicksPerMillisecond * (1000d / (double)TicksPerSecond));
    public static TimeSpan FramesPerSecondTimeSpan => TimeSpan.FromTicks(FramesPerSecondInTicks);

    public const int DoubleTapFrameCountMax = 17;

    public const int PageTransitionDurationInFrames = 4;

    public const Keys FullscreenKey = Keys.F12;
}