using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering.Text;

public sealed class CountDownFont
{
    private const int EmptyGlyphWidth = 2;
    private const int GlyphWidth = 4;
    private const int GlyphHeight = 5;

    private readonly Texture2D _texture;

    public CountDownFont(ContentManager content)
    {
        _texture = content.Load<Texture2D>("Fonts/countdown");
    }

    public void RenderTextSpan(
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

    private int RenderChar(
        SpriteBatch spriteBatch,
        Rectangle dest,
        uint c,
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
            color);

        return glyphWidth;
    }

    private static bool GetCharRenderDetails(uint c, out int sourceX, out int glyphWidth)
    {
        c -= '0';
        if (c <= '9' - '0')
        {
            c *= GlyphWidth;
            sourceX = (int)c;
            glyphWidth = GlyphWidth;
            return true;
        }

        sourceX = -EmptyGlyphWidth;
        glyphWidth = EmptyGlyphWidth;
        return false;
    }
}
