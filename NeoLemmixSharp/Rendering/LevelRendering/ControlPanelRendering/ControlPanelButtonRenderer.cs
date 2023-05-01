using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering.LevelRendering.ControlPanelRendering;

public abstract class ControlPanelButtonRenderer : ISprite
{
    public void Dispose()
    {

    }

    public Rectangle GetLocationRectangle()
    {
        return Rectangle.Empty;
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, int x, int y, int scaleMultiplier)
    {
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int x, int y, int scaleMultiplier)
    {
    }
}