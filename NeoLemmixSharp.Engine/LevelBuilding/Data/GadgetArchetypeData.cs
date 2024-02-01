namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class GadgetArchetypeData
{
    public required int GadgetArchetypeId { get; init; }

    public required string? Style { get; init; }
    public required string? Gadget { get; init; }

    public int TriggerX { get; set; }
    public int TriggerY { get; set; }

    public int? TriggerWidth { get; set; }
    public int? TriggerHeight { get; set; }

    public int PrimaryAnimationFrameCount { get; set; }
}