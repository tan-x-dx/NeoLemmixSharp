namespace NeoLemmixSharp.Engine.Level.Rewind;

public unsafe interface ISnapshotDataConvertible
{
    int GetRequiredNumberOfBytesForSnapshotting();
    void WriteToSnapshotData(byte* snapshotDataPointer);
    void SetFromSnapshotData(byte* snapshotDataPointer);    
}
