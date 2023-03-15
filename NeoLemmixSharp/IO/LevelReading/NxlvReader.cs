using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Lemming;
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

        var bools = new bool[width * height];

        DrawTerrain(_dataParser.TerrainData, levelTerrainTexture, bools);

        levelTitle = string.IsNullOrWhiteSpace(levelTitle)
            ? "Untitled"
            : levelTitle;

        var lemming = new Lemming();
        lemming.X = 160;
        lemming.Y = 80;

        return new LevelScreen(levelTitle, width, height, bools)
        {
            LevelObjects = new ITickable[] { lemming },
            LevelSprites = new IRenderable[] { new LemmingSprite(_graphicsDevice, lemming) },

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

        var bools = new bool[maxX * maxY];

        DrawTerrain(terrainGroup.TerrainDatas, texture, bools);

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

    private void DrawTerrain(List<TerrainData> terrainDataList, RenderTarget2D renderTarget, bool[] bools)
    {
      //  terrainDataList.Sort(SortTerrainEntries);

        _graphicsDevice.SetRenderTarget(renderTarget);
        _graphicsDevice.Clear(Color.Transparent);

        foreach (var terrainData in terrainDataList)
        {
            ApplyTerrainPiece(terrainData, renderTarget, bools);
        }

        _graphicsDevice.SetRenderTarget(null);
    }

    /*private static int SortTerrainEntries(TerrainData x, TerrainData y)
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
    }*/

    private void ApplyTerrainPiece(TerrainData terrainData, RenderTarget2D renderTarget2D, bool[] bools)
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

            var data = new int[terrainTexture.Height * terrainTexture.Width];
            terrainTexture.GetData(data);
            var currentBools = Array.ConvertAll(data, ToBoolData);

            ApplySmallerArrayOntoBigger(
                currentBools,
                terrainTexture.Width,
                terrainTexture.Height,
                bools,
                renderTarget2D.Width,
                renderTarget2D.Height,
                terrainData.Erase,
                terrainData.X,
                terrainData.Y);
        }
    }

    private static bool ToBoolData(int pixel)
    {
        var alpha = (pixel >> 24) & 0xff;
        var colourData = pixel & 0x00ffffff;
        return alpha > 32 && colourData != 0;
    }

    private void ApplySmallerArrayOntoBigger(
        bool[] smallerBools,
        int smallerWidth,
        int smallerHeight,
        bool[] biggerBools,
        int biggerWidth,
        int biggerHeight,
        bool erase,
        int posX,
        int posY)
    {
        var wrappedSmallerBools = new ArrayWrapper2D<bool>(smallerWidth, smallerHeight, smallerBools);
        var wrappedBiggerBools = new ArrayWrapper2D<bool>(biggerWidth, biggerHeight, biggerBools);
        for (var x = 0; x < smallerWidth; x++)
        {
            for (var y = 0; y < smallerHeight; y++)
            {
                var newX = x + posX;
                if (newX < 0 || newX >= biggerWidth)
                    continue;
                var newY = y + posY;
                if (newY < 0 || newY >= biggerHeight)
                    continue;

                var val = wrappedSmallerBools.Get(x, y);
                if (erase)
                {
                    wrappedBiggerBools.Set(newX, newY, !val);
                }
                else
                {
                    wrappedBiggerBools.Set(newX, newY, val);
                }
            }
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