using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering.Text;

public sealed class CountDownFont : INeoLemmixFont
{
    private const int EmptyGlyphWidth = 2;
    private const int GlyphWidth = 4;
    private const int GlyphHeight = 5;

    private readonly Texture2D _texture;

    public float RenderLayer { get; set; } = 0.9f;

    public CountDownFont(ContentManager content)
    {
        _texture = content.Load<Texture2D>("fonts/countdown");
    }

    private static bool GetCharRenderDetails(int c, out int sourceX, out int glyphWidth)
    {
        if (c is >= '0' and <= '9')
        {
            sourceX = (c - '0') * GlyphWidth;
            glyphWidth = GlyphWidth;
            return true;
        }

        sourceX = -EmptyGlyphWidth;
        glyphWidth = EmptyGlyphWidth;
        return false;
    }

    public void RenderText(
        SpriteBatch spriteBatch,
        ReadOnlySpan<char> charactersToRender,
        int x,
        int y,
        int scaleMultiplier,
        Color color)
    {
        var dest = new Rectangle(x, y, 0, GlyphHeight * scaleMultiplier);
        foreach (var c in charactersToRender)
        {
            var glyphWidth = RenderChar(spriteBatch, dest, c, scaleMultiplier, color);

            dest.X += glyphWidth * scaleMultiplier;
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
        var dest = new Rectangle(x, y, 0, GlyphHeight * scaleMultiplier);
        foreach (var c in charactersToRender)
        {
            var glyphWidth = RenderChar(spriteBatch, dest, c, scaleMultiplier, color);

            dest.X += glyphWidth * scaleMultiplier;
        }
    }

    private int RenderChar(
        SpriteBatch spriteBatch,
        Rectangle dest,
        int c,
        int scaleMultiplier,
        Color color)
    {
        if (!GetCharRenderDetails(c, out var sourceX, out var glyphWidth))
            return glyphWidth;

        dest.Width = glyphWidth * scaleMultiplier;

        var source = new Rectangle(sourceX, 0, glyphWidth, GlyphHeight);
        spriteBatch.Draw(
            _texture,
            dest,
            source,
            color,
            RenderLayer);

        return glyphWidth;
    }
}