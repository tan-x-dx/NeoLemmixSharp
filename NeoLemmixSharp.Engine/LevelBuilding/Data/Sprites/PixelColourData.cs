using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

public static class PixelColorDataHelpers
{
    public static ArrayWrapper2D<Color> GetPixelColorDataFromTexture(Texture2D texture)
    {
        var size = new Size(texture.Width, texture.Height);
        var data = new Color[size.Area()];

        texture.GetData(data);

        return new ArrayWrapper2D<Color>(data, size);
    }

    public static ArrayWrapper2D<Color> Trim(this ArrayWrapper2D<Color> data)
    {
        var minX = GetMinXTrim(data);
        var minY = GetMinYTrim(data);
        var maxX = GetMaxXTrim(data);
        var maxY = GetMaxYTrim(data);

        if (minX < 0 || maxX < 0 ||
            minY < 0 || maxY < 0)
            return new ArrayWrapper2D<Color>([], new Size());

        var subRegion = new Region(
            new Point(minX, minY),
            new Point(maxX, maxY));

        // Don't need to trim at all in this case
        if (subRegion.Size == data.Size)
            return data;

        var newColorBuffer = new Color[subRegion.Size.Area()];

        // Transfer the actual color data into a brand new item
        var sourceTextureWrapper = new ArrayWrapper2D<Color>(data.Array, data.Size, subRegion);
        var resultTextureWrapper = new ArrayWrapper2D<Color>(newColorBuffer, subRegion.Size);

        for (var y = 0; y < subRegion.H; y++)
        {
            for (var x = 0; x < subRegion.W; x++)
            {
                var pos = new Point(x, y);
                resultTextureWrapper[pos] = sourceTextureWrapper[pos];
            }
        }

        return new ArrayWrapper2D<Color>(
            newColorBuffer,
            subRegion.Size);
    }

    private static int GetMinXTrim(ArrayWrapper2D<Color> data)
    {
        for (var x = 0; x < data.Size.W; x++)
        {
            for (var y = 0; y < data.Size.H; y++)
            {
                var pos = new Point(x, y);
                if (data[pos] != Color.Transparent)
                {
                    return x;
                }
            }
        }

        return -1;
    }

    private static int GetMinYTrim(ArrayWrapper2D<Color> data)
    {
        for (var y = 0; y < data.Size.H; y++)
        {
            for (var x = 0; x < data.Size.W; x++)
            {
                var pos = new Point(x, y);
                if (data[pos] != Color.Transparent)
                {
                    return y;
                }
            }
        }

        return -1;
    }

    private static int GetMaxXTrim(ArrayWrapper2D<Color> data)
    {
        for (var x = data.Size.W - 1; x >= 0; x--)
        {
            for (var y = 0; y < data.Size.H; y++)
            {
                var pos = new Point(x, y);
                if (data[pos] != Color.Transparent)
                {
                    return x;
                }
            }
        }

        return -1;
    }

    private static int GetMaxYTrim(ArrayWrapper2D<Color> data)
    {
        for (var y = data.Size.H - 1; y >= 0; y--)
        {
            for (var x = 0; x < data.Size.W; x++)
            {
                var pos = new Point(x, y);
                if (data[pos] != Color.Transparent)
                {
                    return y;
                }
            }
        }

        return -1;
    }
}