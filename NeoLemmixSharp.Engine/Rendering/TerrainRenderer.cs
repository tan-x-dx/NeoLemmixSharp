using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level.Terrain;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class TerrainRenderer : IDisposable
{
    private readonly RenderTarget2D _terrainTexture;
    private readonly Texture2D _whitePixelGradientTexture;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly TerrainPainter _terrainPainter;
    private readonly uint[] _terrainColors;

    private readonly Level.Viewport _viewport;

    public TerrainRenderer(GraphicsDevice graphicsDevice,
        RenderTarget2D terrainTexture,
        TerrainPainter terrainPainter,
        uint[] terrainColors,
        Level.Viewport viewport)
    {
        _terrainTexture = terrainTexture;
        _whitePixelGradientTexture = CommonSprites.WhitePixelGradientSprite;
        _graphicsDevice = graphicsDevice;
        _terrainPainter = terrainPainter;
        _terrainColors = terrainColors;

        _viewport = viewport;
    }

    public void UpdateTerrainTexture(SpriteBatch spriteBatch)
    {
        var pixelChanges = _terrainPainter.GetLatestPixelChanges();
        if (pixelChanges.Length == 0)
            return;

        var destinationRectangle = new Rectangle(0, 0, 1, 1);
        var sourceRectangle = CommonSprites.RectangleForWhitePixelAlpha(0xff);

        _graphicsDevice.SetRenderTarget(_terrainTexture);
   //     _graphicsDevice.Clear(Color.Transparent);
   //     spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
      /*  foreach (var pixelChangeData in pixelChanges)
        {
            destinationRectangle.X = pixelChangeData.X;
            destinationRectangle.Y = pixelChangeData.Y;
            spriteBatch.Draw(
                _whitePixelGradientTexture,
                destinationRectangle,
                sourceRectangle,
                new Color(pixelChangeData.ToColor));
        }*/
   //     spriteBatch.End();

    //    _graphicsDevice.SetRenderTarget(null);
    //    _graphicsDevice.Clear(Color.Transparent);
     //   _terrainTexture.GetData(_terrainColors);
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