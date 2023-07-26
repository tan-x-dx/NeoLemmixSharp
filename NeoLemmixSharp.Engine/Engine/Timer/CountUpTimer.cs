namespace NeoLemmixSharp.Engine.Engine.Timer;

public sealed class CountUpTimer : ITimer
{
    private readonly int[] _chars = { ' ', '0', '0', ':', '0', '0' };

    private int _elapsedSeconds;
    public int ElapsedTicks { get; private set; }

    public CountUpTimer()
    {
    }

    public void Tick()
    {
        ElapsedTicks++;
        if (ElapsedTicks % GameConstants.FramesPerSecond == 0)
        {
            _elapsedSeconds++;
            UpdateString(_elapsedSeconds);
        }
    }

    /// <summary>
    /// We do things this way to avoid lots of string allocations.
    /// The text we want to render is only 6 characters anyway
    /// </summary>
    private void UpdateString(int elapsedSeconds, bool partialUpdate = true)
    {
        var seconds = elapsedSeconds % 60;
        var secondsUnits = seconds % 10;
        _chars[5] = DigitToChar(secondsUnits);

        if (partialUpdate && secondsUnits > 0)
            return;

        var secondsTens = seconds / 10;
        _chars[4] = DigitToChar(secondsTens);

        if (partialUpdate && secondsTens > 0)
            return;

        var minutes = elapsedSeconds / 60;
        var minutesUnits = minutes % 10;
        _chars[2] = DigitToChar(minutesUnits);

        if (partialUpdate && minutesUnits > 0)
            return;

        var minutesTens = (minutes / 10) % 10;
        _chars[1] = DigitToChar(minutesTens);

        if (partialUpdate && minutesTens > 0)
            return;

        if (minutes < 100)
        {
            _chars[0] = ' ';
        }
        else
        {
            _chars[0] = DigitToChar(minutes / 100);
        }
    }

    public ReadOnlySpan<int> AsSpan() => new(_chars);

    private static int DigitToChar(int digit) => digit + '0';
}