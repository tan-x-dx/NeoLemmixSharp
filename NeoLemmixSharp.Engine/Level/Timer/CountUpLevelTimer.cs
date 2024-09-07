namespace NeoLemmixSharp.Engine.Level.Timer;

public sealed class CountUpLevelTimer : LevelTimer
{
    public CountUpLevelTimer()
        : base(TimerType.CountUp)
    {
        FontColor = LevelConstants.PanelGreen;
    }

    protected override void UpdateAppearance()
    {
        UpdateCountUpString(ElapsedSeconds, true);
    }
}