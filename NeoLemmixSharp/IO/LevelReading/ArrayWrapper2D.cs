using System;

namespace NeoLemmixSharp.IO.LevelReading;

public sealed class ArrayWrapper2D
{
    public int Width { get; }
    public int Height { get; }

    private readonly int[] _data;

    public ArrayWrapper2D(int width, int height, int[] data)
    {
        Width = width;
        Height = height;

        if (Width * Height != data.Length)
            throw new InvalidOperationException("Invalid dimensions");

        _data = data;
    }

    public int Get(int x, int y)
    {
        var index = Width * y + x;

        return _data[index];
    }

    public void Set(int x, int y, int value)
    {
        var index = Width * y + x;

        _data[index] = value;
    }
}