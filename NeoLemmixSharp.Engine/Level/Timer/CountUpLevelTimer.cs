namespace NeoLemmixSharp.Engine.Level.Timer;

public sealed class CountUpLevelTimer : LevelTimer
{
    public CountUpLevelTimer()
        : base(TimerType.CountUp)
    {
        var charSpan = Chars();

        charSpan[0] = ' ';
        charSpan[1] = '0';
        charSpan[2] = '0';
        charSpan[4] = '0';
        charSpan[5] = '0';

        FontColor = LevelConstants.PanelGreen;
    }

    protected override void UpdateAppearance()
    {
        UpdateCountUpString(ElapsedSeconds, true);
    }
}