using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Ui.Components;

public sealed class TextLabel : RectangularComponent
{
    public TextLabel(int x, int y, int w, string message)
        : base(x, y, w, UiConstants.StandardButtonHeight, message) { }

    public override bool ContainsPoint(LevelPosition pos) => false;

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
    }
}
