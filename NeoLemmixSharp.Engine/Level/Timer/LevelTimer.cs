using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Rewind;

namespace NeoLemmixSharp.Engine.Level.Timer;

public unsafe sealed class LevelTimer : ISnapshotDataConvertible, IDisposable
{
    private const int NumberOfTimerChars = 5; // 00:00
    private const char TimerSeparatorChar = '-';

    private readonly char* _timerCharPointer;
    private readonly RawArray _byteBuffer;

    private int _elapsedSeconds;
    private LevelTimerData _data;

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

    public ReadOnlySpan<char> AsReadOnlySpan() => new(_timerCharPointer, NumberOfTimerChars);

    public static LevelTimer CreateCountUpTimer() => new();
    public static LevelTimer CreateCountDownTimer(uint timeLimitInSeconds) => new(timeLimitInSeconds);

    private LevelTimer(TimerType timerType)
    {
        _byteBuffer = Helpers.AllocateBuffer<char>(NumberOfTimerChars);
        _timerCharPointer = (char*)_byteBuffer.Handle;
        _timerCharPointer[2] = TimerSeparatorChar;

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
        char* pointer = _timerCharPointer;

        uint seconds = elapsedSeconds % 60;
        uint secondsUnits = seconds % 10;
        pointer[4] = TextRenderingHelpers.DigitToChar(secondsUnits);

        uint secondsTens = seconds / 10;
        pointer[3] = TextRenderingHelpers.DigitToChar(secondsTens);

        uint minutes = elapsedSeconds / 60;
        uint minutesUnits = minutes % 10;
        pointer[1] = TextRenderingHelpers.DigitToChar(minutesUnits);

        uint minutesTens = (minutes / 10) % 10;
        pointer[0] = TextRenderingHelpers.DigitToChar(minutesTens);
    }

    public int GetRequiredNumberOfBytesForSnapshotting() => sizeof(LevelTimerData);

    public void WriteToSnapshotData(byte* snapshotDataPointer)
    {
        LevelTimerData* levelTimerSnapshotDataPointer = (LevelTimerData*)snapshotDataPointer;

        *levelTimerSnapshotDataPointer = _data;
    }

    public void SetFromSnapshotData(byte* snapshotDataPointer)
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

    public void Dispose()
    {
        _byteBuffer.Dispose();
    }
}
