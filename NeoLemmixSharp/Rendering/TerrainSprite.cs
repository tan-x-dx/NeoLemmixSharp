using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;

namespace NeoLemmixSharp.Rendering;

public sealed class TerrainSprite : IRenderable
{
    private readonly Texture2D _texture;

    public TerrainSprite(Texture2D texture)
    {
        _texture = texture;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        var viewport = LevelScreen.CurrentLevel!.Viewport;

        spriteBatch.Draw(
            _texture,
            new Rectangle(viewport.ScreenX, viewport.ScreenY, viewport.ScreenWidth, viewport.ScreenHeight),
            new Rectangle(viewport.ViewPortX, viewport.ViewPortY, viewport.ViewPortWidth, viewport.ViewPortHeight),
            Color.White);
    }

    public void Dispose()
    {
        _texture.Dispose();
    }
}