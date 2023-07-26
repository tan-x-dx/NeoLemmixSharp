using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering.Text;

public sealed class SkillCountDigitFont : INeoLemmixFont
{
    private const int GlyphWidth = 4;
    private const int GlyphHeight = 8;

    private readonly Texture2D _texture;

    public float RenderLayer { get; set; } = 1.0f;

    public SkillCountDigitFont(ContentManager content)
    {
        _texture = content.Load<Texture2D>("fonts/skill_count_digits");
    }

    public void Dispose()
    {
        _texture.Dispose();
    }

    public void RenderText(
        SpriteBatch spriteBatch,
        IEnumerable<char> charactersToRender,
        int x,
        int y,
        int scaleMultiplier)
    {
        var dest = new Rectangle(x, y, GlyphWidth * scaleMultiplier, GlyphHeight * scaleMultiplier);
        foreach (var c in charactersToRender.Where(k => k > 47 && k < 58))
        {
            var source = new Rectangle(GlyphWidth * (c - 48), 0, GlyphWidth, GlyphHeight);
            spriteBatch.Draw(
                _texture,
                dest,
                source,
                RenderLayer);
            dest.X += GlyphWidth * scaleMultiplier;
        }
    }

    public void RenderTextSpan(
        SpriteBatch spriteBatch,
        ReadOnlySpan<int> charactersToRender,
        int x,
        int y,
        int scaleMultiplier = 1)
    {
        var dest = new Rectangle(x, y, GlyphWidth * scaleMultiplier, GlyphHeight * scaleMultiplier);
        foreach (var c in charactersToRender)
        {
            if (c <= 47 || c >= 58)
                continue;

            var source = new Rectangle(GlyphWidth * (c - 48), 0, GlyphWidth, GlyphHeight);
            spriteBatch.Draw(
                _texture,
                dest,
                source,
                RenderLayer);
            dest.X += GlyphWidth * scaleMultiplier;
        }
    }
}