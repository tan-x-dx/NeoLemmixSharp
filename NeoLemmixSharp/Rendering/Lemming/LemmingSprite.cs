using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;

namespace NeoLemmixSharp.Rendering.Lemming;

public sealed class LemmingSprite : NeoLemmixSprite
{
    private readonly Engine.Lemming _lemming;
    private Texture2D _texture;

    public LemmingSprite(GraphicsDevice graphicsDevice, Engine.Lemming lemming)
    {
        _lemming = lemming;
        _texture = new Texture2D(graphicsDevice, 3, 3);

        var red = new Color(200, 0, 0, 255);
        var yellow = new Color(200, 200, 0, 255);

        var x = new uint[9];
        x[1] = red.PackedValue;
        x[3] = red.PackedValue;
        x[4] = yellow.PackedValue;
        x[5] = red.PackedValue;
        x[7] = red.PackedValue;
        _texture.SetData(x);
    }

    public override Rectangle BoundingBox { get; } = new(0, 0, 3, 3);
    public override bool ShouldRender => true;
    public override void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _texture,
            GetSpriteDestinationRectangle(),
            Color.White);
    }

    private Rectangle GetSpriteDestinationRectangle()
    {
        var zoom = LevelScreen.CurrentLevel!.Viewport.Zoom;

        return new Rectangle((_lemming.X - 1) * zoom.ScaleMultiplier, _lemming.Y * zoom.ScaleMultiplier, 3 * zoom.ScaleMultiplier, 3 * zoom.ScaleMultiplier);
    }
}