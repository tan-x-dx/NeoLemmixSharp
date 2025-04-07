using System.Diagnostics;

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
        Debug.Assert(dimensions.W >= 0);
        Debug.Assert(dimensions.H >= 0);

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
        Debug.Assert(arrayDimensions.W >= 0);
        Debug.Assert(arrayDimensions.H >= 0);

        Debug.Assert(region.W >= 1);
        Debug.Assert(region.H >= 1);

        var arrayWidth = arrayDimensions.W;
        var arrayHeight = arrayDimensions.H;
        var subRegionX = region.X;
        var subregionY = region.Y;
        var subRegionWidth = region.W;
        var subRegionHeight = region.H;

        if (arrayWidth * arrayHeight == data.Length &&
            subRegionX >= 0 && subregionY >= 0 &&
            subRegionX + subRegionWidth <= arrayWidth &&
            subregionY + subRegionHeight <= arrayHeight)
        {
            _data = data;
            _arrayDimensions = arrayDimensions;
            _subRegion = region;
            return;
        }

        throw new ArgumentException("Invalid dimensions");
    }

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