using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using System;

namespace NeoLemmixSharp.Rendering;

public sealed class TerrainSprite : IDisposable
{
    private readonly Texture2D _texture;

    private readonly uint[] _colourSetter = new uint[1];

    public TerrainSprite(Texture2D texture)
    {
        _texture = texture;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        LevelScreen.CurrentLevel.Viewport.RenderTerrain(spriteBatch, _texture);

        /* var viewport = LevelScreen.CurrentLevel.Viewport;

         spriteBatch.Draw(
             _texture,
             new Rectangle(viewport.ScreenX, viewport.ScreenY, viewport.ScreenWidth, viewport.ScreenHeight),
             new Rectangle(viewport.ViewPortX, viewport.ViewPortY, viewport.ViewPortWidth, viewport.ViewPortHeight),
             Color.White);*/
    }

    public void SetPixelColour(int x, int y, uint colour)
    {
        _colourSetter[0] = colour;
        _texture.SetData(0, new Rectangle(x, y, 1, 1), _colourSetter, 0, 1);
    }

    public void Dispose()
    {
        _texture.Dispose();
    }
}