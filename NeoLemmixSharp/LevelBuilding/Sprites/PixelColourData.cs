using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.LevelBuilding.Sprites;

public sealed class PixelColourData
{
    public uint[] ColourData { get; }

    public int Width { get; }
    public int Height { get; }

    public static PixelColourData GetPixelColourDataFromTexture(Texture2D texture)
    {
        var width = texture.Width;
        var height = texture.Height;
        var data = new uint[width * height];

        texture.GetData(data);

        return new PixelColourData(width, height, data);
    }

    public PixelColourData(int width, int height, uint[] colourData)
    {
        Width = width;
        Height = height;
        ColourData = colourData;
    }

    public uint Get(int x, int y)
    {
        var i = Width * y + x;

        return ColourData[i];
    }

    public void Set(int x, int y, uint pixel)
    {
        var i = Width * y + x;

        ColourData[i] = pixel;
    }
}