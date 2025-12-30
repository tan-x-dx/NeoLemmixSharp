using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;

public sealed class SolidColorBackgroundRenderer : IBackgroundRenderer
{
    private readonly Color _backgroundColor;

    public SolidColorBackgroundRenderer(Color backgroundColor)
    {
        _backgroundColor = backgroundColor;
    }

    public void RenderBackground(SpriteBatch spriteBatch)
    {
        spriteBatch.FillRect(
            new Rectangle(
                0,
                0,
                LevelScreen.HorizontalBoundaryBehaviour.ViewPortLength,
                LevelScreen.VerticalBoundaryBehaviour.ViewPortLength),
            _backgroundColor);
    }

    public void Dispose()
    {
    }
}
