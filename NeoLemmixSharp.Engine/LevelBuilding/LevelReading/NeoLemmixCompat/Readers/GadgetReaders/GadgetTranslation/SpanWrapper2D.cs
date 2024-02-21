namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;

public readonly ref struct SpanWrapper2D
{
    private readonly Span<uint> _data;

    private readonly int _spanWidth;

    private readonly int _x;
    private readonly int _y;

    public readonly int Width;
    public readonly int Height;

    public SpanWrapper2D(
        Span<uint> data,
        int spanWidth,
        int spanHeight,
        int x,
        int y,
        int width,
        int height)
    {
        if (spanWidth <= 0 || spanHeight <= 0 ||
            spanWidth * spanHeight != data.Length ||
            x < 0 || y < 0 ||
            width <= 0 || height <= 0 ||
            x + width > spanWidth ||
            y + height > spanHeight)
            throw new ArgumentException("Invalid dimensions");

        _data = data;
        _spanWidth = spanWidth;
        _x = x;
        _y = y;
        Width = width;
        Height = height;
    }

    public uint this[int x0, int y0]
    {
        get
        {
            var index = GetIndex(x0, y0);

            return _data[index];
        }
        set
        {
            var index = GetIndex(x0, y0);

            _data[index] = value;
        }
    }

    private int GetIndex(int x0, int y0)
    {
        if (x0 < 0 ||
            x0 >= Width)
            throw new ArgumentOutOfRangeException(nameof(x0), x0, "Invalid X");
        if (y0 < 0 ||
            y0 >= Height)
            throw new ArgumentOutOfRangeException(nameof(y0), y0, "Invalid Y");

        return _spanWidth * (_y + y0) + _x + x0;
    }
}