using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Terrain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoLemmixSharp.IO.LevelReading;

public sealed class NxlvReader : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly string _filePath;

    private const string _rootDirectory = "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5";

    private readonly DataParser _dataParser = new(_rootDirectory);
    private readonly Dictionary<string, TerrainTextureBundle> _textureBundleCache = new();

    private bool _disposed;

    public NxlvReader(GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        string filePath)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        _filePath = filePath;
    }

    public LevelScreen CreateLevelFromFile()
    {
        var lines = File.ReadAllLines(_filePath);

        _dataParser.ParseLevel(lines);

        var width = _dataParser.GetIntDataByToken("WIDTH");
        var height = _dataParser.GetIntDataByToken("HEIGHT");
        var levelTitle = _dataParser.GetStringDataByToken("TITLE");

        foreach (var terrainGroup in _dataParser.TerrainGroups)
        {
            ProcessTerrainGroupTexture(terrainGroup);
        }

        var levelTerrainTexture = new RenderTarget2D(
            _graphicsDevice,
            width,
            height);

        DrawTerrain(_dataParser.TerrainData, levelTerrainTexture);

        levelTitle = string.IsNullOrWhiteSpace(levelTitle)
            ? "Untitled"
            : levelTitle;

        return new LevelScreen(levelTitle)
        {
            LevelObjects = new ITickable[0],
            LevelSprites = new IRenderable[0],

            TerrainSprite = new TerrainSprite(width, height, levelTerrainTexture),
            Viewport = new NeoLemmixViewPort()
        };
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

        var texture = new RenderTarget2D(
            _graphicsDevice,
            maxX,
            maxY,
            false,
            _graphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24Stencil8);

        DrawTerrain(terrainGroup.TerrainDatas, texture);

        var textureBundle = new TerrainTextureBundle(texture);
        terrainGroup.TerrainTextureBundle = textureBundle;
    }

    private int GetMaxX(TerrainData terrainData)
    {
        var textureBundle = GetOrLoadTextureBundle(terrainData);
        var terrainTexture = textureBundle.GetTransformedTexture(
            _graphicsDevice,
            terrainData.FlipHorizontal,
            terrainData.FlipVertical,
            terrainData.Rotate,
            terrainData.Erase);

        return terrainData.X + terrainTexture.Width;
    }

    private int GetMaxY(TerrainData terrainData)
    {
        var textureBundle = GetOrLoadTextureBundle(terrainData);
        var terrainTexture = textureBundle.GetTransformedTexture(
            _graphicsDevice,
            terrainData.FlipHorizontal,
            terrainData.FlipVertical,
            terrainData.Rotate,
            terrainData.Erase);

        return terrainData.Y + terrainTexture.Height;
    }

    private void DrawTerrain(List<TerrainData> terrainDataList, RenderTarget2D renderTarget)
    {
        terrainDataList.Sort(SortTerrainEntries);

        _graphicsDevice.SetRenderTarget(renderTarget);
        _graphicsDevice.Clear(Color.Transparent);

        foreach (var terrainData in terrainDataList)
        {
            ApplyTerrainPiece(terrainData, renderTarget);
        }

        _graphicsDevice.SetRenderTarget(null);
    }

    private static int SortTerrainEntries(TerrainData x, TerrainData y)
    {
        if (x.NoOverwrite != y.NoOverwrite)
        {
            if (x.NoOverwrite)
                return -1;
            return 1;
        }

        var mult = x.NoOverwrite
            ? -1
            : 1;

        return x.Id.CompareTo(y.Id) * mult;
    }

    private void ApplyTerrainPiece(TerrainData terrainData, RenderTarget2D renderTarget2D)
    {
        var textureBundle = GetOrLoadTextureBundle(terrainData);

        var terrainTexture = textureBundle.GetTransformedTexture(
            _graphicsDevice,
            terrainData.FlipHorizontal,
            terrainData.FlipVertical,
            terrainData.Rotate,
            terrainData.Erase);

        var pos = new Vector2(terrainData.X, terrainData.Y);
        if (terrainData.Erase)
        {
            using var s1 = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false,
            };

            using var s2 = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.LessEqual,
                StencilPass = StencilOperation.Keep,
                ReferenceStencil = 1,
                DepthBufferEnable = false,
            };

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp);
            _spriteBatch.Draw(terrainTexture, pos, Color.White);
            _spriteBatch.End();
            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp);
            _spriteBatch.Draw(terrainTexture, pos, Color.White);
            _spriteBatch.End();
        }
        else
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(terrainTexture, pos, Color.White);
            _spriteBatch.End();
        }
    }

    private TerrainTextureBundle GetOrLoadTextureBundle(TerrainData terrainData)
    {
        if (terrainData.CurrentParsingPath == null)
        {
            var textureGroup = _dataParser.TerrainGroups.Find(tg => tg.GroupId == terrainData.GroupId)!;

            return textureGroup.TerrainTextureBundle!;
        }

        var filePath = terrainData.CurrentParsingPath!;

        if (_textureBundleCache.TryGetValue(filePath, out var result))
            return result;

        var mainTexture = Texture2D.FromFile(_graphicsDevice, filePath);
        result = new TerrainTextureBundle(mainTexture);
        _textureBundleCache.Add(filePath, result);

        return result;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _dataParser.Dispose();
        foreach (var terrainTextureBundle in _textureBundleCache.Values)
        {
            terrainTextureBundle.Dispose();
        }
        _textureBundleCache.Clear();
        _disposed = true;
    }
}