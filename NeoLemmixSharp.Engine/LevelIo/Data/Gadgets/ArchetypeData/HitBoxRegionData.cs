using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.LevelIo.Data.Gadgets.ArchetypeData;

public sealed class HitBoxRegionData
{
    public required Orientation Orientation { get; init; }
    public required HitBoxType HitBoxType { get; init; }
    public required Point[] HitBoxDefinitionData { get; init; }
}
