namespace NeoLemmixSharp.Engine.Level.Rewind;

public unsafe interface ISnapshotDataConvertible
{
    int GetRequiredNumberOfBytesForSnapshotting();
    void WriteToSnapshotData(byte* snapshotDataPointer);
    void SetFromSnapshotData(byte* snapshotDataPointer);
}

public unsafe interface ISnapshotDataConvertible2
{
    int GetRequiredNumberOfBytesForSnapshotting();
    int Seed(byte* seedPointer);
}
