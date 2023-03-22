namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours;

public sealed class HorizontalWrapBehaviour : ILevelBoundaryBehaviour
{
    private readonly int _width;
    private readonly int _height;
    private readonly PixelData _voidPixel;
    private readonly PixelData[] _data;

    public HorizontalWrapBehaviour(int width, int height, PixelData voidPixel, PixelData[] data)
    {
        _width = width;
        _height = height;
        _voidPixel = voidPixel;
        _data = data;
    }

    public PixelData GetPixel(ref LevelPosition levelPosition)
    {
        if (levelPosition.Y < 0 || levelPosition.Y >= _height)
            return _voidPixel;

        var x = levelPosition.X;
        var y = levelPosition.Y;

        if (x < 0)
        {
            x += _width;
        }

        if (x >= _width)
        {
            x -= _width;
        }

        levelPosition = new LevelPosition(x, y);

        var index = _width * y + x;
        return _data[index];
    }

    public void ScrollViewPortHorizontally(LevelViewPort viewPort, int dx)
    {
        throw new System.NotImplementedException();
    }

    public void ScrollViewPortVertically(LevelViewPort viewPort, int dy)
    {
        throw new System.NotImplementedException();
    }
}