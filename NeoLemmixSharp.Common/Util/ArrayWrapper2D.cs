using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util;

/// <summary>
/// Represents a wrapper over a portion of a 2D array.
/// </summary>
/// <typeparam name="T">The array type</typeparam>
public readonly struct ArrayWrapper2D<T> : IEquatable<ArrayWrapper2D<T>>
{
    private readonly T[] _data;
    private readonly Size _arrayDimensions;
    private readonly RectangularRegion _subRegion;

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
        _subRegion = new RectangularRegion(dimensions);
    }

    public ArrayWrapper2D(
        T[] data,
        Size arrayDimensions,
        RectangularRegion region)
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

            return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_data), index);
        }
    }

    [Pure]
    [DebuggerStepThrough]
    public bool Equals(ArrayWrapper2D<T> other) => ReferenceEquals(_data, other._data) &&
                                                   _arrayDimensions == other._arrayDimensions &&
                                                   _subRegion == other._subRegion;
    [Pure]
    [DebuggerStepThrough]
    public override bool Equals(object? obj) => obj is ArrayWrapper2D<T> other && Equals(other);
    [Pure]
    [DebuggerStepThrough]
    public override int GetHashCode() => HashCode.Combine(_arrayDimensions, _subRegion);
    [Pure]
    [DebuggerStepThrough]
    public static bool operator ==(ArrayWrapper2D<T> left, ArrayWrapper2D<T> right) => left.Equals(right);
    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(ArrayWrapper2D<T> left, ArrayWrapper2D<T> right) => !left.Equals(right);

    public static void CopyTo(
        in ArrayWrapper2D<T> source,
        in ArrayWrapper2D<T> destination,
        DihedralTransformation dht)
    {
        var sourceSize = source.Size;

        if (dht.Transform(sourceSize) != destination._subRegion.Size)
            throw new ArgumentException("Cannot compare different regions!");

        var transformationData = dht.GetTransformationData(sourceSize);

        var w = sourceSize.W;
        var h = sourceSize.H;
        for (var y = 0; y < w; y++)
        {
            for (var x = 0; x < h; x++)
            {
                var p0 = new Point(x, y);
                var p1 = transformationData.Transform(p0);

                var sourceIndex = source._arrayDimensions.GetIndexOfPoint(p0 + source._subRegion.Position);
                var destinationIndex = destination._arrayDimensions.GetIndexOfPoint(p1 + destination._subRegion.Position);

                destination._data[destinationIndex] = source._data[sourceIndex];
            }
        }
    }
}
