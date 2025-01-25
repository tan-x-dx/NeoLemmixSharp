using NeoLemmixSharp.Common.Util;
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

        var minimumBoundingBoxWidth = _bounds.W;
        var minimumBoundingBoxHeight = _bounds.H;

        if (minimumBoundingBoxWidth > DimensionCutoffSize || minimumBoundingBoxHeight > DimensionCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! W:{minimumBoundingBoxWidth}, H:{minimumBoundingBoxHeight}");

        var totalNumberOfPoints = minimumBoundingBoxWidth * minimumBoundingBoxHeight;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexFor(LevelPosition levelPosition) => _bounds.W * levelPosition.Y + levelPosition.X;
}
