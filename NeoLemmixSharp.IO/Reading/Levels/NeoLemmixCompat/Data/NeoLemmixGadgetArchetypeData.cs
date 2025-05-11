using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers.GadgetReaders;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Data;

internal sealed class NeoLemmixGadgetArchetypeData
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

    public Point[] ToRectangularTriggerData() =>
    [
        new Point(TriggerX, TriggerY),
        new Point(TriggerX + TriggerWidth - 1, TriggerY + TriggerHeight - 1)
    ];
}