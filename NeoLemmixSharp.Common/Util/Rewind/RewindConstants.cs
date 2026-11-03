namespace NeoLemmixSharp.Common.Util.Rewind;

public static class RewindConstants
{
    private const int NumberOfSecondsBetweenSnapshots = 2;
    private const int InitialNumberOfSecondsOfSnapshotData = 4 * 60;

    public const int RewindSnapshotInterval = NumberOfSecondsBetweenSnapshots * EngineConstants.EngineTicksPerSecond;
    private const int InitialSnapshotDataBufferMultiplier = InitialNumberOfSecondsOfSnapshotData * EngineConstants.EngineTicksPerSecond;

    internal const int SnapshotDataListSizeMultiplier = InitialSnapshotDataBufferMultiplier / RewindSnapshotInterval;
}
