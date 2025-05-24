using NeoLemmixSharp.IO.Data.Style.Theme;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public readonly struct SpriteBankData(LemmingActionSprite[] actionSprites, TribeColorData[] tribeColorData)
{
    public readonly LemmingActionSprite[] ActionSprites = actionSprites;
    public readonly TribeColorData[] TribeColorData = tribeColorData;
}