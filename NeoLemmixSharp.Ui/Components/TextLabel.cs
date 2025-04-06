using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering.Text;

namespace NeoLemmixSharp.Ui.Components;

public sealed class TextLabel : Component
{
    public TextLabel(int x, int y, string message, ColorPacket colors)
        : base(x, y, 0, 0, message)
    {
        SetSize(MenuFont.GlyphWidth * message.Length, MenuFont.GlyphHeight);

        Colors = colors;
    }

    public override bool ContainsPoint(Point pos) => false;

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        var colors = Colors.AsSpan();
        var color = colors[(int)State];

        FontBank.MenuFont.RenderText(spriteBatch, Label, Left, Top, 1, color);
    }
}
