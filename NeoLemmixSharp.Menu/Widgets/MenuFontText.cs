using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using NeoLemmixSharp.Common.Rendering.Text;

namespace NeoLemmixSharp.Menu.Widgets;

public sealed class MenuFontText : Widget
{
    private readonly Color _color;

    public string Text { get; set; }

    public MenuFontText(string text, Color color)
    {
        Text = text;
        _color = color;

        Width = text.Length * MenuFont.GlyphWidth * MenuConstants.ScaleFactor;
        Height = MenuFont.GlyphHeight * MenuConstants.ScaleFactor;
    }

    public override IBrush GetCurrentBackground() => MenuScreen.Current.MenuSpriteBank.TransparentBrush;

    public override void InternalRender(RenderContext context)
    {
        FontBank.Instance.MenuFont.RenderContext(
            context,
            Text,
            Top,
            Left,
            MenuConstants.ScaleFactor,
            _color);
    }
}