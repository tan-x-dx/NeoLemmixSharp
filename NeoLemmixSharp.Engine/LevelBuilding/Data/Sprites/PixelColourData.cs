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

        // If first value is negative, then the data is completely blank
        if (minX < 0)
            return new ArrayWrapper2D<Color>([], new Size());        

        var minY = GetMinYTrim(data);
        var maxX = GetMaxXTrim(data);
        var maxY = GetMaxYTrim(data);

        var subRegion = new RectangularRegion(
            new Point(minX, minY),
            new Point(maxX, maxY));

        return new ArrayWrapper2D<Color>(data.Array, data.Size, subRegion);
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