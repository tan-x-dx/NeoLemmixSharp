namespace NeoLemmixSharp.LevelBuilding.Data.SpriteSet;

public sealed class LemmingSpriteData
{
    public string AnimationIdentifier { get; }

    public int NumberOfFrames { get; set; }
    public int? LoopToFrame { get; set; }
    public int? PeakFrame { get; set; }

    public int LeftFootX { get; set; }
    public int LeftFootY { get; set; }

    public int RightFootX { get; set; }
    public int RightFootY { get; set; }

    /*  public int DownLeftFootX { get; set; }
      public int DownLeftFootY { get; set; }
      public int DownRightFootX { get; set; }
      public int DownRightFootY { get; set; }

      public int LeftLeftFootX { get; set; }
      public int LeftLeftFootY { get; set; }
      public int LeftRightFootX { get; set; }
      public int LeftRightFootY { get; set; }

      public int UpLeftFootX { get; set; }
      public int UpLeftFootY { get; set; }
      public int UpRightFootX { get; set; }
      public int UpRightFootY { get; set; }

      public int RightLeftFootX { get; set; }
      public int RightLeftFootY { get; set; }
      public int RightRightFootX { get; set; }
      public int RightRightFootY { get; set; }*/

    public LemmingSpriteData(string animationIdentifier)
    {
        AnimationIdentifier = animationIdentifier;
    }
}