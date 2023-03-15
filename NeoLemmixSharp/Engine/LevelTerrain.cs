namespace NeoLemmixSharp.Engine;

public sealed class LevelTerrain
{
    public int Width { get; }
    public int Height { get; }

    public LevelTerrain(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public PixelData GetPixelData(int x, int y)
    {
        return null;
    }

}