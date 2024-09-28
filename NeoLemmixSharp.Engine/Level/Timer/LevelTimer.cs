using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Timer;

public sealed class LevelTimer
{
    private const int NumberOfTimerChars = 6;

    private readonly int _timeLimitInSeconds;

    private int _previousElapsedSeconds;
    private int _elapsedSeconds;
    private int _additionalSeconds;

    private TimerCharBuffer _timerCharBuffer;

    public TimerType Type { get; }
    public Color FontColor { get; private set; }

    public static LevelTimer CreateCountUpLevelTimer()
    {
        return new LevelTimer();
    }

    public static LevelTimer CreateCountDownLevelTimer(int timeLimitInSeconds)
    {
        return new LevelTimer(timeLimitInSeconds);
    }

    private LevelTimer()
    {
        Type = TimerType.CountUp;
        _timeLimitInSeconds = -1;
        FontColor = LevelConstants.PanelGreen;
        _timerCharBuffer[0] = ' ';
        _timerCharBuffer[1] = '0';
        _timerCharBuffer[2] = '0';
        _timerCharBuffer[3] = '-';
        _timerCharBuffer[4] = '0';
        _timerCharBuffer[5] = '0';
    }

    private LevelTimer(int timeLimitInSeconds)
    {
        Type = TimerType.CountDown;
        _timeLimitInSeconds = timeLimitInSeconds;
        FontColor = LevelConstants.PanelGreen;
        _timerCharBuffer[0] = ' ';
        _timerCharBuffer[1] = '0';
        _timerCharBuffer[2] = '0';
        _timerCharBuffer[3] = '-';
        _timerCharBuffer[4] = '0';
        _timerCharBuffer[5] = '0';
        UpdateCountDownString(timeLimitInSeconds, false);
        FontColor = GetColorForTime(timeLimitInSeconds);
    }

    public void AddAdditionalSeconds(int additionalSeconds)
    {
        _additionalSeconds += additionalSeconds;

        if (Type == TimerType.CountDown)
        {
            UpdateCountDown(false);
        }
        else
        {
            UpdateCountUp(false);
        }
    }

    public void SetElapsedTicks(int elapsedTicks, bool partialUpdate)
    {
        _previousElapsedSeconds = _elapsedSeconds;
        _elapsedSeconds = elapsedTicks / EngineConstants.FramesPerSecond;

        if (_previousElapsedSeconds == _elapsedSeconds)
            return;

        if (Type == TimerType.CountDown)
        {
            UpdateCountDown(partialUpdate);
        }
        else
        {
            UpdateCountUp(partialUpdate);
        }
    }

    private void UpdateCountDown(bool partialUpdate)
    {
        var secondsLeft = _additionalSeconds + _timeLimitInSeconds - _elapsedSeconds;

        FontColor = GetColorForTime(secondsLeft);

        if (secondsLeft < 0)
        {
            UpdateCountUpString(-secondsLeft, partialUpdate);
        }
        else
        {
            UpdateCountDownString(secondsLeft, partialUpdate);
        }
    }

    private void UpdateCountUp(bool partialUpdate)
    {
        UpdateCountUpString(_elapsedSeconds, partialUpdate);
    }

    public ReadOnlySpan<char> AsReadOnlySpan() => _timerCharBuffer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateCountUpString(
        int elapsedSeconds,
        bool partialUpdate)
    {
        UpdateTimerString(elapsedSeconds, 0, 0, 0, 0, partialUpdate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateCountDownString(
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

    private static Color GetColorForTime(int secondsLeft) => secondsLeft switch
    {
        <= 0 => LevelConstants.PanelMagenta,
        <= 15 => LevelConstants.PanelRed,
        <= 30 => LevelConstants.PanelYellow,
        _ => LevelConstants.PanelGreen
    };

    [InlineArray(NumberOfTimerChars)]
    private struct TimerCharBuffer
    {
        private char _firstElement;
    }
}