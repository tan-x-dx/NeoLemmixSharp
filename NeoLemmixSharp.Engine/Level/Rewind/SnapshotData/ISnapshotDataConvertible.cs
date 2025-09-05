namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public unsafe interface ISnapshotDataConvertible
{
    int GetRequiredNumberOfBytesForSnapshotting();
    void WriteToSnapshotData(byte* snapshotDataPointer);
    void SetFromSnapshotData(byte* snapshotDataPointer);    
}
