using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Rendering.Viewport;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class TerrainRenderer : IViewportObjectRenderer
{
    private readonly RenderTarget2D _terrainTexture;

    public TerrainRenderer(RenderTarget2D terrainTexture)
    {
        _terrainTexture = terrainTexture;
    }

    public void Dispose()
    {
        _terrainTexture.Dispose();

        GC.SuppressFinalize(this);
    }

    public int RendererId { get; set; }
    public int ItemId => 0;
    public Rectangle GetSpriteBounds() => new(0, 0, _terrainTexture.Width, _terrainTexture.Height);

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int projectionX, int projectionY)
    {
        var destinationRectangle = new Rectangle(
            projectionX,
            projectionY,
            sourceRectangle.Width,
            sourceRectangle.Height);

        spriteBatch.Draw(
            _terrainTexture,
            destinationRectangle,
            sourceRectangle);
    }

    public RectangularRegion CurrentBounds => new(_terrainTexture);
}