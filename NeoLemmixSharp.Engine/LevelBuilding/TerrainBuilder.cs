using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class TerrainBuilder
{
    private readonly TerrainPainter _terrainPainter;

    public TerrainBuilder(GraphicsDevice graphicsDevice, LevelData levelData)
    {
        var terrainDimensions = levelData.LevelDimensions;

        var terrainTexture = new RenderTarget2D(
             graphicsDevice,
             terrainDimensions.W,
             terrainDimensions.H,
             false,
             graphicsDevice.PresentationParameters.BackBufferFormat,
             DepthFormat.Depth24,
             8,
             RenderTargetUsage.DiscardContents)
        {
            Name = levelData.LevelTitle + "_Terrain"
        };

        TextureCache.CacheLevelSpecificTexture(terrainTexture);

        _terrainPainter = new TerrainPainter(levelData, terrainTexture, default);
    }

    public void BuildTerrain()
    {
        _terrainPainter.PaintTerrain();
        _terrainPainter.Apply();
    }

    public RenderTarget2D GetTerrainTexture() => _terrainPainter.TerrainTexture;
    public void GetTerrainColors(out ArrayWrapper2D<Color> terrainColorData) => terrainColorData = _terrainPainter.TerrainColors;
    public void GetPixelData(out ArrayWrapper2D<PixelType> pixelData) => pixelData = _terrainPainter.TerrainPixels;
}
