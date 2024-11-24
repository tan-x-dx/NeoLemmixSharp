using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components.Buttons;

namespace NeoLemmixSharp.Ui.Components;

public sealed class TextLabel : Button
{
    public TextLabel(int x, int y, string message, ColorPacket colors)
        : base(x, y, 0, 0, message)
    {
        SetSize(MenuFont.GlyphWidth * message.Length, MenuFont.GlyphHeight);

        Colors = colors;
    }

    public override bool ContainsPoint(LevelPosition pos) => false;

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        var colors = Colors.AsSpan();
        var color = colors[(int)State];

        FontBank.MenuFont.RenderText(spriteBatch, Label, Left, Top, 1, color);
    }
}
