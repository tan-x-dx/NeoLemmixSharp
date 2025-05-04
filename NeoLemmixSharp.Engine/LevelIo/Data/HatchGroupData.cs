namespace NeoLemmixSharp.Engine.LevelIo.Data;

public sealed class HatchGroupData
{
    public required int HatchGroupId { get; init; }
    public required int MaxSpawnInterval { get; init; }
    public required int InitialSpawnInterval { get; init; }
    public required int MinSpawnInterval { get; init; }
}