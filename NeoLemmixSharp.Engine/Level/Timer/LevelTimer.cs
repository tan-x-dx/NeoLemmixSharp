using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Timer;

public abstract class LevelTimer
{
    private const int NumberOfTimerChars = 6;

    public readonly TimerType Type;
    private int _previousElapsedSeconds;
    protected int ElapsedSeconds;

    private TimerCharBuffer _timerCharBuffer;

    public Color FontColor { get; protected set; }

    protected LevelTimer(TimerType type)
    {
        Type = type;
        _timerCharBuffer[0] = ' ';
        _timerCharBuffer[1] = '0';
        _timerCharBuffer[2] = '0';
        _timerCharBuffer[3] = '-';
        _timerCharBuffer[4] = '0';
        _timerCharBuffer[5] = '0';
    }

    public void SetElapsedTicks(int elapsedTicks)
    {
        _previousElapsedSeconds = ElapsedSeconds;
        ElapsedSeconds = elapsedTicks / EngineConstants.FramesPerSecond;

        if (_previousElapsedSeconds != ElapsedSeconds)
        {
            UpdateAppearance();
        }
    }

    protected abstract void UpdateAppearance();

    public ReadOnlySpan<char> AsReadOnlySpan() => _timerCharBuffer;

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
        _timerCharBuffer[5] = TextRenderingHelpers.DigitToChar(secondsUnits);

        if (partialUpdate && secondsUnits != secondUnitsTest)
            return;

        var secondsTens = seconds / 10;
        _timerCharBuffer[4] = TextRenderingHelpers.DigitToChar(secondsTens);

        if (partialUpdate && secondsTens != secondTensTest)
            return;

        var minutes = elapsedSeconds / 60;
        var minutesUnits = minutes % 10;
        _timerCharBuffer[2] = TextRenderingHelpers.DigitToChar(minutesUnits);

        if (partialUpdate && minutesUnits != minutesUnitsTest)
            return;

        var minutesTens = (minutes / 10) % 10;
        _timerCharBuffer[1] = TextRenderingHelpers.DigitToChar(minutesTens);

        if (partialUpdate && minutesTens != minutesTensTest)
            return;

        _timerCharBuffer[0] = minutes < 100
            ? ' '
            : TextRenderingHelpers.DigitToChar(minutes / 100);
    }

    public enum TimerType
    {
        CountDown,
        CountUp
    }

    [InlineArray(NumberOfTimerChars)]
    private struct TimerCharBuffer
    {
        private char _firstElement;
    }
}