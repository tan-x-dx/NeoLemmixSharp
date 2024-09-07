namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public interface ISnapshotDataConvertible<TSnapshotData>
    where TSnapshotData : struct
{
    void ToSnapshotData(out TSnapshotData snapshotData);
    void SetFromSnapshotData(in TSnapshotData snapshotData);
}