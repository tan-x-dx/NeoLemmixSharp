using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelIo.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.LevelIo.Data.Style.Gadget;

public sealed class HitBoxRegionData
{
    public required Orientation Orientation { get; init; }
    public required HitBoxType HitBoxType { get; init; }
    public required Point[] HitBoxDefinitionData { get; init; }
}
