namespace NeoLemmixSharp.Common.Util;

public sealed class ArrayWrapper2D<T>
{
    public int Width { get; }
    public int Height { get; }

    private readonly IList<T> _data;

    public ArrayWrapper2D(int width, int height, IList<T> data)
    {
        Width = width;
        Height = height;

        if (Width * Height != data.Count)
            throw new InvalidOperationException("Invalid dimensions");

        _data = data;
    }

    public T Get(int x, int y)
    {
        var index = Width * y + x;

        return _data[index];
    }

    public void Set(int x, int y, T value)
    {
        var index = Width * y + x;

        _data[index] = value;
    }
}