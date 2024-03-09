using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

public sealed class PixelColorData
{
    public uint[] ColorData { get; }

    public int Width { get; }
    public int Height { get; }

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

    public uint Get(int x, int y)
    {
        var i = Width * y + x;

        return ColorData[i];
    }

    public void Set(int x, int y, uint pixel)
    {
        var i = Width * y + x;

        ColorData[i] = pixel;
    }
}