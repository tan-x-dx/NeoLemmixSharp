using NeoLemmixSharp.Common;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;

public sealed class GadgetStateArchetypeData
{
    public required Point HitBoxOffset { get; init; }
    public required HitBoxData[] HitBoxData { get; init; }
    public HitBoxRegionDataArray RegionData = new();
}

[InlineArray(EngineConstants.NumberOfOrientations)]
public struct HitBoxRegionDataArray
{
    public HitBoxRegionData _x;
}