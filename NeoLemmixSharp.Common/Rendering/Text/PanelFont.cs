using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering.Text;

public sealed class PanelFont
{
    private const int IndexOfPercentInPng = 0;
    private const int IndexOfZeroInPng = 1;
    private const int IndexOfDashInPng = 11;
    private const int IndexOfAInPng = 12;

    public const int GlyphWidth = 8;
    private const int GlyphHeight = 16;

    private readonly Texture2D _texture;

    public PanelFont(ContentManager content)
    {
        _texture = content.Load<Texture2D>("Fonts/panel_font");
    }

    public void RenderTextSpan(
        SpriteBatch spriteBatch,
        ReadOnlySpan<char> charactersToRender,
        int x,
        int y,
        int scaleMultiplier,
        Color color)
    {
        var dx = GlyphWidth * scaleMultiplier;
        var dest = new Rectangle(x, y, dx, GlyphHeight * scaleMultiplier);
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
                Color.White);

            source.Y = GlyphHeight;
            spriteBatch.Draw(
                _texture,
                dest,
                source,
                color);

            dest.X += dx;
        }
    }

    private static bool CanRenderChar(uint c, out int adjustedChar)
    {
        if (c is '%')
        {
            adjustedChar = IndexOfPercentInPng;
            return true;
        }
        if (c is '-')
        {
            adjustedChar = IndexOfDashInPng;
            return true;
        }

        uint c0 = c - '0';
        if (c0 <= '9' - '0')
        {
            c0 += IndexOfZeroInPng;
            adjustedChar = (int)c0;
            return true;
        }

        c0 = c - 'a';
        if (c0 <= 'z' - 'a')
        {
            c0 += IndexOfAInPng;
            adjustedChar = (int)c0;
            return true;
        }

        c0 = c - 'A';
        if (c0 <= 'Z' - 'A')
        {
            c0 += IndexOfAInPng;
            adjustedChar = (int)c0;
            return true;
        }

        adjustedChar = -1;
        return false;
    }
}
