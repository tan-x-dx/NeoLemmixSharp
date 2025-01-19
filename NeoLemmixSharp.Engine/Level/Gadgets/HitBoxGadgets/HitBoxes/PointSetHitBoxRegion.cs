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

    private readonly LevelPosition _offset;
    private readonly LevelSize _minimumBoundingBoxDimensions;

    public LevelPosition Offset => _offset;
    public LevelSize BoundingBoxDimensions => _minimumBoundingBoxDimensions;

    public PointSetHitBoxRegion(ReadOnlySpan<LevelPosition> points)
    {
        if (points.Length == 0)
            throw new ArgumentException("Cannot create PointSetHitBoxRegion with zero points!");

        var minimumBoundingBox = new LevelRegion(points);
        _offset = minimumBoundingBox.P1;
        _minimumBoundingBoxDimensions = minimumBoundingBox.GetSize();

        var minimumBoundingBoxWidth = _minimumBoundingBoxDimensions.W;
        var minimumBoundingBoxHeight = _minimumBoundingBoxDimensions.H;

        if (minimumBoundingBoxWidth > DimensionCutoffSize || minimumBoundingBoxHeight > DimensionCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! W:{minimumBoundingBoxWidth}, H:{minimumBoundingBoxHeight}");

        var totalNumberOfPoints = minimumBoundingBoxWidth * minimumBoundingBoxHeight;

        if (totalNumberOfPoints > AreaCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! Area:{totalNumberOfPoints}");

        _levelPositionBits = BitArrayHelpers.CreateBitArray(totalNumberOfPoints, false);
        var span = new Span<uint>(_levelPositionBits);

        for (var i = 0; i < points.Length; i++)
        {
            var p = points[i] - _offset;

            var index = IndexFor(p);
            BitArrayHelpers.SetBit(span, index);
        }
    }

    [Pure]
    public bool ContainsPoint(LevelPosition levelPosition)
    {
        levelPosition -= _offset;
        var index = IndexFor(levelPosition);

        return (uint)levelPosition.X < (uint)_minimumBoundingBoxDimensions.W &&
               (uint)levelPosition.Y < (uint)_minimumBoundingBoxDimensions.H &&
               BitArrayHelpers.GetBit(new ReadOnlySpan<uint>(_levelPositionBits), index);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexFor(LevelPosition levelPosition) => _minimumBoundingBoxDimensions.W * levelPosition.Y + levelPosition.X;
}
