using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.IO;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using System.Runtime.InteropServices;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public readonly struct TerrainPainter
{
    private readonly LevelData _levelData;

    public readonly RenderTarget2D TerrainTexture;
    public readonly ArrayWrapper2D<Color> TerrainColors;
    public readonly ArrayWrapper2D<PixelType> TerrainPixels;

    private readonly Dictionary<StylePiecePair, TerrainArchetypeData> _terrainArchetypeDataLookup;
    private readonly Dictionary<StylePiecePair, ArrayWrapper2D<Color>> _colorDataLookup = new(IoConstants.AssumedNumberOfTerrainArchetypeDataInLevel);

    private readonly Point _terrainOffset;

    public TerrainPainter(
        LevelData levelData,
        RenderTarget2D terrainTexture,
        Point terrainOffset)
    {
        _levelData = levelData;
        TerrainTexture = terrainTexture;

        _terrainOffset = terrainOffset;

        var terrainDimensions = new Size(terrainTexture.Width, terrainTexture.Height);

        var area = terrainDimensions.Area();
        var rawPixels = new PixelType[area];
        TerrainPixels = new ArrayWrapper2D<PixelType>(rawPixels, terrainDimensions);
        var rawColors = new Color[area];
        TerrainColors = new ArrayWrapper2D<Color>(rawColors, terrainDimensions);

        _terrainArchetypeDataLookup = StyleCache.GetAllTerrainArchetypeData(levelData);
    }

    public void PaintTerrain()
    {
        LoadAllColorData();

        foreach (var terrainGroup in _levelData.AllTerrainGroups)
        {
            ProcessTerrainGroup(terrainGroup);
        }

        var textureData = new ArrayWrapper2D<Color>(TerrainColors.Array, new Size(TerrainTexture.Width, TerrainTexture.Height));

        DrawTerrainPieces(_levelData.AllTerrainInstanceData, in textureData);
    }

    public void Apply()
    {
        TerrainTexture.SetData(TerrainColors.Array);
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
            in terrainPixelColorData);

        terrainGroupData.TerrainPixelColorData = ArrayWrapperHelpers.Trim(in terrainPixelColorData);
    }

    private void DrawTerrainPieces(
        List<TerrainInstanceData> terrainDataList,
        in ArrayWrapper2D<Color> targetData)
    {
        foreach (var terrainData in terrainDataList)
        {
            if (terrainData.GroupName is null)
            {
                var terrainArchetypeData = GetTerrainArchetypeData(terrainData);

                if (terrainArchetypeData.ResizeType == ResizeType.None)
                {
                    DrawTerrainPiece(
                        terrainData,
                        terrainArchetypeData,
                        in targetData);
                }
                else
                {
                    PaintResizeableTerrainPiece(
                        terrainData,
                        terrainArchetypeData,
                        in targetData);
                }
            }
            else
            {
                var textureGroup = _levelData.AllTerrainGroups.Find(tg => string.Equals(tg.GroupName, terrainData.GroupName, StringComparison.OrdinalIgnoreCase))!;
                DrawTerrainPiece(
                    terrainData,
                    textureGroup,
                    in targetData);
            }
        }
    }

    private void DrawTerrainPiece(
        TerrainInstanceData terrainData,
        ITerrainArchetypeData terrainArchetypeData,
        in ArrayWrapper2D<Color> targetPixelColorData)
    {
        var sourcePixelColorData = _colorDataLookup[terrainData.GetStylePiecePair()];

        var sourceSize = sourcePixelColorData.Size;
        var targetSize = targetPixelColorData.Size;

        var transformationData = new DihedralTransformation.TransformationData(
            terrainData.Orientation,
            terrainData.FacingDirection,
            sourceSize);

        var hueMultiplyMatrix = new HueMultiplyMatrix(terrainData.HueAngle);

        for (var y = 0; y < sourceSize.H; y++)
        {
            for (var x = 0; x < sourceSize.W; x++)
            {
                var p = new Point(x, y);
                var sourcePixelColor = sourcePixelColorData[p];
                if (sourcePixelColor == Color.Transparent)
                    continue;

                var p0 = transformationData.Transform(p);

                p0 += terrainData.Position + _terrainOffset;

                if (targetSize.EncompassesPoint(p0))
                {
                    ChangePixel(terrainData, terrainArchetypeData, in targetPixelColorData, in hueMultiplyMatrix, sourcePixelColor, p0);
                }
            }
        }
    }

    private void ChangePixel(
        TerrainInstanceData terrainData,
        ITerrainArchetypeData terrainArchetypeData,
        in ArrayWrapper2D<Color> targetPixelColorData,
        in HueMultiplyMatrix matrix,
        Color sourcePixelColor,
        Point p0)
    {
        if (terrainData.HueAngle != 0)
        {
            sourcePixelColor = matrix.ShiftHue(sourcePixelColor);
        }

        ref var targetPixelColor = ref targetPixelColorData[p0];
        ref var targetPixelData = ref TerrainPixels[p0];

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

    private void PaintResizeableTerrainPiece(
        TerrainInstanceData terrainData,
        TerrainArchetypeData terrainArchetypeData,
        in ArrayWrapper2D<Color> targetPixelColorData)
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
        var newA = 1f - (1f - fgA) * (1f - bgA);
        var newR = fgR * fgA / newA + bgR * bgA * (1f - fgA) / newA;
        var newG = fgG * fgA / newA + bgG * bgA * (1f - fgA) / newA;
        var newB = fgB * fgA / newA + bgB * bgA * (1f - fgA) / newA;

        return new Color(newR, newG, newB, newA);
    }

    private static bool PixelColorIsSubstantial(Color color)
    {
        uint alpha = color.A;
        return alpha >= EngineConstants.MinimumSubstantialAlphaValue;
    }

    private void LoadAllColorData()
    {
        foreach (var terrainData in _levelData.AllTerrainInstanceData)
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

    private void LoadColorData(TerrainInstanceData terrainData)
    {
        var stylePiecePair = terrainData.GetStylePiecePair();

        ref var colorData = ref CollectionsMarshal.GetValueRefOrAddDefault(_colorDataLookup, stylePiecePair, out var exists);
        if (exists)
            return;

        var pngPath = GetTerrainArchetypeData(terrainData).TextureFilePath;

        var mainTexture = TextureCache.GetOrLoadTexture(
            pngPath,
            terrainData.StyleIdentifier,
            terrainData.PieceIdentifier,
            TextureType.TerrainSprite);
        colorData = ArrayWrapperHelpers.GetPixelColorDataFromTexture(mainTexture);
    }

    private TerrainArchetypeData GetTerrainArchetypeData(TerrainInstanceData terrainData)
    {
        return StyleCache.GetTerrainArchetypeData(terrainData.StyleIdentifier, terrainData.PieceIdentifier, _levelData.FileFormatType);
    }

    private readonly struct HueMultiplyMatrix
    {
        private const float OneThird = 1f / 3f;
        private const float SqrtOneThird = 0.5773502691896257645091488f;

        private readonly float _00;
        private readonly float _01;
        private readonly float _02;
        private readonly float _10;
        private readonly float _11;
        private readonly float _12;
        private readonly float _20;
        private readonly float _21;
        private readonly float _22;

        public HueMultiplyMatrix(uint angle)
        {
            var radianValue = angle * MathF.PI / 180f;

            var (sinA, cosA) = MathF.SinCos(radianValue);

            _00 = cosA + ((1f - cosA) / 3f);
            _01 = (OneThird * (1f - cosA)) - (SqrtOneThird * sinA);
            _02 = (OneThird * (1f - cosA)) + (SqrtOneThird * sinA);
            _10 = (OneThird * (1f - cosA)) + (SqrtOneThird * sinA);
            _11 = cosA + (OneThird * (1f - cosA));
            _12 = (OneThird * (1f - cosA)) - (SqrtOneThird * sinA);
            _20 = (OneThird * (1f - cosA)) - (SqrtOneThird * sinA);
            _21 = (OneThird * (1f - cosA)) + (SqrtOneThird * sinA);
            _22 = cosA + (OneThird * (1f - cosA));
        }

        public Color ShiftHue(Color color)
        {
            var r = (float)color.R;
            var g = (float)color.G;
            var b = (float)color.B;

            var rx = r * _00 + g * _01 + b * _02;
            var gx = r * _10 + g * _11 + b * _12;
            var bx = r * _20 + g * _21 + b * _22;

            return new Color((int)rx, (int)gx, (int)bx);
        }
    }
}
