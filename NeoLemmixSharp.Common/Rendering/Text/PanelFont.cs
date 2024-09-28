using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering.Text;

public sealed class PanelFont
{
    public const int GlyphWidth = 8;
    private const int GlyphHeight = 16;

    private readonly Texture2D _texture;

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
                adjustedChar = c + (1 - '0');
                return true;
            case '-':
                adjustedChar = 11;
                return true;
            case >= 'A' and <= 'Z':
                adjustedChar = c + (12 - 'A');
                return true;
            case >= 'a' and <= 'z':
                adjustedChar = c + (12 - 'a');
                return true;
            default:
                adjustedChar = -1;
                return false;
        }
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
                Color.White);

            source.Y += GlyphHeight;
            spriteBatch.Draw(
                _texture,
                dest,
                source,
                color);

            dest.X += dx;
        }
    }
}