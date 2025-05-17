namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public readonly struct LemmingStateSnapshotData
{
    public readonly int TribeId;
    public readonly uint StateData;

    public LemmingStateSnapshotData(int tribeId, uint stateData)
    {
        TribeId = tribeId;
        StateData = stateData;
    }
}