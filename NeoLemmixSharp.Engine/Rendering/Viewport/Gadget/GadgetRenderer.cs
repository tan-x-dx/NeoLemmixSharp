using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Gadget;

public class GadgetRenderer : IViewportObjectRenderer
{


    public void Dispose()
    {
    }

    public Rectangle GetLocationRectangle()
    {
        throw new NotImplementedException();
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, int x, int y, int scaleMultiplier)
    {
        throw new NotImplementedException();
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int x, int y, int scaleMultiplier)
    {
        throw new NotImplementedException();
    }
}