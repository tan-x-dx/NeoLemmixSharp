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
            new Rectangle(viewport.TargetX, viewport.TargetY, viewport.TargetWidth, viewport.TargetHeight),
            new Rectangle(viewport.SourceX, viewport.SourceY, viewport.SourceWidth, viewport.SourceHeight),
            Color.White);
    }

    public void Dispose()
    {
        _texture.Dispose();
    }
}