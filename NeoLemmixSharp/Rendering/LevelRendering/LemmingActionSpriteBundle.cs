using System;

namespace NeoLemmixSharp.Rendering.LevelRendering;

public sealed class LemmingActionSpriteBundle : IDisposable
{
    public ActionSprite DownLeftSprite { get; set; }
    public ActionSprite DownRightSprite { get; set; }

    public ActionSprite UpLeftSprite { get; set; }
    public ActionSprite UpRightSprite { get; set; }

    public ActionSprite LeftLeftSprite { get; set; }
    public ActionSprite LeftRightSprite { get; set; }

    public ActionSprite RightLeftSprite { get; set; }
    public ActionSprite RightRightSprite { get; set; }

    public void Dispose()
    {
        DownLeftSprite.Dispose();
        DownRightSprite.Dispose();
        UpLeftSprite.Dispose();
        UpRightSprite.Dispose();
        LeftLeftSprite.Dispose();
        LeftRightSprite.Dispose();
        RightLeftSprite.Dispose();
        RightRightSprite.Dispose();

        DownLeftSprite = null;
        DownRightSprite = null;
        UpLeftSprite = null;
        UpRightSprite = null;
        LeftLeftSprite = null;
        LeftRightSprite = null;
        RightLeftSprite = null;
        RightRightSprite = null;
    }
}