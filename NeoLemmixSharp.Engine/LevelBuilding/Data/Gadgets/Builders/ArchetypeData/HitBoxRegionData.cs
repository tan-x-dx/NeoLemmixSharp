using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;

public sealed class HitBoxRegionData
{
    public required HitBoxType HitBoxType { get; init; }
    public required LevelPosition[] HitBoxData { get; init; }
}
