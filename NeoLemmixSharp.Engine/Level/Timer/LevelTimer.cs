using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Timer;

public sealed class LevelTimer
{
    private const int NumberOfTimerChars = 5; // "00-00"
    private const char TimerSeparatorChar = '-';

    private readonly LevelTimerDataPointer _data;

    private int _elapsedSeconds;
    public int TimeLimitInSeconds { get; }
    public TimerType Type { get; }
    public Color FontColor { get; private set; }

    private LevelTimerCharBuffer _charBuffer;

    public int EffectiveSecondsRemaining
    {
        get
        {
            var result = EngineConstants.TrivialTimeLimitInSeconds;
            if (Type == TimerType.CountDown)
                result = TimeLimitInSeconds + _data.AdditionalSeconds - _elapsedSeconds;
            return result;
        }
    }

    public int EffectiveElapsedSeconds => _elapsedSeconds - _data.AdditionalSeconds;
    public bool OutOfTime => EffectiveSecondsRemaining < 0;

    public ReadOnlySpan<char> AsReadOnlySpan() => _charBuffer;

    public static LevelTimer CreateCountUpTimer(nint dataHandle) => new(dataHandle);
    public static LevelTimer CreateCountDownTimer(nint dataHandle, uint timeLimitInSeconds) => new(dataHandle, timeLimitInSeconds);

    private LevelTimer(nint dataHandle)
    {
        _data = new LevelTimerDataPointer(dataHandle);
        _charBuffer[2] = TimerSeparatorChar;
        Type = TimerType.CountUp;
        TimeLimitInSeconds = -1;

        FontColor = EngineConstants.PanelGreen;
        UpdateTimerString(0);
    }

    private LevelTimer(nint dataHandle, uint timeLimitInSeconds)
    {
        _data = new LevelTimerDataPointer(dataHandle);
        _charBuffer[2] = TimerSeparatorChar;
        Type = TimerType.CountDown;
        TimeLimitInSeconds = (int)timeLimitInSeconds;

        FontColor = GetColorForTime((int)timeLimitInSeconds);
        UpdateTimerString(timeLimitInSeconds);
    }

    public void AddAdditionalSeconds(int additionalSeconds)
    {
        _data.AdditionalSeconds += additionalSeconds;

        UpdateTimerString();
    }

    public void SetElapsedTicks(int elapsedTicks)
    {
        var previousElapsedSeconds = _elapsedSeconds;
        var currentElapsedSeconds = elapsedTicks / EngineConstants.EngineTicksPerSecond;

        if (previousElapsedSeconds == currentElapsedSeconds)
            return;

        _elapsedSeconds = currentElapsedSeconds;

        UpdateTimerString();
    }

    private void UpdateTimerString()
    {
        if (Type == TimerType.CountDown)
        {
            UpdateCountDown();
        }
        else
        {
            UpdateCountUp();
        }
    }

    private void UpdateCountDown()
    {
        var secondsLeft = TimeLimitInSeconds - EffectiveElapsedSeconds;

        FontColor = GetColorForTime(secondsLeft);

        if (secondsLeft < 0)
            secondsLeft = -secondsLeft;

        UpdateTimerString((uint)secondsLeft);
    }

    private void UpdateCountUp()
    {
        UpdateTimerString((uint)_elapsedSeconds);
    }

    private void UpdateTimerString(uint elapsedSeconds)
    {
        (uint minutes, uint seconds) = Math.DivRem(elapsedSeconds, 60);

        (uint minutesTens, uint minutesUnits) = Math.DivRem(minutes, 10);

        _charBuffer[0] = NumberFormattingHelpers.DigitToChar(minutesTens);
        _charBuffer[1] = NumberFormattingHelpers.DigitToChar(minutesUnits);

        (uint secondsTens, uint secondsUnits) = Math.DivRem(seconds, 10);

        _charBuffer[3] = NumberFormattingHelpers.DigitToChar(secondsTens);
        _charBuffer[4] = NumberFormattingHelpers.DigitToChar(secondsUnits);
    }

    private static Color GetColorForTime(int secondsLeft)
    {
        uint packedValue = EngineConstants.PanelGreenValue;

        if (secondsLeft <= 30) packedValue = EngineConstants.PanelYellowValue;
        if (secondsLeft <= 15) packedValue = EngineConstants.PanelRedValue;
        if (secondsLeft <= 0) packedValue = EngineConstants.PanelMagentaValue;

        return new Color(packedValue);
    }

    [InlineArray(NumberOfTimerChars)]
    private struct LevelTimerCharBuffer
    {
        private char _0;
    }
}
