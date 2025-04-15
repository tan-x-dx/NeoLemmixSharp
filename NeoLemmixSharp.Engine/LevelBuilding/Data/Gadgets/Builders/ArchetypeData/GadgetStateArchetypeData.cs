using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;

public sealed class GadgetStateArchetypeData
{
    public required Point HitBoxOffset { get; init; }
    public required HitBoxData[] HitBoxData { get; init; }
    public required HitBoxRegionData[] RegionData { get; init; }
}
