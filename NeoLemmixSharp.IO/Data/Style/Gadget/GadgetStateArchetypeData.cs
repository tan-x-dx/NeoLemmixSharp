using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public sealed class GadgetStateArchetypeData
{
    public required Point HitBoxOffset { get; init; }
    public required HitBoxData[] HitBoxData { get; init; }
    public required HitBoxRegionData[] RegionData { get; init; }
}
