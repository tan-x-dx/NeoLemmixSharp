namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public readonly struct LemmingManagerSnapshotData
{
    public readonly int NumberOfLemmingsReleasedFromHatch;
    public readonly int NumberOfClonedLemmings;

    public readonly int LemmingsToRelease;
    public readonly int LemmingsOut;
    public readonly int LemmingsRemoved;
    public readonly int LemmingsSaved;

    public LemmingManagerSnapshotData(int numberOfLemmingsReleasedFromHatch, int numberOfClonedLemmings, int lemmingsToRelease, int lemmingsOut, int lemmingsRemoved, int lemmingsSaved)
    {
        NumberOfLemmingsReleasedFromHatch = numberOfLemmingsReleasedFromHatch;
        NumberOfClonedLemmings = numberOfClonedLemmings;
        LemmingsToRelease = lemmingsToRelease;
        LemmingsOut = lemmingsOut;
        LemmingsRemoved = lemmingsRemoved;
        LemmingsSaved = lemmingsSaved;
    }
}