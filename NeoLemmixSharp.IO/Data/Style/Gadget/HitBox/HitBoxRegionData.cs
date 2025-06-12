using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

public sealed class HitBoxRegionData
{
    public required Orientation Orientation { get; init; }
    public required HitBoxType HitBoxType { get; init; }
    public required Point[] HitBoxDefinitionData { get; init; }
}
