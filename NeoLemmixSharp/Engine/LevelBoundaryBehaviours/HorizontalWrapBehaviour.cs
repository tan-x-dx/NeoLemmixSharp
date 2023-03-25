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

    public PixelData GetPixel(LevelPosition levelPosition)
    {
        if (levelPosition.Y < 0 || levelPosition.Y >= _height)
            return _voidPixel;

        var index = _width * levelPosition.Y + levelPosition.X;
        return _data[index];
    }

    public LevelPosition NormalisePosition(LevelPosition levelPosition)
    {
        if (levelPosition.X < 0)
        {
            levelPosition.X += _width;
        }
        else if (levelPosition.X >= _width)
        {
            levelPosition.X -= _width;
        }

        return levelPosition;
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