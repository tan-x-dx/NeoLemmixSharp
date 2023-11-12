using Microsoft.Xna.Framework.Input;

namespace NeoLemmixSharp.Common;

public static class EngineConstants
{
    public const int StandardTicksPerSecond = 17;
    public const int FramesPerSecond = StandardTicksPerSecond * 3;
    public static TimeSpan FramesPerSecondTimeSpan => TimeSpan.FromTicks((long)(TimeSpan.TicksPerMillisecond * (1000 / (double)FramesPerSecond)));

    public const int DoubleTapFrameCountMax = 17;

    public const int PageTransitionDurationInFrames = 4;

    public const Keys FullscreenKey = Keys.F12;
}