using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;

public sealed class HitBoxRegionData
{
    public required Point HitBoxOffset { get; init; }
    public required Orientation Orientation { get; init; }
    public required HitBoxType HitBoxType { get; init; }
    public required Point[] HitBoxDefinitionData { get; init; }
}
