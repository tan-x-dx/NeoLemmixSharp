using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;

namespace NeoLemmixSharp.Rendering.LevelRendering.BackgroundRendering;

public sealed class SolidColourBackgroundRenderer : IBackgroundRenderer
{
    private readonly Texture2D _pixelTexture;
    private readonly LevelViewport _viewport;
    private readonly Color _backgroundColor;

    public SolidColourBackgroundRenderer(SpriteBank spriteBank, LevelViewport viewport, Color backgroundColor)
    {
        _pixelTexture = spriteBank.WhitePixelTexture;
        _backgroundColor = backgroundColor;
        _viewport = viewport;
    }

    public void RenderBackground(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _pixelTexture,
            new Rectangle(
                _viewport.ScreenX,
                _viewport.ScreenY,
                _viewport.ScreenWidth,
                _viewport.ScreenHeight),
            _backgroundColor);
    }
}