using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Ui.Components;

public sealed class Tab : Component
{
    public Tab(int x, int y, int width, int height)
        : base(x, y, width, height) { }

    protected override void RenderComponent(SpriteBatch spriteBatch) => UiSprites.DrawBeveledRectangle(spriteBatch, this);
}
