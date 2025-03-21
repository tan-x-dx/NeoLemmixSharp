﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

public sealed class NeoLemmixGadgetArchetypeData
{
    public required string Style { get; init; }
    public required string GadgetPiece { get; init; }

    public NeoLemmixGadgetBehaviour Behaviour { get; set; }

    public int TriggerX { get; set; }
    public int TriggerY { get; set; }

    public int TriggerWidth { get; set; } = 1;
    public int TriggerHeight { get; set; } = 1;

    public int? DefaultWidth { get; set; }
    public int? DefaultHeight { get; set; }

    public ResizeType ResizeType { get; set; }

    public int PrimaryAnimationFrameCount { get; set; }
    public int PrimaryAnimationOffsetX { get; set; }
    public int PrimaryAnimationOffsetY { get; set; }
    public bool IsSkillPickup { get; set; }

    // public List<AnimationData> AnimationData { get; } = new();

    public LevelPosition[] ToRectangularTriggerData() =>
    [
        new LevelPosition(TriggerX, TriggerY),
        new LevelPosition(TriggerX + TriggerWidth - 1, TriggerY + TriggerHeight - 1)
    ];
}