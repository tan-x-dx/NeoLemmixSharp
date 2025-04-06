namespace NeoLemmixSharp.Common.Util;

public readonly ref struct SpanWrapper2D<T>
{
    private readonly Span<T> _data;
    private readonly LevelSize _spanSize;
    private readonly LevelRegion _region;

    public int Width => _region.W;
    public int Height => _region.H;

    public SpanWrapper2D(
        Span<T> data,
        LevelSize spanSize,
        LevelRegion region)
    {
        var spanWidth = spanSize.W;
        var spanHeight = spanSize.H;
        var x = region.X;
        var y = region.Y;
        var width = region.W;
        var height = region.H;

        if (spanWidth <= 0 || spanHeight <= 0 ||
            spanWidth * spanHeight != data.Length ||
            x < 0 || y < 0 ||
            width <= 0 || height <= 0 ||
            x + width > spanWidth ||
            y + height > spanHeight)
            throw new ArgumentException("Invalid dimensions");

        _data = data;
        _spanSize = spanSize;
        _region = region;
    }

    public T this[LevelPosition pos]
    {
        get
        {
            _region.Size.AssertEncompassesPoint(pos);
            var index = _spanSize.GetIndexOfPoint(pos + _region.Position);
            return _data[index];
        }
        set
        {
            _region.Size.AssertEncompassesPoint(pos);
            var index = _spanSize.GetIndexOfPoint(pos + _region.Position);
            _data[index] = value;
        }
    }
}