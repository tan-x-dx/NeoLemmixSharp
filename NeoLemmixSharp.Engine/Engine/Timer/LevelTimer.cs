using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Engine.Timer;

public abstract class LevelTimer
{
    // We do things this way to avoid lots of string allocations.
    // The text we want to render is only 6 characters anyway
    protected readonly int[] Chars = new int[6];
    protected int ElapsedSeconds;

    public Color FontColor { get; protected set; }

    public int ElapsedTicks { get; protected set; }

    protected LevelTimer()
    {
        Chars[3] = '-';
    }

    public void Tick()
    {
        ElapsedTicks++;
        if (ElapsedTicks % GameConstants.FramesPerSecond != 0)
            return;

        ElapsedSeconds++;
        UpdateAppearance();
    }

    public void SetElapsedTicks(int elapsedTicks)
    {
        ElapsedTicks = elapsedTicks;
        ElapsedSeconds = elapsedTicks / GameConstants.FramesPerSecond;

        UpdateAppearance();
    }

    protected abstract void UpdateAppearance();

    public ReadOnlySpan<int> AsSpan() => new(Chars);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void UpdateCountUpString(
        int elapsedSeconds,
        bool partialUpdate)
    {
        UpdateTimerString(elapsedSeconds, 0, 0, 0, 0, partialUpdate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void UpdateCountDownString(
        int elapsedSeconds,
        bool partialUpdate)
    {
        UpdateTimerString(elapsedSeconds, 9, 5, 9, 9, partialUpdate);
    }

    private void UpdateTimerString(
        int elapsedSeconds,
        int secondUnitsTest,
        int secondTensTest,
        int minutesUnitsTest,
        int minutesTensTest,
        bool partialUpdate)
    {
        var seconds = elapsedSeconds % 60;
        var secondsUnits = seconds % 10;
        Chars[5] = DigitToChar(secondsUnits);

        if (partialUpdate && secondsUnits != secondUnitsTest)
            return;

        var secondsTens = seconds / 10;
        Chars[4] = DigitToChar(secondsTens);

        if (partialUpdate && secondsTens != secondTensTest)
            return;

        var minutes = elapsedSeconds / 60;
        var minutesUnits = minutes % 10;
        Chars[2] = DigitToChar(minutesUnits);

        if (partialUpdate && minutesUnits != minutesUnitsTest)
            return;

        var minutesTens = (minutes / 10) % 10;
        Chars[1] = DigitToChar(minutesTens);

        if (partialUpdate && minutesTens != minutesTensTest)
            return;

        Chars[0] = minutes < 100
            ? ' '
            : DigitToChar(minutes / 100);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DigitToChar(int digit) => digit + '0';
}