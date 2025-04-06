namespace NeoLemmixSharp.Common.Util;

/// <summary>
/// Represents a wrapper over a portion of a 2D array.
/// </summary>
/// <typeparam name="T">The array type</typeparam>
public readonly struct ArrayWrapper2D<T>
{
    private readonly T[] _data;
    private readonly Size _arrayDimensions;
    private readonly Region _subRegion;

    public T[] Array => _data;
    public Size Size => _subRegion.Size;

    public ArrayWrapper2D(
        T[] data,
        Size dimensions)
    {
        if (data.Length != dimensions.Area())
            throw new ArgumentException("Invalid dimensions");

        _data = data;
        _arrayDimensions = dimensions;
        _subRegion = new Region(dimensions);
    }

    public ArrayWrapper2D(
        T[] data,
        Size arrayDimensions,
        Region region)
    {
        var spanWidth = arrayDimensions.W;
        var spanHeight = arrayDimensions.H;
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
        _arrayDimensions = arrayDimensions;
        _subRegion = region;
    }

    public bool EncompasesPoint(Point pos) => _subRegion.Size.EncompassesPoint(pos);

    public ref T this[Point pos]
    {
        get
        {
            _subRegion.Size.AssertEncompassesPoint(pos);
            var index = _arrayDimensions.GetIndexOfPoint(pos + _subRegion.Position);
            return ref _data[index];
        }
    }
}