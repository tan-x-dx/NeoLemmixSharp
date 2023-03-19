using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using System;

namespace NeoLemmixSharp.Rendering;

public abstract class NeoLemmixSprite : IRenderable, IDisposable
{
    public abstract Texture2D GetTexture();
    public abstract Rectangle GetBoundingBox();
    public abstract LevelPosition GetAnchorPoint();

    public abstract bool ShouldRender { get; }
    public abstract void Render(SpriteBatch spriteBatch);

    protected bool IsOnScreen()
    {
        //   var viewPort = LevelScreen.CurrentLevel!.Viewport;

        return true;
    }

    public virtual void Dispose()
    {
    }
}