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
        var arrayWidth = arrayDimensions.W;
        var arrayHeight = arrayDimensions.H;
        var x = region.X;
        var y = region.Y;
        var width = region.W;
        var height = region.H;

        if (arrayWidth < 0 || arrayHeight < 0 ||
            arrayWidth * arrayHeight != data.Length ||
            x < 0 || y < 0 ||
            width < 0 || height < 0 ||
            x + width > arrayWidth ||
            y + height > arrayHeight)
            throw new ArgumentException("Invalid dimensions");

        _data = data;
        _arrayDimensions = arrayDimensions;
        _subRegion = region;
    }

    public ref T this[Point pos]
    {
        get
        {
            Size.AssertEncompassesPoint(pos);
            var index = _arrayDimensions.GetIndexOfPoint(pos + _subRegion.Position);
            return ref _data[index];
        }
    }
}