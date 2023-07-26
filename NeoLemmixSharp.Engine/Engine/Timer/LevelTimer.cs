using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Engine.Timer;

public abstract class LevelTimer
{
    // We do things this way to avoid lots of string allocations.
    // The text we want to render is only 6 characters anyway
    protected readonly int[] Chars = new int[6];
    protected int ElapsedSeconds;

    public int ElapsedTicks { get; protected set; }

    protected LevelTimer()
    {
        Chars[3] = '-';
    }

    public abstract void Tick();
    public ReadOnlySpan<int> AsSpan() => new(Chars);

    protected void UpdateCountUpString(
        int elapsedSeconds,
        bool partialUpdate = true)
    {
        var seconds = elapsedSeconds % 60;
        var secondsUnits = seconds % 10;
        Chars[5] = DigitToChar(secondsUnits);

        if (partialUpdate && secondsUnits > 0)
            return;

        var secondsTens = seconds / 10;
        Chars[4] = DigitToChar(secondsTens);

        if (partialUpdate && secondsTens > 0)
            return;

        var minutes = elapsedSeconds / 60;
        var minutesUnits = minutes % 10;
        Chars[2] = DigitToChar(minutesUnits);

        if (partialUpdate && minutesUnits > 0)
            return;

        var minutesTens = (minutes / 10) % 10;
        Chars[1] = DigitToChar(minutesTens);

        if (partialUpdate && minutesTens > 0)
            return;

        if (minutes < 100)
        {
            Chars[0] = ' ';
        }
        else
        {
            Chars[0] = DigitToChar(minutes / 100);
        }
    }

    protected void UpdateCountDownString(
        int elapsedSeconds,
        bool partialUpdate = true)
    {
        var seconds = elapsedSeconds % 60;
        var secondsUnits = seconds % 10;
        Chars[5] = DigitToChar(secondsUnits);

        if (partialUpdate && secondsUnits < 9)
            return;

        var secondsTens = seconds / 10;
        Chars[4] = DigitToChar(secondsTens);

        if (partialUpdate && secondsTens < 5)
            return;

        var minutes = elapsedSeconds / 60;
        var minutesUnits = minutes % 10;
        Chars[2] = DigitToChar(minutesUnits);

        if (partialUpdate && minutesUnits < 9)
            return;

        var minutesTens = (minutes / 10) % 10;
        Chars[1] = DigitToChar(minutesTens);

        if (partialUpdate && minutesTens < 9)
            return;

        if (minutes < 100)
        {
            Chars[0] = ' ';
        }
        else
        {
            Chars[0] = DigitToChar(minutes / 100);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DigitToChar(int digit) => digit + '0';
}