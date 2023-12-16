using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
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
    public required Color MouseOverColor { get; init; }
    public required Color ClickColor { get; init; }

    // public override IBrush GetCurrentBackground() => MenuScreen.Current.MenuSpriteBank.TransparentBrush;

    public MenuFontButton(string text)
    {
        _text = text;

        Size = new Vector2(
            text.Length * MenuFont.GlyphWidth * MenuConstants.ScaleFactor,
            MenuFont.GlyphHeight * MenuConstants.ScaleFactor);
    }
    /*
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
    }*/
}