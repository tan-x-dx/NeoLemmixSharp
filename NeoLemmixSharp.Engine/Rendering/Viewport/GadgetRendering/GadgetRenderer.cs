using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class GadgetRenderer : IViewportObjectRenderer
{

    public Rectangle GetSpriteBounds()
    {
        throw new NotImplementedException();
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int screenX, int screenY)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
    }
}