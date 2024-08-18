using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Timer;

public abstract class LevelTimer
{
    private const int NumberOfTimerChars = 6;

    public readonly TimerType Type;
    protected int ElapsedSeconds;

    private TimerCharBuffer _timerCharBuffer;

    public Color FontColor { get; protected set; }

    protected LevelTimer(TimerType type)
    {
        _timerCharBuffer[3] = '-';
        Type = type;
    }

    protected Span<int> Chars() => _timerCharBuffer;

    public void Tick()
    {
        // ElapsedTicks++;
        // if (ElapsedTicks % LevelConstants.FramesPerSecond != 0)
        //     return;

        ElapsedSeconds++;
        UpdateAppearance();
    }

    public void SetElapsedTicks(int elapsedTicks)
    {
        //   ElapsedTicks = elapsedTicks;
        ElapsedSeconds = elapsedTicks / EngineConstants.FramesPerSecond;

        UpdateAppearance();
    }

    protected abstract void UpdateAppearance();

    public ReadOnlySpan<int> AsSpan() => _timerCharBuffer;

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
        var charSpan = Chars();

        var seconds = elapsedSeconds % 60;
        var secondsUnits = seconds % 10;
        charSpan[5] = DigitToChar(secondsUnits);

        if (partialUpdate && secondsUnits != secondUnitsTest)
            return;

        var secondsTens = seconds / 10;
        charSpan[4] = DigitToChar(secondsTens);

        if (partialUpdate && secondsTens != secondTensTest)
            return;

        var minutes = elapsedSeconds / 60;
        var minutesUnits = minutes % 10;
        charSpan[2] = DigitToChar(minutesUnits);

        if (partialUpdate && minutesUnits != minutesUnitsTest)
            return;

        var minutesTens = (minutes / 10) % 10;
        charSpan[1] = DigitToChar(minutesTens);

        if (partialUpdate && minutesTens != minutesTensTest)
            return;

        charSpan[0] = minutes < 100
            ? ' '
            : DigitToChar(minutes / 100);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DigitToChar(int digit) => digit + '0';

    public enum TimerType
    {
        CountDown,
        CountUp
    }

    [InlineArray(NumberOfTimerChars)]
    private struct TimerCharBuffer
    {
        private int _firstElement;
    }
}