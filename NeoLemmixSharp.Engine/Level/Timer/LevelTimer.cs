using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Level.Rewind;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Timer;

public sealed class LevelTimer : ISnapshotDataConvertible
{
    private const int NumberOfTimerChars = 5; // "00-00"
    private const char TimerSeparatorChar = '-';

    private int _elapsedSeconds;
    private LevelTimerData _data;

    private LevelTimerCharBuffer _charBuffer;

    public int TimeLimitInSeconds { get; }
    public TimerType Type { get; }
    public Color FontColor { get; private set; }

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

    public static LevelTimer CreateCountUpTimer() => new();
    public static LevelTimer CreateCountDownTimer(uint timeLimitInSeconds) => new(timeLimitInSeconds);

    private LevelTimer(TimerType timerType)
    {
        _charBuffer = new LevelTimerCharBuffer();
        _charBuffer[2] = TimerSeparatorChar;

        Type = timerType;
    }

    private LevelTimer() : this(TimerType.CountUp)
    {
        TimeLimitInSeconds = -1;

        FontColor = EngineConstants.PanelGreen;
        UpdateTimerString(0);
    }

    private LevelTimer(uint timeLimitInSeconds) : this(TimerType.CountDown)
    {
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
        uint seconds = elapsedSeconds % 60;
        uint secondsUnits = seconds % 10;
        _charBuffer[4] = TextRenderingHelpers.DigitToChar(secondsUnits);

        uint secondsTens = seconds / 10;
        _charBuffer[3] = TextRenderingHelpers.DigitToChar(secondsTens);

        uint minutes = elapsedSeconds / 60;
        uint minutesUnits = minutes % 10;
        _charBuffer[1] = TextRenderingHelpers.DigitToChar(minutesUnits);

        uint minutesTens = (minutes / 10) % 10;
        _charBuffer[0] = TextRenderingHelpers.DigitToChar(minutesTens);
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

        UpdateTimerString();
    }

    private static Color GetColorForTime(int secondsLeft) => secondsLeft switch
    {
        <= 0 => EngineConstants.PanelMagenta,
        <= 15 => EngineConstants.PanelRed,
        <= 30 => EngineConstants.PanelYellow,
        _ => EngineConstants.PanelGreen
    };

    [InlineArray(NumberOfTimerChars)]
    private struct LevelTimerCharBuffer
    {
        private char _0;
    }
}
