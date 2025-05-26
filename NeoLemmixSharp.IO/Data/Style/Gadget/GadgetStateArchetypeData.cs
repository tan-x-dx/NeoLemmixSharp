using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public sealed class GadgetStateArchetypeData
{
    public required Point HitBoxOffset { get; init; }
    public required HitBoxData[] HitBoxData { get; init; }
    public required HitBoxRegionData[] RegionData { get; init; }
    public required SpriteArchetypeData SpriteData { get; init; }
}
