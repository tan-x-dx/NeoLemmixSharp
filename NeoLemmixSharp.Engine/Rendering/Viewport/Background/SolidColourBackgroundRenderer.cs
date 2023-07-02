using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Engine;
using NeoLemmixSharp.Engine.Rendering.Ui;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Background;

public sealed class SolidColourBackgroundRenderer : IBackgroundRenderer
{
    private readonly Texture2D _pixelTexture;
    private readonly LevelViewport _viewport;
    private readonly Color _backgroundColor;

    public SolidColourBackgroundRenderer(ControlPanelSpriteBank spriteBank, LevelViewport viewport, Color backgroundColor)
    {
        _pixelTexture = spriteBank.GetTexture("WhitePixel");
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
            new Rectangle(0, 0, 1, 1),
            _backgroundColor,
            0.0f,
            new Vector2(),
            SpriteEffects.None,
            RenderingLayers.BackgroundLayer);
    }
}