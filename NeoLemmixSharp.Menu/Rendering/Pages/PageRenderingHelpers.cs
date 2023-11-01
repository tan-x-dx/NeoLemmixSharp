using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Menu.Rendering.Pages;

public static class PageRenderingHelpers
{
    public static void RenderBackground(
        Texture2D texture,
        SpriteBatch spriteBatch,
        int windowWidth,
        int windowHeight)
    {
        var textureWidth = texture.Width;
        var textureHeight = texture.Height;

        var maxX = (int)Math.Ceiling((double)windowWidth / textureWidth);
        var maxY = (int)Math.Ceiling((double)windowHeight / textureHeight);

        for (var x = 0; x < maxX; x++)
        {
            var vx = x * textureWidth;

            for (var y = 0; y < maxY; y++)
            {
                var vy = y * textureHeight;

                spriteBatch.Draw(texture, new Vector2(vx, vy), Color.White);
            }
        }
    }
}