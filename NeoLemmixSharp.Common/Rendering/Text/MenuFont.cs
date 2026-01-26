using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering.Text;

public sealed class MenuFont
{
    private const int LowerCharLimit = ' ';
    private const int UpperCharLimit = '~';

    public const int GlyphWidth = 16;
    public const int GlyphHeight = 19;

    private readonly Texture2D _texture;

    public MenuFont(ContentManager content)
    {
        _texture = content.Load<Texture2D>("Fonts/menu_font");
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
        var source = new Rectangle(0, 0, GlyphWidth, GlyphHeight);
        var dest = new Rectangle(x, y, dx, GlyphHeight * scaleMultiplier);

        foreach (var c in charactersToRender)
        {
            if (c < LowerCharLimit || c > UpperCharLimit)
                continue;

            source.X = (GlyphWidth * c) - (GlyphWidth * (1 + LowerCharLimit));

            spriteBatch.Draw(
                _texture,
                dest,
                source,
                color);
            dest.X += dx;
        }
    }
}
