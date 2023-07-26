using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering.Text;

public sealed class MenuFont : INeoLemmixFont
{
    private const int GlyphWidth = 16;
    private const int GlyphHeight = 19;

    private readonly Texture2D _texture;

    public float RenderLayer { get; set; } = 1.0f;

    public MenuFont(ContentManager content)
    {
        _texture = content.Load<Texture2D>("fonts/menu_font");
    }

    public void Dispose()
    {
        _texture.Dispose();
    }

    public void RenderText(
        SpriteBatch spriteBatch,
        ReadOnlySpan<char> charactersToRender,
        int x,
        int y,
        int scaleMultiplier,
        Color color)
    {
        var dest = new Rectangle(x, y, GlyphWidth * scaleMultiplier, GlyphHeight * scaleMultiplier);
        foreach (var c in charactersToRender)
        {
            if (c <= 31 || c >= 127)
                continue;

            var source = new Rectangle(GlyphWidth * (c - 33), 0, GlyphWidth, GlyphHeight);
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
        int scaleMultiplier,
        Color color)
    {
        var dest = new Rectangle(x, y, GlyphWidth * scaleMultiplier, GlyphHeight * scaleMultiplier);
        foreach (var c in charactersToRender)
        {
            if (c <= 31 || c >= 127)
                continue;

            var source = new Rectangle(GlyphWidth * (c - 33), 0, GlyphWidth, GlyphHeight);
            spriteBatch.Draw(
                _texture,
                dest,
                source,
                RenderLayer);
            dest.X += GlyphWidth * scaleMultiplier;
        }
    }
}