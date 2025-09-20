using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Level.Rewind;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Timer;

public sealed class LevelTimer : ISnapshotDataConvertible
{
    private const int NumberOfTimerChars = 5; // 00:00
    private const char TimerSeparatorChar = '-';

    private int _elapsedSeconds;
    private LevelTimerData _data;
    private TimerCharBuffer _timerCharBuffer;

    public int TimeLimitInSeconds { get; }
    public TimerType Type { get; }
    public Color FontColor { get; private set; }

    public int EffectiveSecondsRemaining => Type == TimerType.CountDown
        ? TimeLimitInSeconds + _data.AdditionalSeconds - _elapsedSeconds
        : EngineConstants.TrivialTimeLimitInSeconds;

    public int EffectiveElapsedSeconds => _elapsedSeconds - _data.AdditionalSeconds;
    public bool OutOfTime => EffectiveSecondsRemaining < 0;

    public static LevelTimer CreateCountUpTimer() => new();
    public static LevelTimer CreateCountDownTimer(int timeLimitInSeconds) => new(timeLimitInSeconds);

    private LevelTimer()
    {
        TimeLimitInSeconds = -1;

        _timerCharBuffer[2] = TimerSeparatorChar;

        Type = TimerType.CountUp;
        FontColor = EngineConstants.PanelGreen;
        UpdateCountUpString(0, false);
    }

    private LevelTimer(int timeLimitInSeconds)
    {
        TimeLimitInSeconds = timeLimitInSeconds;

        _timerCharBuffer[2] = TimerSeparatorChar;

        Type = TimerType.CountDown;
        FontColor = GetColorForTime(timeLimitInSeconds);
        UpdateCountDownString(timeLimitInSeconds, false);
    }

    public void AddAdditionalSeconds(int additionalSeconds)
    {
        _data.AdditionalSeconds += additionalSeconds;

        UpdateTimerString(false);
    }

    public void SetElapsedTicks(int elapsedTicks, bool partialUpdate)
    {
        var previousElapsedSeconds = _elapsedSeconds;
        _elapsedSeconds = elapsedTicks / EngineConstants.EngineTicksPerSecond;

        if (previousElapsedSeconds == _elapsedSeconds)
            return;

        UpdateTimerString(partialUpdate);
    }

    private void UpdateTimerString(bool partialUpdate)
    {
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
        var secondsLeft = TimeLimitInSeconds - EffectiveElapsedSeconds;

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
        UpdateTimerString((uint)elapsedSeconds, 0, 0, 0, partialUpdate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateCountDownString(
        int elapsedSeconds,
        bool partialUpdate)
    {
        UpdateTimerString((uint)elapsedSeconds, 9, 5, 9, partialUpdate);
    }

    private void UpdateTimerString(
        uint elapsedSeconds,
        uint minutesUnitsTest,
        uint secondTensTest,
        uint secondUnitsTest,
        bool partialUpdate)
    {
        var seconds = elapsedSeconds % 60;
        var secondsUnits = seconds % 10;
        _timerCharBuffer[4] = TextRenderingHelpers.DigitToChar(secondsUnits);

        if (partialUpdate && secondsUnits != secondUnitsTest)
            return;

        var secondsTens = seconds / 10;
        _timerCharBuffer[3] = TextRenderingHelpers.DigitToChar(secondsTens);

        if (partialUpdate && secondsTens != secondTensTest)
            return;

        var minutes = elapsedSeconds / 60;
        var minutesUnits = minutes % 10;
        _timerCharBuffer[1] = TextRenderingHelpers.DigitToChar(minutesUnits);

        if (partialUpdate && minutesUnits != minutesUnitsTest)
            return;

        var minutesTens = (minutes / 10) % 10;
        _timerCharBuffer[0] = TextRenderingHelpers.DigitToChar(minutesTens);
    }

    public unsafe int GetRequiredNumberOfBytesForSnapshotting() => sizeof(LevelTimerData);

    public unsafe void WriteToSnapshotData(byte* snapshotDataPointer)
    {
        LevelTimerData* levelTimerSnapshotDataPointer = (LevelTimerData*)snapshotDataPointer;

        *levelTimerSnapshotDataPointer = _data;
    }

    public unsafe void SetFromSnapshotData(byte* snapshotDataPointer)
    {
        LevelTimerData* levelTimerSnapshotDataPointer = (LevelTimerData*)snapshotDataPointer;

        _data = *levelTimerSnapshotDataPointer;

        UpdateTimerString(false);
    }

    private static Color GetColorForTime(int secondsLeft) => secondsLeft switch
    {
        <= 0 => EngineConstants.PanelMagenta,
        <= 15 => EngineConstants.PanelRed,
        <= 30 => EngineConstants.PanelYellow,
        _ => EngineConstants.PanelGreen
    };

    [InlineArray(NumberOfTimerChars)]
    private struct TimerCharBuffer
    {
        private char _0;
    }
}
