namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public readonly struct LemmingStateSnapshotData
{
    public readonly int TeamId;
    public readonly uint StateData;

    public LemmingStateSnapshotData(int teamId, uint stateData)
    {
        TeamId = teamId;
        StateData = stateData;
    }
}