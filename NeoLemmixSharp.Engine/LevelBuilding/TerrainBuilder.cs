using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using System.Runtime.InteropServices;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public readonly ref struct TerrainBuilder
{
    private readonly LevelData _levelData;

    private readonly RenderTarget2D _terrainTexture;
    private readonly ArrayWrapper2D<Color> _terrainColors;
    private readonly ArrayWrapper2D<PixelType> _terrainPixels;

    private readonly Dictionary<StylePiecePair, TerrainArchetypeData> _terrainArchetypeDataLookup;
    private readonly Dictionary<StylePiecePair, ArrayWrapper2D<Color>> _colorDataLookup = new(EngineConstants.AssumedNumberOfTerrainArchetypeDataInLevel);

    public TerrainBuilder(GraphicsDevice graphicsDevice, LevelData levelData)
    {
        _levelData = levelData;

        var terrainDimensions = levelData.LevelDimensions;

        _terrainTexture = new RenderTarget2D(
            graphicsDevice,
            terrainDimensions.W,
            terrainDimensions.H,
            false,
            graphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24,
            8,
            RenderTargetUsage.DiscardContents);

        _terrainTexture.Name = levelData.LevelTitle;

        TextureCache.CacheShortLivedTexture(_terrainTexture);

        var rawPixels = new PixelType[terrainDimensions.Area()];
        _terrainPixels = new ArrayWrapper2D<PixelType>(rawPixels, terrainDimensions);
        var rawColors = new Color[terrainDimensions.Area()];
        _terrainColors = new ArrayWrapper2D<Color>(rawColors, terrainDimensions);

        _terrainArchetypeDataLookup = StyleCache.GetAllTerrainArchetypeData(levelData);
    }

    public void BuildTerrain()
    {
        LoadAllColorData();

        foreach (var terrainGroup in _levelData.AllTerrainGroups)
        {
            ProcessTerrainGroup(terrainGroup);
        }

        var textureData = new ArrayWrapper2D<Color>(_terrainColors.Array, _levelData.LevelDimensions);

        DrawTerrainPieces(_levelData.AllTerrainData, textureData);
        _terrainTexture.SetData(_terrainColors.Array);
    }

    public ArrayWrapper2D<Color> GetTerrainColors()
    {
        return _terrainColors;
    }

    public ArrayWrapper2D<PixelType> GetPixelData()
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
            var colorData = _colorDataLookup[terrainData.GetStylePiecePair()];

            var w = terrainData.Width ?? colorData.Size.W;
            var h = terrainData.Height ?? colorData.Size.H;

            maxX = Math.Max(maxX, terrainData.Position.X + w);
            maxY = Math.Max(maxY, terrainData.Position.Y + h);
        }

        var size = new Size(maxX, maxY);
        var colors = new Color[size.Area()];
        var terrainPixelColorData = new ArrayWrapper2D<Color>(colors, size);

        DrawTerrainPieces(
            terrainGroupData.AllBasicTerrainData,
            terrainPixelColorData);

        terrainGroupData.TerrainPixelColorData = terrainPixelColorData.Trim();
    }

    private void DrawTerrainPieces(
        List<TerrainData> terrainDataList,
        ArrayWrapper2D<Color> targetData)
    {
        foreach (var terrainData in terrainDataList)
        {
            if (terrainData.GroupName is null)
            {
                var terrainArchetypeData = _terrainArchetypeDataLookup[terrainData.GetStylePiecePair()];

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
        ArrayWrapper2D<Color> targetPixelColorData)
    {
        var sourcePixelColorData = _colorDataLookup[terrainData.GetStylePiecePair()];

        var sourceSize = sourcePixelColorData.Size;
        var targetSize = targetPixelColorData.Size;

        var transformationData = new DihedralTransformation.TransformationData(
            terrainData.Orientation,
            terrainData.FacingDirection,
            sourceSize);

        for (var y = 0; y < sourceSize.H; y++)
        {
            for (var x = 0; x < sourceSize.W; x++)
            {
                var p = new Point(x, y);
                var sourcePixelColor = sourcePixelColorData[p];
                if (sourcePixelColor == Color.Transparent)
                    continue;

                var p0 = transformationData.Transform(p);

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
        ArrayWrapper2D<Color> targetPixelColorData,
        Color sourcePixelColor,
        Point p0)
    {
        if (terrainData.Tint.HasValue)
        {
            sourcePixelColor = BlendColors(terrainData.Tint.Value, sourcePixelColor);
        }

        ref var targetPixelColor = ref targetPixelColorData[p0];
        ref var targetPixelData = ref _terrainPixels[p0];

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
        ArrayWrapper2D<Color> targetPixelColorData)
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

    private void LoadAllColorData()
    {
        foreach (var terrainData in _levelData.AllTerrainData)
        {
            LoadColorData(terrainData);
        }

        foreach (var terrainGroup in _levelData.AllTerrainGroups)
        {
            foreach (var terrainData in terrainGroup.AllBasicTerrainData)
            {
                LoadColorData(terrainData);
            }
        }
    }

    private void LoadColorData(TerrainData terrainData)
    {
        var stylePiecePair = terrainData.GetStylePiecePair();

        ref var colorData = ref CollectionsMarshal.GetValueRefOrAddDefault(_colorDataLookup, stylePiecePair, out var exists);
        if (exists)
            return;

        var rootFilePath = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            stylePiecePair.StyleName.ToString(),
            DefaultFileExtensions.TerrainFolderName,
            stylePiecePair.PieceName.ToString());

        var pngPath = Path.ChangeExtension(rootFilePath, "png");

        var mainTexture = TextureCache.GetOrLoadTexture(
            pngPath,
            terrainData.StyleName,
            terrainData.PieceName,
            TextureType.TerrainSprite);
        colorData = ArrayWrapperHelpers.GetPixelColorDataFromTexture(mainTexture);
    }
}