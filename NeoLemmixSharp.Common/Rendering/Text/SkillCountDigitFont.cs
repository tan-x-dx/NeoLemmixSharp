using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering.Text;

public sealed class SkillCountDigitFont
{
    private const int EmptyGlyphWidth = 2;
    private const int DigitGlyphWidth = 4;
    private const int SpecialGlyphWidth = 8;
    private const int GlyphHeight = 8;

    public const char InfinityGlyph = '∞';
    private const int InfinityGlyphOffset = 40;
    public const char LockGlyph = '∅';
    private const int LockGlyphOffset = 48;

    private readonly Texture2D _texture;

    public SkillCountDigitFont(ContentManager content)
    {
        _texture = content.Load<Texture2D>("Fonts/skill_count_digits");
    }

    private static bool GetCharRenderDetails(int c, out int sourceX, out int glyphWidth)
    {
        switch (c)
        {
            case >= '0' and <= '9':
                sourceX = (c - '0') * DigitGlyphWidth;
                glyphWidth = DigitGlyphWidth;
                return true;
            case InfinityGlyph:
                sourceX = InfinityGlyphOffset;
                glyphWidth = SpecialGlyphWidth;
                return true;
            case LockGlyph:
                sourceX = LockGlyphOffset;
                glyphWidth = SpecialGlyphWidth;
                return true;
            default:
                sourceX = -EmptyGlyphWidth;
                glyphWidth = EmptyGlyphWidth;
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
            color);

        return glyphWidth;
    }
}