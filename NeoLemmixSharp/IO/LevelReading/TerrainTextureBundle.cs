using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace NeoLemmixSharp.IO.LevelReading;

public sealed class TerrainTextureBundle : IDisposable
{
    private bool _disposed;

    private Texture2D? _mainTexture;
    private readonly Dictionary<int, Texture2D> _textureTransformationLookup = new();

    public TerrainTextureBundle(Texture2D mainTexture)
    {
        var key = GetKey(false, false, false, false);
        _textureTransformationLookup.Add(key, mainTexture);

        _mainTexture = mainTexture;
    }

    public Texture2D GetTransformedTexture(
        GraphicsDevice graphicsDevice,
        bool flipHorizontally,
        bool flipVertically,
        bool rotate,
        bool erase)
    {
        if (!flipHorizontally &&
            !flipVertically &&
            !rotate &&
            !erase)
            return _mainTexture!;

        var rotNum = rotate
            ? 1
            : 0;

        if (flipVertically)
        {
            flipHorizontally = !flipHorizontally;
            rotNum += 2;
        }

        var flipNum = flipHorizontally
            ? 4
            : 0;

        var eraseNum = erase
            ? 8
            : 0;

        var key = eraseNum | flipNum | rotNum;

        if (_textureTransformationLookup.TryGetValue(key, out var result))
            return result;

        int newWidth;
        int newHeight;
        if ((rotNum & 1) != 0)
        {
            newWidth = _mainTexture!.Height;
            newHeight = _mainTexture.Width;
        }
        else
        {
            newWidth = _mainTexture!.Width;
            newHeight = _mainTexture.Height;
        }

        var newTexture = new Texture2D(graphicsDevice, newWidth, newHeight);
        TransformTexture(newTexture, flipHorizontally, rotNum, erase);

        _textureTransformationLookup.Add(key, newTexture);

        return newTexture;
    }

    public static int GetKey(
        bool flipHorizontally,
        bool flipVertically,
        bool rotate,
        bool erase)
    {
        var rotNum = rotate
            ? 1
            : 0;

        if (flipVertically)
        {
            flipHorizontally = !flipHorizontally;
            rotNum += 2;
        }

        var flipNum = flipHorizontally
            ? 4
            : 0;

        var eraseNum = erase
            ? 8
            : 0;

        return eraseNum | flipNum | rotNum;
    }

    private void TransformTexture(
        Texture2D textureToDraw,
        bool flipHorizontally,
        int rotNum,
        bool erase)
    {
        var size = _mainTexture!.Height * _mainTexture.Width;
        var originalData = new int[size];
        _mainTexture.GetData(originalData);
        var newData = new int[size];

        var originalWrapper = new ArrayWrapper2D<int>(_mainTexture.Width, _mainTexture.Height, originalData);
        var newWrapper = new ArrayWrapper2D<int>(textureToDraw.Width, textureToDraw.Height, newData);

        if (erase)
        {
            DrawErase(originalWrapper, newWrapper, flipHorizontally, rotNum);
        }
        else
        {
            DrawNormal(originalWrapper, newWrapper, flipHorizontally, rotNum);
        }

        textureToDraw.SetData(newData);
    }

    private void DrawErase(
        ArrayWrapper2D<int> originalData,
        ArrayWrapper2D<int> newData,
        bool flipHorizontally,
        int rotNum)
    {
        var a = 0;
        var b = 0;
        var w = 0;
        var h = 0;

        int m;
        int s;
        if (flipHorizontally)
        {
            m = -1;
            s = newData.Width - 1;
        }
        else
        {
            m = 1;
            s = 0;
        }

        switch (rotNum)
        {
            case 0:
                a = 1;
                b = 0;
                w = 0;
                h = 0;
                break;
            case 1:
                a = 0;
                b = 1;
                w = originalData.Height - 1;
                h = 0;
                break;
            case 2:
                a = -1;
                b = 0;
                w = originalData.Width - 1;
                h = originalData.Height - 1;
                break;
            case 3:
                a = 0;
                b = -1;
                w = 0;
                h = originalData.Width - 1;
                break;
        }

        for (var x = 0; x < originalData.Width; x++)
        {
            for (var y = 0; y < originalData.Height; y++)
            {
                var pixel = originalData.Get(x, y);

                var x0 = s + m * (a * x - b * y + w);
                var y0 = b * x + a * y + h;

                var alpha = (pixel >> 24) & 0xff;
                var colourData = pixel & 0x00ffffff;
                if (alpha > 32 && colourData != 0)
                {
                    pixel = -1; //0xff << 24;
                }
                else
                {
                    pixel = 0;
                }

                newData.Set(x0, y0, pixel);
            }
        }
    }

    private static void DrawNormal(
        ArrayWrapper2D<int> originalData,
        ArrayWrapper2D<int> newData,
        bool flipHorizontally,
        int rotNum)
    {
        var a = 0;
        var b = 0;
        var w = 0;
        var h = 0;

        int m;
        int s;
        if (flipHorizontally)
        {
            m = -1;
            s = newData.Width - 1;
        }
        else
        {
            m = 1;
            s = 0;
        }

        switch (rotNum)
        {
            case 0:
                a = 1;
                b = 0;
                w = 0;
                h = 0;
                break;
            case 1:
                a = 0;
                b = 1;
                w = originalData.Height - 1;
                h = 0;
                break;
            case 2:
                a = -1;
                b = 0;
                w = originalData.Width - 1;
                h = originalData.Height - 1;
                break;
            case 3:
                a = 0;
                b = -1;
                w = 0;
                h = originalData.Width - 1;
                break;
        }

        for (var x = 0; x < originalData.Width; x++)
        {
            for (var y = 0; y < originalData.Height; y++)
            {
                var pixel = originalData.Get(x, y);

                var x0 = s + m * (a * x - b * y + w);
                var y0 = b * x + a * y + h;

                newData.Set(x0, y0, pixel);
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        foreach (var texture in _textureTransformationLookup.Values)
        {
            texture.Dispose();
        }

        _textureTransformationLookup.Clear();
        _mainTexture = null;
        _disposed = true;
    }
}