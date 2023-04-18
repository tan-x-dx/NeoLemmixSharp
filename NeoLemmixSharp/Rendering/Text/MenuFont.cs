using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace NeoLemmixSharp.Rendering.Text;

public sealed class MenuFont : INeoLemmixFont
{
    private readonly Texture2D _texture;
    private readonly char[] _chars = Enumerable.Range(32, 95).Select(i => (char)i).ToArray();

    public MenuFont(ContentManager content)
    {
        _texture = content.Load<Texture2D>("Fonts/menu_font");
    }

    public void Dispose()
    {
        _texture.Dispose();
    }

    public void RenderText(
        SpriteBatch spriteBatch,
        IEnumerable<char> charactersToRender,
        int x,
        int y)
    {
        var dest = new Rectangle(x, y, 16, 19);
        foreach (var c in charactersToRender.Where(k => k > 31 && k < 127))
        {
            var source = new Rectangle(16 * (c - 33), 0, 16, 19);
            spriteBatch.Draw(_texture, dest, source, Color.White);
            dest.X += 16;
        }
    }
}