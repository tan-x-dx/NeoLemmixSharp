using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class PointSetHitBoxRegion : HitBoxRegion
{
    private const int DimensionCutoffSize = 64;

    private readonly uint[] _levelPositionBits;

    private readonly RectangularRegion _bounds;

    public override RectangularRegion CurrentBounds => _bounds;

    public PointSetHitBoxRegion(ReadOnlySpan<Point> points)
    {
        if (points.Length == 0)
            throw new ArgumentException("Cannot create PointSetHitBoxRegion with zero points!");

        _bounds = new RectangularRegion(points);

        if (_bounds.W > DimensionCutoffSize || _bounds.H > DimensionCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! W:{_bounds.W}, H:{_bounds.H}");

        var totalNumberOfPoints = _bounds.Size.Area();

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
    public override bool ContainsPoint(Point levelPosition)
    {
        var p = levelPosition - _bounds.Position;
        if (!_bounds.Size.EncompassesPoint(p))
            return false;

        var pointIndex = IndexFor(p);

        return BitArrayHelpers.GetBit(new ReadOnlySpan<uint>(_levelPositionBits), pointIndex);
    }

    [Pure]
    public override bool ContainsEitherPoint(Point p1, Point p2)
    {
        int pointIndex;
        Point p;
        var span = new ReadOnlySpan<uint>(_levelPositionBits);

        p = p1 - _bounds.Position;
        if (_bounds.Size.EncompassesPoint(p))
        {
            pointIndex = IndexFor(p);
            if (BitArrayHelpers.GetBit(span, pointIndex))
                return true;
        }

        p = p2 - _bounds.Position;
        if (!_bounds.Size.EncompassesPoint(p))
            return false;

        pointIndex = IndexFor(p);
        return BitArrayHelpers.GetBit(span, pointIndex);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexFor(Point levelPosition) => _bounds.Size.GetIndexOfPoint(levelPosition);
}
