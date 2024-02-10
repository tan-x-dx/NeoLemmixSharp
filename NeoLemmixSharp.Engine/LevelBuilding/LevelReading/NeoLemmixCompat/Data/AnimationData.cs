namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

public sealed class AnimationData
{
    public int NumberOfFrames { get; set; }
    public int InitialFrame { get; set; }
    public string Name { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public bool Hide { get; set; }

    public List<AnimationTriggerData> TriggerData { get; } = new();
}