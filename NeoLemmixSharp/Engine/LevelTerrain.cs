namespace NeoLemmixSharp.Engine;

public sealed class LevelTerrain
{
    public int Width { get; }
    public int Height { get; }

    private readonly PixelData[] _data;

    public LevelTerrain(int width, int height)
    {
        Width = width;
        Height = height;

        _data = new PixelData[Width * Height];

        for (var i = 0; i < _data.Length; i++)
        {
            _data[i] = new PixelData();
        }
    }

    public PixelData GetPixelData(LevelPosition pos)
    {
        var index = Width * pos.Y + pos.X;
        return _data[index];
    }

    public PixelData GetPixelData(int x, int y)
    {
        var index = Width * y + x;
        return _data[index];
    }
}