using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Rendering.Viewport;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class TerrainRenderer : IViewportObjectRenderer
{
    private readonly RenderTarget2D _terrainTexture;

    public TerrainRenderer(RenderTarget2D terrainTexture)
    {
        _terrainTexture = terrainTexture;
    }
    /*
    public void RenderTerrain(SpriteBatch spriteBatch)
    {
        var maxX = _viewport.HorizontalBoundaryBehaviour.NumberOfHorizontalRenderIntervals;
        var maxY = _viewport.VerticalBoundaryBehaviour.NumberOfVerticalRenderIntervals;

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
    }*/

    public void Dispose()
    {
        _terrainTexture.Dispose();
    }

    public int RendererId { get; set; }
    public int ItemId => 0;
    public Rectangle GetSpriteBounds() => new(0, 0, _terrainTexture.Width - 1, _terrainTexture.Height - 1);

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int screenX, int screenY)
    {
        var destinationRectangle = new Rectangle(
            screenX,
            screenY,
            sourceRectangle.Width,
            sourceRectangle.Height);

        spriteBatch.Draw(
            _terrainTexture,
            destinationRectangle,
            sourceRectangle);
    }

    public LevelPosition TopLeftPixel => new(0, 0);
    public LevelPosition BottomRightPixel => new(_terrainTexture.Width - 1, _terrainTexture.Height - 1);
    public LevelPosition PreviousTopLeftPixel => new(0, 0);
    public LevelPosition PreviousBottomRightPixel => new(_terrainTexture.Width - 1, _terrainTexture.Height - 1);
}