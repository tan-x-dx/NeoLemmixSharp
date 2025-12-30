using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Menu.LevelEditor.Components;

public sealed class LevelCanvas : Component
{
    public LevelCanvas(int x, int y, int width, int height) : base(x, y, width, height, string.Empty)
    {
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        spriteBatch.FillRect(Helpers.CreateRectangle(Position, Dimensions), Color.Black);
    }
}
