using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using NeoLemmixSharp.Common.Rendering.Text;

namespace NeoLemmixSharp.Menu.Widgets;

public sealed class MenuFontButton : Button
{
    private readonly string _text;

    private readonly Color _normalColor;

    private Color _currentColor;

    public Color NormalColor
    {
        get => _normalColor;
        init
        {
            _normalColor = value;
            _currentColor = value;
        }
    }
    public Color MouseOverColor { get; init; }
    public Color ClickColor { get; init; }

    public override IBrush GetCurrentBackground() => MenuScreen.Current.MenuSpriteBank.TransparentBrush;

    public MenuFontButton(string text)
    {
        _text = text;

        Width = text.Length * MenuFont.GlyphWidth * MenuConstants.ScaleFactor;
        Height = MenuFont.GlyphHeight * MenuConstants.ScaleFactor;
    }

    public override void OnMouseEntered()
    {
        base.OnMouseEntered();

        _currentColor = MouseOverColor;
    }

    public override void OnMouseLeft()
    {
        base.OnMouseLeft();

        _currentColor = _normalColor;
    }

    public override bool IsPressed
    {
        get => base.IsPressed;
        set
        {
            _currentColor = value
                ? ClickColor
                : IsMouseInside
                    ? MouseOverColor
                    : _normalColor;
            base.IsPressed = value;
        }
    }

    public override void InternalRender(RenderContext context)
    {
        FontBank.Instance.MenuFont.RenderContext(
            context,
            _text,
            Top,
            Left,
            MenuConstants.ScaleFactor,
            _currentColor);
    }
}