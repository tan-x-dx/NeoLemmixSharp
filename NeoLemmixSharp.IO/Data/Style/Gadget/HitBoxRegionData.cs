using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public sealed class HitBoxRegionData
{
    public required Orientation Orientation { get; init; }
    public required HitBoxType HitBoxType { get; init; }
    public required Point[] HitBoxDefinitionData { get; init; }
}
