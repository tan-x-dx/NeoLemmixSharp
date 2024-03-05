using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

public sealed class AnimationData
{
    public int NumberOfFrames;
    public int InitialFrame;
    public string Name;
    public int OffsetX;
    public int OffsetY;
    public bool Hide;

    public List<AnimationTriggerData> TriggerData { get; } = new();
    public Texture2D? Texture;
}