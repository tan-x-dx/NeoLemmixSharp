namespace NeoLemmixSharp.IO.Data.Level;

public sealed class HatchGroupData
{
    public required int HatchGroupId { get; init; }
    public required uint MaxSpawnInterval { get; init; }
    public required uint InitialSpawnInterval { get; init; }
    public required uint MinSpawnInterval { get; init; }

    internal HatchGroupData()
    {
    }
}
