using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

[DebuggerDisplay("{StateName}")]
public sealed class GadgetStateArchetypeData
{
    public required GadgetStateName StateName { get; init; }
    public required HitBoxRegionData[] RegionData { get; init; }
    public required AnimationLayerArchetypeData[] AnimationLayerData { get; init; }

    internal GadgetStateArchetypeData()
    {
    }
}
