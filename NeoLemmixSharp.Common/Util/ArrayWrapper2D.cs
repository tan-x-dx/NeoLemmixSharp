namespace NeoLemmixSharp.Common.Util;

/// <summary>
/// Represents a wrapper over a portion of a 2D array.
/// </summary>
/// <typeparam name="T">The array type</typeparam>
public readonly struct ArrayWrapper2D<T>
{
    private readonly T[] _data;
    private readonly LevelSize _spanSize;
    private readonly LevelRegion _region;

    public T[] Array => _data;

    public ArrayWrapper2D(
        T[] data,
        LevelSize dimensions)
    {
        if (data.Length != dimensions.Area())
            throw new ArgumentException("Invalid dimensions");

        _data = data;
        _spanSize = dimensions;
        _region = new LevelRegion(dimensions);
    }

    public ArrayWrapper2D(
        T[] data,
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

    public bool EncompasesPoint(LevelPosition pos) => _region.Size.EncompassesPoint(pos);

    public ref T this[LevelPosition pos]
    {
        get
        {
            _region.Size.AssertEncompassesPoint(pos);
            var index = _spanSize.GetIndexOfPoint(pos + _region.Position);
            return ref _data[index];
        }
    }
}