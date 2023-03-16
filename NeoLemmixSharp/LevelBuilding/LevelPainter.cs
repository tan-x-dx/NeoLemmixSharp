using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.LevelBuilding.Painting;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Terrain;
using NeoLemmixSharp.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class LevelPainter : IDisposable
{
    private const string _rootDirectory = "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5";

    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly Dictionary<string, TextureData> _textureBundleCache = new();

    private readonly List<TerrainGroup> _terrainGroups = new();

    private bool _disposed;

    private TerrainSprite? _terrainSprite;

    public LevelPainter(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
    }

    public void PaintLevel(
        LevelData levelData,
        IEnumerable<TerrainGroup> terrainGroups,
        IEnumerable<TerrainData> terrainData)
    {
        _terrainGroups.AddRange(terrainGroups);

        foreach (var terrainGroup in _terrainGroups)
        {
            ProcessTerrainGroupTexture(terrainGroup);
        }

        var levelTerrainTexture = new Texture2D(
            _graphicsDevice,
            levelData.LevelWidth,
            levelData.LevelHeight);

        var uintData = new uint[levelData.LevelWidth * levelData.LevelHeight];
        var textureData = new TextureData(
            levelData.LevelWidth,
            levelData.LevelHeight,
            uintData,
            false);

        DrawTerrain(terrainData, textureData);
        levelTerrainTexture.SetData(uintData);
        _terrainSprite = new TerrainSprite(
            levelData.LevelWidth,
            levelData.LevelHeight,
            levelTerrainTexture);
    }

    private void ProcessTerrainGroupTexture(TerrainGroup terrainGroup)
    {
        var minX = terrainGroup.TerrainDatas.Select(td => td.X).Min();
        var minY = terrainGroup.TerrainDatas.Select(td => td.Y).Min();

        foreach (var terrainData in terrainGroup.TerrainDatas)
        {
            terrainData.X -= minX;
            terrainData.Y -= minY;
        }

        var maxX = terrainGroup.TerrainDatas.Select(GetMaxX).Max();
        var maxY = terrainGroup.TerrainDatas.Select(GetMaxY).Max();

        var textureData = new TextureData(
            maxX,
            maxY,
            new uint[maxX * maxY],
            false);

        DrawTerrain(terrainGroup.TerrainDatas, textureData);

        terrainGroup.TextureData = textureData;
    }

    private int GetMaxX(TerrainData terrainData)
    {
        var textureBundle = GetOrLoadTextureBundle(terrainData);
        var w = terrainData.Rotate
            ? textureBundle.Height
            : textureBundle.Width;

        return terrainData.X + w;
    }

    private int GetMaxY(TerrainData terrainData)
    {
        var textureBundle = GetOrLoadTextureBundle(terrainData);
        var h = terrainData.Rotate
            ? textureBundle.Width
            : textureBundle.Height;

        return terrainData.Y + h;
    }

    private void DrawTerrain(IEnumerable<TerrainData> terrainDataList, TextureData targetData)
    {
        foreach (var terrainData in terrainDataList)
        {
            ApplyTerrainPiece(terrainData, targetData);
        }
    }

    private void ApplyTerrainPiece(TerrainData terrainData, TextureData targetData)
    {
        var sourceData = GetOrLoadTextureBundle(terrainData);

        var dihedralTransformation = DihedralTransformation.GetForTransformation(
            terrainData.FlipHorizontal,
            terrainData.FlipVertical,
            terrainData.Rotate);

        for (var x = 0; x < sourceData.Width; x++)
        {
            for (var y = 0; y < sourceData.Height; y++)
            {
                dihedralTransformation.Transform(
                    sourceData.Width - 1,
                    sourceData.Height - 1,
                    x,
                    y,
                    out var x0,
                    out var y0);

                x0 += terrainData.X;
                y0 += terrainData.Y;

                if (x0 < 0 || x0 >= targetData.Width ||
                    y0 < 0 || y0 >= targetData.Height)
                    continue;

                var pixel = sourceData.Get(x, y);

                if (terrainData.Tint.HasValue)
                {
                    pixel = BlendColours(terrainData.Tint.Value, pixel);
                }

                var targetPixel = targetData.Get(x0, y0);

                if (terrainData.Erase)
                {
                    if (pixel != 0U)
                    {
                        targetPixel = 0U;
                    }
                }
                else if (terrainData.NoOverwrite)
                {
                    targetPixel = BlendColours(targetPixel, pixel);
                }
                else
                {
                    targetPixel = BlendColours(pixel, targetPixel);
                }
                targetData.Set(x0, y0, targetPixel);
            }
        }
    }

    private static uint BlendColours(uint foreground, uint background)
    {
        var fgA = ((foreground >> 24) & 0xffU) / 255d;
        var fgR = ((foreground >> 16) & 0xffU) / 255d;
        var fgG = ((foreground >> 8) & 0xffU) / 255d;
        var fgB = (foreground & 0xffU) / 255d;
        var bgA = ((background >> 24) & 0xffU) / 255d;
        var bgR = ((background >> 16) & 0xffU) / 255d;
        var bgG = ((background >> 8) & 0xffU) / 255d;
        var bgB = (background & 0xffU) / 255d;
        var newA = 1.0 - (1.0 - fgA) * (1.0 - bgA);
        var newR = fgR * fgA / newA + bgR * bgA * (1.0 - fgA) / newA;
        var newG = fgG * fgA / newA + bgG * bgA * (1 - fgA) / newA;
        var newB = fgB * fgA / newA + bgB * bgA * (1 - fgA) / newA;
        var a = (uint)Math.Round(newA * 255);
        var r = (uint)Math.Round(newR * 255);
        var g = (uint)Math.Round(newG * 255);
        var b = (uint)Math.Round(newB * 255);

        return (a << 24) | (r << 16) | (g << 8) | b;
    }

    private TextureData GetOrLoadTextureBundle(TerrainData terrainData)
    {
        if (terrainData.GroupId != null)
        {
            var textureGroup = _terrainGroups.First(tg => tg.GroupId == terrainData.GroupId);

            return textureGroup.TextureData!;
        }

        var rootFilePath = Path.Combine(_rootDirectory, "styles", terrainData.Style!, "terrain", terrainData.TerrainName!);

        if (_textureBundleCache.TryGetValue(rootFilePath, out var result))
            return result;

        var png = Path.ChangeExtension(rootFilePath, "png");
        var isSteel = File.Exists(Path.ChangeExtension(rootFilePath, "nxmt"));

        var mainTexture = Texture2D.FromFile(_graphicsDevice, png);
        result = GetTextureData(mainTexture, isSteel);
        _textureBundleCache.Add(rootFilePath, result);

        return result;
    }

    private static TextureData GetTextureData(Texture2D texture, bool isSteel)
    {
        var width = texture.Width;
        var height = texture.Height;
        var data = new uint[width * height];

        texture.GetData(data);

        return new TextureData(width, height, data, isSteel);
    }

    public SpriteBank GetSpriteBank()
    {
        return null;
    }

    public LevelTerrain GetLevelTerrain()
    {
        return new LevelTerrain(0, 0);
    }

    public TerrainSprite GetTerrainSprite()
    {
        return _terrainSprite!;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _textureBundleCache.Clear();
        _terrainGroups.Clear();
        _disposed = true;
    }
}