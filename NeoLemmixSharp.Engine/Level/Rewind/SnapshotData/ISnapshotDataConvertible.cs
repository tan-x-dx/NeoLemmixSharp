namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public unsafe interface ISnapshotDataConvertible<TSnapshotData>
    where TSnapshotData : unmanaged
{
    int GetRequiredNumberOfBytesForSnapshotting();
    int WriteToSnapshotData(TSnapshotData* snapshotDataPointer);
    int SetFromSnapshotData(TSnapshotData* snapshotDataPointer);    
}
