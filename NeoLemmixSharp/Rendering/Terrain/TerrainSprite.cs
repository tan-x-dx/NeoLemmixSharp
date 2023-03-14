using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;

namespace NeoLemmixSharp.Rendering.Terrain;

public sealed class TerrainSprite : NeoLemmixSprite
{
    private readonly int _width;
    private readonly int _height;
    private readonly Texture2D _texture;

    public TerrainSprite(
        int width,
        int height,
        Texture2D texture)
    {
        _width = width;
        _height = height;
        _texture = texture;

        var x = new int[width * height];
        _texture.GetData(x);

        BoundingBox = new Rectangle(0, 0, width, height);
    }

    public override Rectangle BoundingBox { get; }
    public override bool ShouldRender => true;
    public override void Render(SpriteBatch spriteBatch)
    {
        var zoom = LevelScreen.CurrentLevel!.Viewport.Zoom;
        
        spriteBatch.Draw(
            _texture,
            new Rectangle(0,0,_width * zoom.ScaleMultiplier, _height* zoom.ScaleMultiplier),
            Color.White);
    }
}