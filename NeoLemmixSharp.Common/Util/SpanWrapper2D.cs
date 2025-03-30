namespace NeoLemmixSharp.Common.Util;

public readonly ref struct SpanWrapper2D<T>
{
    private readonly Span<T> _data;

    private readonly LevelSize _spanSize;

    private readonly LevelPosition _position;
    public readonly LevelSize Size;

    public SpanWrapper2D(
        Span<T> data,
        LevelSize spanSize,
        LevelPosition position,
        LevelSize size)
    {
        var spanWidth = spanSize.W;
        var spanHeight = spanSize.H;
        var x = position.X;
        var y = position.Y;
        var width = size.W;
        var height = size.H;

        if (spanWidth <= 0 || spanHeight <= 0 ||
            spanWidth * spanHeight != data.Length ||
            x < 0 || y < 0 ||
            width <= 0 || height <= 0 ||
            x + width > spanWidth ||
            y + height > spanHeight)
            throw new ArgumentException("Invalid dimensions");

        _data = data;
        _spanSize = spanSize;
        _position = position;
        Size = size;
    }

    public T this[LevelPosition pos]
    {
        get
        {
            Size.AssertEncompassesPoint(pos);
            var index = _spanSize.GetIndexOfPoint(pos + _position);
            return _data[index];
        }
        set
        {
            Size.AssertEncompassesPoint(pos);
            var index = _spanSize.GetIndexOfPoint(pos + _position);
            _data[index] = value;
        }
    }
}