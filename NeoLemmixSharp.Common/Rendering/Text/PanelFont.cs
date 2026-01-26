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
        if (charactersToRender.Length == 0)
            return;

        var dx = GlyphWidth * scaleMultiplier;
        var source = new Rectangle(0, GlyphHeight, GlyphWidth, GlyphHeight);
        var dest = new Rectangle(x, y, dx, GlyphHeight * scaleMultiplier);

        foreach (var c in charactersToRender)
        {
            if (CanRenderChar(c, out var charIndex))
            {
                source.X = GlyphWidth * charIndex;
                source.Y = 0;
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
            }

            dest.X += dx;
        }
    }

    private static bool CanRenderChar(uint c, out int charIndex)
    {
        if (c is '%')
        {
            charIndex = IndexOfPercentInPng;
            return true;
        }
        if (c is '-')
        {
            charIndex = IndexOfDashInPng;
            return true;
        }

        uint c0 = c - '0';
        if (c0 <= '9' - '0')
        {
            c0 += IndexOfZeroInPng;
            charIndex = (int)c0;
            return true;
        }

        c0 = c - 'a';
        if (c0 <= 'z' - 'a')
        {
            c0 += IndexOfAInPng;
            charIndex = (int)c0;
            return true;
        }

        c0 = c - 'A';
        if (c0 <= 'Z' - 'A')
        {
            c0 += IndexOfAInPng;
            charIndex = (int)c0;
            return true;
        }

        charIndex = 0;
        return false;
    }
}
