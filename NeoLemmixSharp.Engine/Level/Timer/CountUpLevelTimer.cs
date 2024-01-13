namespace NeoLemmixSharp.Engine.Level.Timer;

public sealed class CountUpLevelTimer : LevelTimer
{
    public CountUpLevelTimer()
        : base(TimerType.CountUp)
    {
        Chars[0] = ' ';
        Chars[1] = '0';
        Chars[2] = '0';
        Chars[4] = '0';
        Chars[5] = '0';

        FontColor = LevelConstants.PanelGreen;
    }

    protected override void UpdateAppearance()
    {
        UpdateCountUpString(ElapsedSeconds, true);
    }
}