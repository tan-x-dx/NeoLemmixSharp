using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Ui.Components;

public sealed class Tab : RectangularComponent
{
    public Tab(int x, int y, int width, int height)
        : base(x, y, width, height) { }

    public Tab(int x, int y, int width, int height, string? label)
        : base(x, y, width, height, label) { }

    protected override void RenderComponent(SpriteBatch spriteBatch) => UiSprites.DrawBeveledRectangle(spriteBatch, this);
}
