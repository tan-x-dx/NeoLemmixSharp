using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class PointSetHitBoxRegion : IHitBoxRegion
{
    private const int DimensionCutoffSize = 64;
    private const int AreaCutoffSize = 64 * 64;

    private readonly uint[] _levelPositionBits;

    private readonly RectangularRegion _bounds;

    public RectangularRegion CurrentBounds => _bounds;

    public PointSetHitBoxRegion(ReadOnlySpan<Point> points)
    {
        if (points.Length == 0)
            throw new ArgumentException("Cannot create PointSetHitBoxRegion with zero points!");

        _bounds = new RectangularRegion(points);

        if (_bounds.W > DimensionCutoffSize || _bounds.H > DimensionCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! W:{_bounds.W}, H:{_bounds.H}");

        var totalNumberOfPoints = _bounds.Size.Area();

        if (totalNumberOfPoints > AreaCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! Area:{totalNumberOfPoints}");

        _levelPositionBits = BitArrayHelpers.CreateBitArray(totalNumberOfPoints, false);
        var span = new Span<uint>(_levelPositionBits);

        for (var i = 0; i < points.Length; i++)
        {
            var p = points[i] - _bounds.Position;

            var index = IndexFor(p);
            BitArrayHelpers.SetBit(span, index);
        }
    }

    [Pure]
    public bool ContainsPoint(Point levelPosition)
    {
        levelPosition -= _bounds.Position;
        var index = IndexFor(levelPosition);

        return _bounds.Size.EncompassesPoint(levelPosition) &&
               BitArrayHelpers.GetBit(new ReadOnlySpan<uint>(_levelPositionBits), index);
    }

    [Pure]
    public bool ContainsEitherPoint(Point p1, Point p2)
    {
        p1 -= _bounds.Position;
        p2 -= _bounds.Position;
        var index1 = IndexFor(p1);
        var index2 = IndexFor(p2);
        var span = new ReadOnlySpan<uint>(_levelPositionBits);

        return (_bounds.Size.EncompassesPoint(p1) &&
                BitArrayHelpers.GetBit(span, index1)) ||
               (_bounds.Size.EncompassesPoint(p2) &&
                BitArrayHelpers.GetBit(span, index2));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexFor(Point levelPosition) => _bounds.Size.GetIndexOfPoint(levelPosition);
}
