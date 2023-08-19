using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering.Text;

public sealed class PanelFont : INeoLemmixFont
{
    public static readonly Color Green = new(0x00, 0xB0, 0x00);
    public static readonly Color Yellow = new(0xB0, 0xB0, 0x00);
    public static readonly Color Red = new(0xB0, 0x00, 0x00);
    public static readonly Color Magenta = new(0xB0, 0x00, 0xB0);

    public const int GlyphWidth = 8;
    private const int GlyphHeight = 16;

    private readonly Texture2D _texture;

    public float RenderLayer { get; set; } = 1.0f;

    public PanelFont(ContentManager content)
    {
        _texture = content.Load<Texture2D>("fonts/panel_font");
    }

    public void Dispose()
    {
        _texture.Dispose();
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

    public void RenderText(
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
                continue;

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
                continue;

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