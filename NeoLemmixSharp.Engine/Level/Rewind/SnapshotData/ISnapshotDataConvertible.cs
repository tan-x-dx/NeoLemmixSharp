namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public interface ISnapshotDataConvertible<TSnapshotData>
    where TSnapshotData : unmanaged
{
    void WriteToSnapshotData(out TSnapshotData snapshotData);
    void SetFromSnapshotData(in TSnapshotData snapshotData);
}
