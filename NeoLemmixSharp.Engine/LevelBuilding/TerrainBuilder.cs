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
            new LevelSize(_levelData.LevelWidth, _levelData.LevelHeight),
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
            minX = Math.Min(minX, terrainData.Position.X);
            minY = Math.Min(minY, terrainData.Position.Y);
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

            var w = terrainData.Width ?? terrainArchetypeData.TerrainPixelColorData.Size.W;
            var h = terrainData.Height ?? terrainArchetypeData.TerrainPixelColorData.Size.H;

            maxX = Math.Max(maxX, terrainData.Position.X + w);
            maxY = Math.Max(maxY, terrainData.Position.Y + h);
        }

        var size = new LevelSize(maxX, maxY);
        var colors = new Color[size.Area()];
        var terrainPixelColorData = new PixelColorData(size, colors);

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

        var sourceSize = sourcePixelColorData.Size;
        var targetSize = targetPixelColorData.Size;

        for (var y = 0; y < sourceSize.H; y++)
        {
            for (var x = 0; x < sourceSize.W; x++)
            {
                var p = new LevelPosition(x, y);
                var sourcePixelColor = sourcePixelColorData[p];
                if (sourcePixelColor == Color.Transparent)
                    continue;

                var p0 = dihedralTransformation.Transform(p, sourceSize);

                p0 += terrainData.Position;

                if (targetSize.EncompassesPoint(p0))
                {
                    ChangePixel(terrainData, terrainArchetypeData, targetPixelColorData, sourcePixelColor, p0);
                }
            }
        }
    }

    private void ChangePixel(
        TerrainData terrainData,
        ITerrainArchetypeData terrainArchetypeData,
        PixelColorData targetPixelColorData,
        Color sourcePixelColor,
        LevelPosition p0)
    {
        if (terrainData.Tint.HasValue)
        {
            sourcePixelColor = BlendColors(terrainData.Tint.Value, sourcePixelColor);
        }

        var targetPixelColor = targetPixelColorData[p0];

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

        targetPixelColorData[p0] = targetPixelColor;

        var pixelIndex = targetPixelColorData.Size.GetIndexOfPoint(p0);
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
        terrainArchetypeData.TerrainPixelColorData = PixelColorData.GetPixelColorDataFromTexture(mainTexture);
    }
}