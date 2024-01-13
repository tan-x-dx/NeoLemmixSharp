using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine.Level.Timer;

public sealed class CountDownLevelTimer : LevelTimer
{
    private readonly int _timeLimitInSeconds;

    public CountDownLevelTimer(int timeLimitInSeconds)
        : base(TimerType.CountDown)
    {
        _timeLimitInSeconds = timeLimitInSeconds;

        UpdateCountDownString(_timeLimitInSeconds, false);

        FontColor = GetColorForTime(_timeLimitInSeconds);
    }

    protected override void UpdateAppearance()
    {
        var secondsLeft = _timeLimitInSeconds - ElapsedSeconds;

        FontColor = GetColorForTime(secondsLeft);

        if (secondsLeft < 0)
        {
            UpdateCountUpString(-secondsLeft, true);
        }
        else
        {
            UpdateCountDownString(secondsLeft, true);
        }
    }

    private static Color GetColorForTime(int secondsLeft) => secondsLeft switch
    {
        <= 0 => LevelConstants.PanelMagenta,
        <= 15 => LevelConstants.PanelRed,
        <= 30 => LevelConstants.PanelYellow,
        _ => LevelConstants.PanelGreen
    };
}