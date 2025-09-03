namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public unsafe interface ISnapshotDataConvertible
{
    int GetRequiredNumberOfBytesForSnapshotting();
    int WriteToSnapshotData(byte* snapshotDataPointer);
    int SetFromSnapshotData(byte* snapshotDataPointer);    
}
