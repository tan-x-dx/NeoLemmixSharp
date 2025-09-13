namespace NeoLemmixSharp.Engine.Level.Timer;

public struct LevelTimerData
{
    public int AdditionalSeconds;

    public LevelTimerData(int additionalSeconds)
    {
        AdditionalSeconds = additionalSeconds;
    }
}