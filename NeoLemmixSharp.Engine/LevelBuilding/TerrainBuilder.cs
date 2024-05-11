using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class TerrainBuilder
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly LevelData _levelData;

    private readonly RenderTarget2D _terrainTexture;
    private readonly Color[] _terrainColors;
    private readonly PixelType[] _terrainPixels;

    public TerrainBuilder(GraphicsDevice graphicsDevice, LevelData levelData)
    {
        _graphicsDevice = graphicsDevice;
        _levelData = levelData;

        _terrainTexture = new RenderTarget2D(
            _graphicsDevice,
            _levelData.LevelWidth,
            _levelData.LevelHeight,
            false,
            _graphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24,
            8,
            RenderTargetUsage.DiscardContents);

        _terrainPixels = new PixelType[_levelData.LevelWidth * _levelData.LevelHeight];

        _terrainColors = new Color[_levelData.LevelWidth * _levelData.LevelHeight];
    }

    public void BuildTerrain()
    {
        foreach (var terrainArchetypeData in _levelData.TerrainArchetypeData)
        {
            LoadPixelColorData(terrainArchetypeData);
        }

        foreach (var terrainGroup in _levelData.AllTerrainGroups)
        {
            ProcessTerrainGroup(terrainGroup);
        }

        var textureData = new PixelColorData(
            _levelData.LevelWidth,
            _levelData.LevelHeight,
            _terrainColors);

        DrawTerrainPieces(_levelData.AllTerrainData, textureData, 0, 0);
        _terrainTexture.SetData(_terrainColors);
    }

    private static void ProcessTerrainGroup(TerrainGroup terrainGroup)
    {
        var minX = terrainGroup.TerrainDatas.Select(td => td.X).Min();
        var minY = terrainGroup.TerrainDatas.Select(td => td.Y).Min();

        foreach (var terrainData in terrainGroup.TerrainDatas)
        {
            terrainData.X -= minX;
            terrainData.Y -= minY;
        }
    }

    private void DrawTerrainPieces(
        IEnumerable<TerrainData> terrainDataList,
        PixelColorData targetData,
        int dx,
        int dy)
    {
        foreach (var terrainData in terrainDataList)
        {
            if (terrainData.GroupName is null)
            {
                DrawTerrainPiece(
                    terrainData,
                    targetData,
                    dx,
                    dy);
            }
            else
            {
                var textureGroup = _levelData.AllTerrainGroups.First(tg => tg.GroupName == terrainData.GroupName);
                DrawTerrainPieces(
                    textureGroup.TerrainDatas,
                    targetData,
                    dx + terrainData.X,
                    dy + terrainData.Y);
            }
        }
    }

    private void DrawTerrainPiece(
        TerrainData terrainData,
        PixelColorData targetPixelColorData,
        int dx,
        int dy)
    {
        var terrainArchetypeData = _levelData.TerrainArchetypeData[terrainData.TerrainArchetypeId];
        var sourcePixelColorData = terrainArchetypeData.TerrainPixelColorData;

        var dihedralTransformation = new DihedralTransformation(
            terrainData.RotNum,
            terrainData.Flip);

        for (var x = 0; x < sourcePixelColorData.Width; x++)
        {
            for (var y = 0; y < sourcePixelColorData.Height; y++)
            {
                var sourcePixelColor = sourcePixelColorData[x, y];
                if (sourcePixelColor == Color.Transparent)
                    continue;

                dihedralTransformation.Transform(
                    x,
                    y,
                    sourcePixelColorData.Width - 1,
                    sourcePixelColorData.Height - 1,
                    out var x0,
                    out var y0);

                x0 = x0 + terrainData.X + dx;
                y0 = y0 + terrainData.Y + dy;

                if (x0 < 0 || x0 >= targetPixelColorData.Width ||
                    y0 < 0 || y0 >= targetPixelColorData.Height)
                    continue;

                if (terrainData.Tint.HasValue)
                {
                    sourcePixelColor = BlendColors(terrainData.Tint.Value, sourcePixelColor);
                }

                var targetPixelColor = targetPixelColorData[x0, y0];

                if (terrainData.Erase)
                {
                    if (sourcePixelColor != Color.Transparent)
                    {
                        targetPixelColor = Color.Transparent;
                    }
                }
                else if (terrainData.NoOverwrite)
                {
                    targetPixelColor = BlendColors(targetPixelColor, sourcePixelColor);
                }
                else
                {
                    targetPixelColor = BlendColors(sourcePixelColor, targetPixelColor);
                }

                targetPixelColorData[x0, y0] = targetPixelColor;
                var pixelIndex = targetPixelColorData.Width * y0 + x0;
                ref var targetPixelData = ref _terrainPixels[pixelIndex];

                if (PixelColorIsSubstantial(targetPixelColor))
                {
                    if (terrainArchetypeData.IsSteel)
                    {
                        targetPixelData |= PixelType.Steel;
                    }
                    else
                    {
                        targetPixelData &= ~PixelType.Steel;
                    }

                    targetPixelData |= PixelType.SolidToAllOrientations;
                }
                else
                {
                    targetPixelData = PixelType.Empty;
                }
            }
        }
    }

    private static Color BlendColors(Color foregroundColor, Color backgroundColor)
    {
        var fgA = foregroundColor.A / 255f;
        var fgR = foregroundColor.R / 255f;
        var fgG = foregroundColor.G / 255f;
        var fgB = foregroundColor.B / 255f;
        var bgA = backgroundColor.A / 255f;
        var bgR = backgroundColor.R / 255f;
        var bgG = backgroundColor.G / 255f;
        var bgB = backgroundColor.B / 255f;
        var newA = 1.0f - (1.0f - fgA) * (1.0f - bgA);
        var newR = fgR * fgA / newA + bgR * bgA * (1.0f - fgA) / newA;
        var newG = fgG * fgA / newA + bgG * bgA * (1.0f - fgA) / newA;
        var newB = fgB * fgA / newA + bgB * bgA * (1.0f - fgA) / newA;

        return new Color(newR, newG, newB, newA);
    }

    private static bool PixelColorIsSubstantial(Color color)
    {
        uint alpha = color.A;
        return alpha >= LevelConstants.MinimumSubstantialAlphaValue;
    }

    private void LoadPixelColorData(TerrainArchetypeData terrainArchetypeData)
    {
        var rootFilePath = Path.Combine(
            RootDirectoryManager.RootDirectory,
            NeoLemmixFileExtensions.StyleFolderName,
            terrainArchetypeData.Style!,
            NeoLemmixFileExtensions.TerrainFolderName,
            terrainArchetypeData.TerrainPiece!);

        var pngPath = Path.ChangeExtension(rootFilePath, "png");

        using var mainTexture = Texture2D.FromFile(_graphicsDevice, pngPath);
        var pixelColorData = PixelColorData.GetPixelColorDataFromTexture(mainTexture);
        terrainArchetypeData.TerrainPixelColorData = pixelColorData;
    }

    public Color[] GetTerrainColors()
    {
        return _terrainColors;
    }

    public PixelType[] GetPixelData()
    {
        return _terrainPixels;
    }

    public RenderTarget2D GetTerrainTexture()
    {
        return _terrainTexture;
    }
}