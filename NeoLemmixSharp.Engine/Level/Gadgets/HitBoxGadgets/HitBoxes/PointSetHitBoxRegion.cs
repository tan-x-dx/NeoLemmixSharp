using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class PointSetHitBoxRegion : IHitBoxRegion
{
    private const int DimensionCutoffSize = 128;
    private const int AreaCutoffSize = 128 * 128;

    private readonly uint[] _levelPositionBits;

    private readonly int _minimumBoundingBoxWidth;
    private readonly int _minimumBoundingBoxHeight;

    private LevelPosition _currentOffset;
    private LevelPosition _previousOffset;

    public PointSetHitBoxRegion(ReadOnlySpan<LevelPosition> points)
    {
        if (points.Length == 0)
            throw new ArgumentException("Cannot create PointSetHitBoxRegion with zero points!");

        var minimumBoundingBox = new LevelRegion(points);

        _minimumBoundingBoxWidth = 1 + minimumBoundingBox.P2X - minimumBoundingBox.P1X;
        _minimumBoundingBoxHeight = 1 + minimumBoundingBox.P2Y - minimumBoundingBox.P1Y;

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

        _currentOffset = LevelScreen.NormalisePosition(minimumBoundingBox.GetTopLeftPosition());
        _previousOffset = _currentOffset;
    }

    [Pure]
    public bool ContainsPoint(LevelPosition levelPosition)
    {
        levelPosition -= _currentOffset;
        var index = IndexFor(levelPosition.X, levelPosition.Y);

        return (uint)levelPosition.X < (uint)_minimumBoundingBoxWidth &&
               (uint)levelPosition.Y < (uint)_minimumBoundingBoxHeight &&
               BitArrayHelpers.GetBit(new ReadOnlySpan<uint>(_levelPositionBits), index);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexFor(int x, int y) => _minimumBoundingBoxWidth * y + x;

    public void Move(int dx, int dy)
    {
        _previousOffset = _currentOffset;
        _currentOffset = LevelScreen.NormalisePosition(_currentOffset + new LevelPosition(dx, dy));
    }

    public void SetPosition(int x, int y)
    {
        _previousOffset = _currentOffset;
        _currentOffset = LevelScreen.NormalisePosition(new LevelPosition(x, y));
    }

    public LevelPosition TopLeftPixel => _currentOffset;
    public LevelPosition BottomRightPixel => new(_currentOffset.X + _minimumBoundingBoxWidth, _currentOffset.Y + _minimumBoundingBoxHeight);
    public LevelPosition PreviousTopLeftPixel => _previousOffset;
    public LevelPosition PreviousBottomRightPixel => new(_previousOffset.X + _minimumBoundingBoxWidth, _previousOffset.Y + _minimumBoundingBoxHeight);
}
