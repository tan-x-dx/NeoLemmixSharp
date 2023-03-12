using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;

namespace NeoLemmixSharp.Rendering;

public abstract class NeoLemmixSprite : IRenderable
{
    public abstract Rectangle BoundingBox { get; }

    public abstract bool ShouldRender { get; }
    public abstract void Render(SpriteBatch spriteBatch);

    protected bool IsOnScreen()
    {
        var viewPort = LevelScreen.CurrentLevel!.Viewport;

        return true;
    }
}