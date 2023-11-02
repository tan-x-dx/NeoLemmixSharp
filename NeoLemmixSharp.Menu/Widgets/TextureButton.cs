using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;

namespace NeoLemmixSharp.Menu.Widgets;

public sealed class TextureButton : Button
{
    private readonly Texture2D _texture;

    public TextureButton(Texture2D texture)
    {
        _texture = texture;
    }

    public override IBrush GetCurrentBackground() => MenuScreen.Current.MenuSpriteBank.TransparentBrush;

    public override void InternalRender(RenderContext context)
    {
        context.Draw(
            _texture,
            new Vector2(Left, Top),
            Color.White,
            new Vector2(MenuConstants.ScaleFactor, MenuConstants.ScaleFactor));
    }
}