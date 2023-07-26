using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine.Engine.Timer;

public sealed class CountDownLevelTimer : LevelTimer
{
    private static readonly Color Green = new(0x00, 0xB0, 0x00);
    private static readonly Color Yellow = new(0xB0, 0xB0, 0x00);
    private static readonly Color Red = new(0xB0, 0x00, 0x00);
    private static readonly Color Magenta = new(0xB0, 0x00, 0xB0);

    private readonly int _timeLimitInSeconds;

    public CountDownLevelTimer(int timeLimitInSeconds)
    {
        _timeLimitInSeconds = timeLimitInSeconds;

        UpdateCountDownString(_timeLimitInSeconds, false);

        FontColor = GetColorForTime(_timeLimitInSeconds);
    }

    public override void Tick()
    {
        ElapsedTicks++;
        if (ElapsedTicks % GameConstants.FramesPerSecond != 0)
            return;

        ElapsedSeconds++;
        UpdateAppearance();
    }

    protected override void UpdateAppearance()
    {
        var secondsLeft = _timeLimitInSeconds - ElapsedSeconds;

        FontColor = GetColorForTime(secondsLeft);

        if (secondsLeft < 0)
        {
            UpdateCountUpString(-secondsLeft);
        }
        else
        {
            UpdateCountDownString(secondsLeft);
        }
    }

    private static Color GetColorForTime(int secondsLeft) => secondsLeft switch
    {
        <= 0 => Magenta,
        <= 15 => Red,
        <= 30 => Yellow,
        _ => Green
    };
}