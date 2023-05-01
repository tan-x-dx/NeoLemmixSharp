using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering.LevelRendering.ControlPanelRendering;

public abstract class ControlPanelButtonRenderer
{
    public void Dispose()
    {

    }

    public Rectangle GetLocationRectangle()
    {
        return Rectangle.Empty;
    }

    public abstract void RenderAtPosition(SpriteBatch spriteBatch, int x, int y, int scaleMultiplier);

    public abstract void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int x, int y, int scaleMultiplier);
}