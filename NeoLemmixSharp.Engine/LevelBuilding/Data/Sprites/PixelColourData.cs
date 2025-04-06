using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

public readonly struct PixelColorData
{
    public readonly Size Size;

    private readonly Color[] _colorData;

    public static PixelColorData GetPixelColorDataFromTexture(Texture2D texture)
    {
        var size = new Size(texture);
        var data = new Color[size.Area()];

        texture.GetData(data);

        return new PixelColorData(size, data);
    }

    public PixelColorData(
        Size size,
        Color[] colorData)
    {
        Size = size;
        _colorData = colorData;
    }

    public Color this[Point p]
    {
        get
        {
            Size.AssertEncompassesPoint(p);
            var index = Size.GetIndexOfPoint(p);
            return _colorData[index];
        }
        set
        {
            Size.AssertEncompassesPoint(p);
            var index = Size.GetIndexOfPoint(p);
            _colorData[index] = value;
        }
    }

    public Texture2D CreateTexture(GraphicsDevice graphicsDevice)
    {
        var result = new Texture2D(graphicsDevice, Size.W, Size.H);
        result.SetData(_colorData);
        return result;
    }

    public PixelColorData Trim()
    {
        var minX = GetMinXTrim();
        var minY = GetMinYTrim();
        var maxX = GetMaxXTrim();
        var maxY = GetMaxYTrim();

        if (minX < 0 || maxX < 0 ||
            minY < 0 || maxY < 0)
            return new PixelColorData(new Size(), []);

        var subRegion = new Region(
            new Point(minX, minY),
            new Point(maxX, maxY));

        // Don't need to trim at all in this case
        if (subRegion.Size == Size)
            return this;

        var newColorBuffer = new Color[subRegion.Size.Area()];

        // Transfer the actual color data into a brand new item
        var sourceTextureWrapper = new ArrayWrapper2D<Color>(_colorData, Size, subRegion);
        var resultTextureWrapper = new ArrayWrapper2D<Color>(newColorBuffer, subRegion.Size);

        for (var y = 0; y < subRegion.H; y++)
        {
            for (var x = 0; x < subRegion.W; x++)
            {
                var pos = new Point(x, y);
                resultTextureWrapper[pos] = sourceTextureWrapper[pos];
            }
        }

        return new PixelColorData(
            subRegion.Size,
            newColorBuffer);
    }

    private int GetMinXTrim()
    {
        for (var x = 0; x < Size.W; x++)
        {
            for (var y = 0; y < Size.H; y++)
            {
                var pos = new Point(x, y);
                if (this[pos] != Color.Transparent)
                {
                    return x;
                }
            }
        }

        return -1;
    }

    private int GetMinYTrim()
    {
        for (var y = 0; y < Size.H; y++)
        {
            for (var x = 0; x < Size.W; x++)
            {
                var pos = new Point(x, y);
                if (this[pos] != Color.Transparent)
                {
                    return y;
                }
            }
        }

        return -1;
    }

    private int GetMaxXTrim()
    {
        for (var x = Size.W - 1; x >= 0; x--)
        {
            for (var y = 0; y < Size.H; y++)
            {
                var pos = new Point(x, y);
                if (this[pos] != Color.Transparent)
                {
                    return x;
                }
            }
        }

        return -1;
    }

    private int GetMaxYTrim()
    {
        for (var y = Size.H - 1; y >= 0; y--)
        {
            for (var x = 0; x < Size.W; x++)
            {
                var pos = new Point(x, y);
                if (this[pos] != Color.Transparent)
                {
                    return y;
                }
            }
        }

        return -1;
    }
}