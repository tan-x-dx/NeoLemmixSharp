using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Rendering.Text;

namespace NeoLemmixSharp.Engine.Engine.Timer;

public sealed class CountDownLevelTimer : LevelTimer
{
    private readonly int _timeLimitInSeconds;

    public CountDownLevelTimer(int timeLimitInSeconds)
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
        <= 0 => PanelFont.Magenta,
        <= 15 => PanelFont.Red,
        <= 30 => PanelFont.Yellow,
        _ => PanelFont.Green
    };
}