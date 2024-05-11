using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class TerrainRenderer : IDisposable
{
    private readonly RenderTarget2D _terrainTexture;

    private readonly Level.Viewport _viewport;

    public TerrainRenderer(
        RenderTarget2D terrainTexture,
        Level.Viewport viewport)
    {
        _terrainTexture = terrainTexture;

        _viewport = viewport;
    }

    public void RenderTerrain(SpriteBatch spriteBatch)
    {
        var maxX = _viewport.NumberOfHorizontalRenderIntervals;
        var maxY = _viewport.NumberOfVerticalRenderIntervals;

        for (var i = 0; i < maxX; i++)
        {
            var hInterval = _viewport.GetHorizontalRenderInterval(i);
            for (var j = 0; j < maxY; j++)
            {
                var vInterval = _viewport.GetVerticalRenderInterval(j);
                var destinationRectangle = new Rectangle(hInterval.ScreenStart, vInterval.ScreenStart, hInterval.PixelLength, vInterval.PixelLength);
                var sourceRectangle = new Rectangle(hInterval.PixelStart, vInterval.PixelStart, hInterval.PixelLength, vInterval.PixelLength);

                spriteBatch.Draw(
                    _terrainTexture,
                    destinationRectangle,
                    sourceRectangle,
                    RenderingLayers.TerrainLayer);
            }
        }
    }

    public void Dispose()
    {
        _terrainTexture.Dispose();
    }
}