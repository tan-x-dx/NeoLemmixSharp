namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public readonly struct LevelTimerSnapshotData
{
    public readonly int AdditionalSeconds;

    public LevelTimerSnapshotData(int additionalSeconds)
    {
        AdditionalSeconds = additionalSeconds;
    }
}