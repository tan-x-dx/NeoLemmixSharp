using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering.Terrain;

public sealed class TerrainSprite : NeoLemmixSprite
{
    private readonly Texture2D _texture;

    public TerrainSprite(
        int width,
        int height,
        Texture2D texture)
    {
        _texture = texture;

        BoundingBox = new Rectangle(0, 0, width, height);
    }

    public override Rectangle BoundingBox { get; }
    public override bool ShouldRender => true;
    public override void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _texture,
            BoundingBox,
            Color.White);
    }
}