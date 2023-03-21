using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;

namespace NeoLemmixSharp.Rendering.Terrain;

public sealed class TerrainSprite : NeoLemmixSprite
{
    private readonly int _width;
    private readonly int _height;
    private readonly Texture2D _texture;
    private readonly Rectangle _bounds;

    public TerrainSprite(
        int width,
        int height,
        Texture2D texture)
    {
        _width = width;
        _height = height;
        _texture = texture;

        _bounds = new Rectangle(0, 0, width, height);
    }

    public override Texture2D GetTexture() => _texture;
    public override Rectangle GetBoundingBox() => _bounds;
    public override LevelPosition GetAnchorPoint() => new(0, 0);

    public override bool ShouldRender => true;
    public override void Render(SpriteBatch spriteBatch)
    {
        var viewport = LevelScreen.CurrentLevel!.Viewport;

        spriteBatch.Draw(
            _texture,
            viewport.DestinationBounds,
            viewport.SourceBounds,
            Color.White);
    }
}