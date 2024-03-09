using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

public sealed class NeoLemmixGadgetArchetypeData
{
    public required int GadgetArchetypeId { get; init; }

    public required string Style { get; init; }
    public required string Gadget { get; init; }

    public NeoLemmixGadgetBehaviour Behaviour { get; set; }

    public int TriggerX { get; set; }
    public int TriggerY { get; set; }

    public int TriggerWidth { get; set; } = 1;
    public int TriggerHeight { get; set; } = 1;

    public ResizeType ResizeType { get; set; }

    public int PrimaryAnimationFrameCount { get; set; }
    public bool IsSkillPickup { get; set; }

    public List<AnimationData> AnimationData { get; } = new();
}