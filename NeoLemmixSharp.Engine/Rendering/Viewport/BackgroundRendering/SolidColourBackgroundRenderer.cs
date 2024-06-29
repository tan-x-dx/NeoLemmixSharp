using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;

public sealed class SolidColorBackgroundRenderer : IBackgroundRenderer
{
    private readonly Texture2D _pixelTexture;
    private readonly Level.Viewport _viewport;
    private readonly Color _backgroundColor;

    public SolidColorBackgroundRenderer(Level.Viewport viewport, Color backgroundColor)
    {
        _pixelTexture = CommonSprites.WhitePixelGradientSprite;
        _backgroundColor = backgroundColor;
        _viewport = viewport;
    }

    public void RenderBackground(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _pixelTexture,
            new Rectangle(
                0,
                0,
                _viewport.HorizontalBoundaryBehaviour.ViewPortLength,
                _viewport.VerticalBoundaryBehaviour.ViewPortLength),
            CommonSprites.RectangleForWhitePixelAlpha(0xff),
            _backgroundColor);
    }

    public void Dispose()
    {
    }
}