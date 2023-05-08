using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering.LevelRendering.ControlPanelRendering;

public abstract class ControlPanelButtonRenderer
{
    private const int ControlPanelButtonPixelWidth = 16;
    private const int ControlPanelButtonPixelHeight = 23;

    protected static Rectangle GetPanelButtonBackgroundSourceRectangle(int frame)
    {
        return new Rectangle(frame * ControlPanelButtonPixelWidth, 0, ControlPanelButtonPixelWidth, ControlPanelButtonPixelHeight);
    }

    public abstract void Render(SpriteBatch spriteBatch);
}