using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

public readonly struct PixelColorData
{
    public readonly int Width;
    public readonly int Height;

    public readonly uint[] ColorData;

    public static PixelColorData GetPixelColorDataFromTexture(Texture2D texture)
    {
        var width = texture.Width;
        var height = texture.Height;
        var data = new uint[width * height];

        texture.GetData(data);

        return new PixelColorData(width, height, data);
    }

    public PixelColorData(int width, int height, uint[] colorData)
    {
        Width = width;
        Height = height;
        ColorData = colorData;
    }

    public uint this[int x, int y]
    {
        get
        {
            var i = Width * y + x;

            return ColorData[i];
        }
        set
        {
            var i = Width * y + x;

            ColorData[i] = value;
        }
    }
}