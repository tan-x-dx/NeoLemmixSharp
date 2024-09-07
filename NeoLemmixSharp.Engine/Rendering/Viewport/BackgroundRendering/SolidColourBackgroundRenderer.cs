using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Level;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;

public sealed class SolidColorBackgroundRenderer : IBackgroundRenderer
{
    private readonly Texture2D _pixelTexture;
    private readonly Color _backgroundColor;

    public SolidColorBackgroundRenderer(Color backgroundColor)
    {
        _pixelTexture = CommonSprites.WhitePixelGradientSprite;
        _backgroundColor = backgroundColor;
    }

    public void RenderBackground(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _pixelTexture,
            new Rectangle(
                0,
                0,
                LevelScreen.HorizontalBoundaryBehaviour.ViewPortLength,
                LevelScreen.VerticalBoundaryBehaviour.ViewPortLength),
            CommonSprites.RectangleForWhitePixelAlpha(0xff),
            _backgroundColor);
    }

    public void Dispose()
    {
    }
}