namespace NeoLemmixSharp.Engine.Engine.Timer;

public sealed class CountDownLevelTimer : LevelTimer
{
    private readonly int _timeLimitInSeconds;

    public CountDownLevelTimer(int timeLimitInSeconds)
    {
        _timeLimitInSeconds = timeLimitInSeconds;

        UpdateCountDownString(_timeLimitInSeconds, false);
    }

    public override void Tick()
    {
        ElapsedTicks++;
        if (ElapsedTicks % 2 != 0)
            return;

        ElapsedSeconds++;

        if (_timeLimitInSeconds < ElapsedSeconds)
        {
            UpdateCountUpString(ElapsedSeconds - _timeLimitInSeconds);
        }
        else
        {
            UpdateCountDownString(_timeLimitInSeconds - ElapsedSeconds);
        }
    }
}