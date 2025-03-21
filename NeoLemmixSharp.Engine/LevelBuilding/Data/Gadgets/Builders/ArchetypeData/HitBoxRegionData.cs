﻿using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;

public sealed class HitBoxRegionData
{
    public required Orientation Orientation { get; init; }
    public required HitBoxType HitBoxType { get; init; }
    public required LevelPosition[] HitBoxDefinitionData { get; init; }
}
