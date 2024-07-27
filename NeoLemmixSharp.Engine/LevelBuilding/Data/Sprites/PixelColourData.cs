using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

public readonly struct PixelColorData
{
    public readonly int Width;
    public readonly int Height;

    private readonly Color[] _colorData;

    public static PixelColorData GetPixelColorDataFromTexture(Texture2D texture)
    {
        var width = texture.Width;
        var height = texture.Height;
        var data = new Color[width * height];

        texture.GetData(data);

        return new PixelColorData(width, height, data);
    }

    public PixelColorData(
        int width,
        int height,
        Color[] colorData)
    {
        Width = width;
        Height = height;
        _colorData = colorData;
    }

    public Color this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= Width ||
                y < 0 || y >= Height)
                throw new ArgumentException("Invalid dimensions");

            var i = Width * y + x;

            return _colorData[i];
        }
        set
        {
            if (x < 0 || x >= Width ||
                y < 0 || y >= Height)
                throw new ArgumentException("Invalid dimensions");

            var i = Width * y + x;

            _colorData[i] = value;
        }
    }

    public Texture2D CreateTexture(GraphicsDevice graphicsDevice)
    {
        var result = new Texture2D(graphicsDevice, Width, Height);
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
            return new PixelColorData(0, 0, []);

        var newWidth = 1 + maxX - minX;
        var newHeight = 1 + maxY - minY;

        var newColors = new Color[newWidth * newHeight];

        var sourceTextureWrapper = new SpanWrapper2D<Color>(_colorData, Width, Height, minX, minY, newWidth, newHeight);
        var resultTextureWrapper = new SpanWrapper2D<Color>(newColors, newWidth, newHeight, 0, 0, newWidth, newHeight);

        for (var x = 0; x < sourceTextureWrapper.Width; x++)
        {
            for (var y = 0; y < sourceTextureWrapper.Height; y++)
            {
                resultTextureWrapper[x, y] = sourceTextureWrapper[x, y];
            }
        }

        return new PixelColorData(
            newWidth,
            newHeight,
            newColors);
    }

    private int GetMinXTrim()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                if (this[x, y] != Color.Transparent)
                {
                    return x;
                }
            }
        }

        return -1;
    }

    private int GetMinYTrim()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if (this[x, y] != Color.Transparent)
                {
                    return y;
                }
            }
        }

        return -1;
    }

    private int GetMaxXTrim()
    {
        for (var x = Width - 1; x >= 0; x--)
        {
            for (var y = 0; y < Height; y++)
            {
                if (this[x, y] != Color.Transparent)
                {
                    return x;
                }
            }
        }

        return -1;
    }

    private int GetMaxYTrim()
    {
        for (var y = Height - 1; y >= 0; y--)
        {
            for (var x = 0; x < Width; x++)
            {
                if (this[x, y] != Color.Transparent)
                {
                    return y;
                }
            }
        }

        return -1;
    }
}