using NeoLemmixSharp.Rendering2.Level.ViewportSprites;

namespace NeoLemmixSharp.LevelBuilding.Data.SpriteSet;

public sealed class LemmingSpriteData : ISpriteData
{
    public string AnimationIdentifier { get; }

    public int NumberOfFrames { get; set; }
    public int? LoopToFrame { get; set; }
    public int? PeakFrame { get; set; }

    public int LeftFootX { get; set; }
    public int LeftFootY { get; set; }

    public int RightFootX { get; set; }
    public int RightFootY { get; set; }

    public LemmingSpriteData(string animationIdentifier)
    {
        AnimationIdentifier = animationIdentifier;
    }
}