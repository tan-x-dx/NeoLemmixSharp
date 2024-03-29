﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering.Text;

public sealed class PanelFont
{
    public const int GlyphWidth = 8;
    private const int GlyphHeight = 16;

    private readonly Texture2D _texture;

    public float RenderLayer { get; set; } = 1.0f;

    public PanelFont(ContentManager content)
    {
        _texture = content.Load<Texture2D>("fonts/panel_font");
    }

    private static bool CanRenderChar(int c, out int adjustedChar)
    {
        switch (c)
        {
            case '%':
                adjustedChar = 0;
                return true;
            case >= '0' and <= '9':
                adjustedChar = c - '0' + 1;
                return true;
            case '-':
                adjustedChar = 11;
                return true;
            case >= 'A' and <= 'Z':
                adjustedChar = c - 'A' + 12;
                return true;
            case >= 'a' and <= 'z':
                adjustedChar = c - 'a' + 12;
                return true;
            default:
                adjustedChar = -1;
                return false;
        }
    }

    public void RenderTextSpan(
        SpriteBatch spriteBatch,
        ReadOnlySpan<int> charactersToRender,
        int x,
        int y,
        int scaleMultiplier,
        Color color)
    {
        var dx = GlyphWidth * scaleMultiplier;
        var dest = new Rectangle(x, y, GlyphWidth * scaleMultiplier, GlyphHeight * scaleMultiplier);
        foreach (var c in charactersToRender)
        {
            if (!CanRenderChar(c, out var adjustedChar))
            {
                dest.X += dx;
                continue;
            }

            var source = new Rectangle(GlyphWidth * adjustedChar, 0, GlyphWidth, GlyphHeight);
            spriteBatch.Draw(
                _texture,
                dest,
                source,
                Color.White,
                RenderLayer);

            source.Y += GlyphHeight;
            spriteBatch.Draw(
                _texture,
                dest,
                source,
                color,
                RenderLayer);

            dest.X += dx;
        }
    }
}