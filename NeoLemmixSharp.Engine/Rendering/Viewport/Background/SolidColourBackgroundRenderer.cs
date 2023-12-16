using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Rendering.Ui;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Background;

public sealed class SolidColorBackgroundRenderer : IBackgroundRenderer
{
    private readonly Texture2D _pixelTexture;
    private readonly Level.Viewport _viewport;
    private readonly Color _backgroundColor;

    public SolidColorBackgroundRenderer(ControlPanelSpriteBank spriteBank, Level.Viewport viewport, Color backgroundColor)
    {
        _pixelTexture = spriteBank.GetTexture(ControlPanelTexture.WhitePixel);
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
            RenderingLayers.BackgroundLayer);
    }
}