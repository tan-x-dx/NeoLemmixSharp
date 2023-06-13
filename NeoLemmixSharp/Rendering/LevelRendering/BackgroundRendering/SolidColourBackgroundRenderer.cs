﻿using Microsoft.Xna.Framework;
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
        _pixelTexture = spriteBank.GetTexture(SpriteBankTextureNames.WhitePixel);
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