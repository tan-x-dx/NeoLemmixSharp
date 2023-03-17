namespace NeoLemmixSharp.LevelBuilding.Painting;

public sealed class PixelColourData
{
    private readonly uint[] _terrainData;

    public int Width { get; }
    public int Height { get; }
    public bool IsSteel { get; }

    public PixelColourData(int width, int height, uint[] terrainData, bool isSteel)
    {
        Width = width;
        Height = height;
        _terrainData = terrainData;
        IsSteel = isSteel;
    }

    public uint Get(int x, int y)
    {
        var i = Width * y + x;

        return _terrainData[i];
    }

    public void Set(int x, int y, uint pixel)
    {
        var i = Width * y + x;

        _terrainData[i] = pixel;
    }
}