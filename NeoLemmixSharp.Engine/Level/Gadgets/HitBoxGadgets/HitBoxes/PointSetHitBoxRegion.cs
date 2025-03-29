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

    private readonly LevelRegion _bounds;

    public LevelRegion CurrentBounds => _bounds;

    public PointSetHitBoxRegion(ReadOnlySpan<LevelPosition> points)
    {
        if (points.Length == 0)
            throw new ArgumentException("Cannot create PointSetHitBoxRegion with zero points!");

        _bounds = new LevelRegion(points);

        if (_bounds.W > DimensionCutoffSize || _bounds.H > DimensionCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! W:{_bounds.W}, H:{_bounds.H}");

        var totalNumberOfPoints = _bounds.S.Area();

        if (totalNumberOfPoints > AreaCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! Area:{totalNumberOfPoints}");

        _levelPositionBits = BitArrayHelpers.CreateBitArray(totalNumberOfPoints, false);
        var span = new Span<uint>(_levelPositionBits);

        for (var i = 0; i < points.Length; i++)
        {
            var p = points[i] - _bounds.P;

            var index = IndexFor(p);
            BitArrayHelpers.SetBit(span, index);
        }
    }

    [Pure]
    public bool ContainsPoint(LevelPosition levelPosition)
    {
        levelPosition -= _bounds.P;
        var index = IndexFor(levelPosition);

        return (uint)levelPosition.X < (uint)_bounds.W &&
               (uint)levelPosition.Y < (uint)_bounds.H &&
               BitArrayHelpers.GetBit(new ReadOnlySpan<uint>(_levelPositionBits), index);
    }

    [Pure]
    public bool ContainsPoints(LevelPosition p1, LevelPosition p2)
    {
        p1 -= _bounds.P;
        p2 -= _bounds.P;
        var index1 = IndexFor(p1);
        var index2 = IndexFor(p2);
        var span = new ReadOnlySpan<uint>(_levelPositionBits);

        var boundsUw = (uint)_bounds.W;
        var boundsUh = (uint)_bounds.H;

        return ((uint)p1.X < boundsUw &&
                (uint)p1.Y < boundsUh &&
                BitArrayHelpers.GetBit(span, index1)) ||
               ((uint)p2.X < boundsUw &&
                (uint)p2.Y < boundsUh &&
                BitArrayHelpers.GetBit(span, index2));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexFor(LevelPosition levelPosition) => _bounds.S.GetIndexOfPoint(levelPosition);
}
