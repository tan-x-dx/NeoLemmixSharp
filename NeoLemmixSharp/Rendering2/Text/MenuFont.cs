using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Rendering2.Level;
using System.Collections.Generic;
using System.Linq;

namespace NeoLemmixSharp.Rendering2.Text;

public sealed class MenuFont : INeoLemmixFont
{
    private const int GlyphWidth = 16;
    private const int GlyphHeight = 19;

    private readonly Texture2D _texture;

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
        IEnumerable<char> charactersToRender,
        int x,
        int y,
        int scaleMultiplier)
    {
        var dest = new Rectangle(x, y, GlyphWidth * scaleMultiplier, GlyphHeight * scaleMultiplier);
        foreach (var c in charactersToRender.Where(k => k > 31 && k < 127))
        {
            var source = new Rectangle(GlyphWidth * (c - 33), 0, GlyphWidth, GlyphHeight);
            spriteBatch.Draw(
                _texture,
                dest,
                source,
                Color.White,
                0.0f,
                new Vector2(),
                SpriteEffects.None,
                RenderingLayers.ControlPanelSkillCountLayer);
            dest.X += GlyphWidth * scaleMultiplier;
        }
    }
}