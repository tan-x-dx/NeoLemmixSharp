using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public readonly ref struct TerrainBuilder
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
        foreach (var (_, terrainArchetypeData) in _levelData.TerrainArchetypeData)
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

        DrawTerrainPieces(_levelData.AllTerrainData, textureData);
        _terrainTexture.SetData(_terrainColors);
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

    private void ProcessTerrainGroup(TerrainGroupData terrainGroupData)
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;

        foreach (var terrainData in terrainGroupData.AllBasicTerrainData)
        {
            minX = Math.Min(minX, terrainData.X);
            minY = Math.Min(minY, terrainData.Y);
        }
        /*
        foreach (var terrainData in terrainGroupData.AllBasicTerrainData)
        {
            terrainData.X -= minX;
            terrainData.Y -= minY;
        }*/

        var maxX = int.MinValue;
        var maxY = int.MinValue;

        foreach (var terrainData in terrainGroupData.AllBasicTerrainData)
        {
            var terrainArchetypeData = _levelData.TerrainArchetypeData[terrainData.GetStylePiecePair()];

            var w = terrainData.Width ?? terrainArchetypeData.TerrainPixelColorData.Width;
            var h = terrainData.Height ?? terrainArchetypeData.TerrainPixelColorData.Height;

            maxX = Math.Max(maxX, terrainData.X + w);
            maxY = Math.Max(maxY, terrainData.Y + h);
        }

        var colors = new Color[maxX * maxY];
        var terrainPixelColorData = new PixelColorData(maxX, maxY, colors);

        DrawTerrainPieces(
            terrainGroupData.AllBasicTerrainData,
            terrainPixelColorData);

        terrainGroupData.TerrainPixelColorData = terrainPixelColorData.Trim();
    }

    private void DrawTerrainPieces(
        List<TerrainData> terrainDataList,
        PixelColorData targetData)
    {
        foreach (var terrainData in terrainDataList)
        {
            if (terrainData.GroupName is null)
            {
                var terrainArchetypeData = _levelData.TerrainArchetypeData[terrainData.GetStylePiecePair()];

                if (terrainArchetypeData.ResizeType == ResizeType.None)
                {
                    DrawTerrainPiece(
                        terrainData,
                        terrainArchetypeData,
                        targetData);
                }
                else
                {
                    DrawResizeableTerrainPiece(
                        terrainData,
                        terrainArchetypeData,
                        targetData);
                }
            }
            else
            {
                var textureGroup = _levelData.AllTerrainGroups.Find(tg => string.Equals(tg.GroupName, terrainData.GroupName, StringComparison.OrdinalIgnoreCase))!;
                DrawTerrainPiece(
                    terrainData,
                    textureGroup,
                    targetData);
            }
        }
    }

    private void DrawTerrainPiece(
        TerrainData terrainData,
        ITerrainArchetypeData terrainArchetypeData,
        PixelColorData targetPixelColorData)
    {
        var sourcePixelColorData = terrainArchetypeData.TerrainPixelColorData;

        var dihedralTransformation = new DihedralTransformation(
            terrainData.Orientation,
            terrainData.FacingDirection);

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

                x0 += terrainData.X;
                y0 += terrainData.Y;

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

    private void DrawResizeableTerrainPiece(
        TerrainData terrainData,
        TerrainArchetypeData terrainArchetypeData,
        PixelColorData targetPixelColorData)
    {

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
        return alpha >= EngineConstants.MinimumSubstantialAlphaValue;
    }

    private void LoadPixelColorData(TerrainArchetypeData terrainArchetypeData)
    {
        var rootFilePath = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            terrainArchetypeData.Style,
            DefaultFileExtensions.TerrainFolderName,
            terrainArchetypeData.TerrainPiece!);

        var pngPath = Path.ChangeExtension(rootFilePath, "png");

        using var mainTexture = Texture2D.FromFile(_graphicsDevice, pngPath);
        var pixelColorData = PixelColorData.GetPixelColorDataFromTexture(mainTexture);
        terrainArchetypeData.TerrainPixelColorData = pixelColorData;
    }
}