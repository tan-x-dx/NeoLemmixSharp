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

    private readonly int _minimumBoundingBoxWidth;
    private readonly int _minimumBoundingBoxHeight;
    public LevelSize BoundingBoxDimensions => new(_minimumBoundingBoxWidth, _minimumBoundingBoxHeight);

    public PointSetHitBoxRegion(ReadOnlySpan<LevelPosition> points)
    {
        if (points.Length == 0)
            throw new ArgumentException("Cannot create PointSetHitBoxRegion with zero points!");

        var minimumBoundingBox = new LevelRegion(points);
        var minimumBoundingBoxSize = minimumBoundingBox.GetSize();

        _minimumBoundingBoxWidth = minimumBoundingBoxSize.W;
        _minimumBoundingBoxHeight = minimumBoundingBoxSize.H;

        if (_minimumBoundingBoxWidth > DimensionCutoffSize || _minimumBoundingBoxHeight > DimensionCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! W:{_minimumBoundingBoxWidth}, H:{_minimumBoundingBoxHeight}");

        var totalNumberOfPoints = _minimumBoundingBoxWidth * _minimumBoundingBoxHeight;

        if (totalNumberOfPoints > AreaCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! Area:{totalNumberOfPoints}");

        _levelPositionBits = BitArrayHelpers.CreateBitArray(totalNumberOfPoints, false);
        var span = new Span<uint>(_levelPositionBits);

        foreach (var levelPosition in points)
        {
            var x = levelPosition.X - minimumBoundingBox.P1X;
            var y = levelPosition.Y - minimumBoundingBox.P1Y;

            var index = IndexFor(x, y);
            BitArrayHelpers.SetBit(span, index);
        }
    }

    [Pure]
    public bool ContainsPoint(LevelPosition levelPosition)
    {
        var index = IndexFor(levelPosition.X, levelPosition.Y);

        return (uint)levelPosition.X < (uint)_minimumBoundingBoxWidth &&
               (uint)levelPosition.Y < (uint)_minimumBoundingBoxHeight &&
               BitArrayHelpers.GetBit(new ReadOnlySpan<uint>(_levelPositionBits), index);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexFor(int x, int y) => _minimumBoundingBoxWidth * y + x;
}
