using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("{_dimensionType}:{_boundaryBehaviourType} [{_levelLength}]")]
public unsafe sealed class BoundaryBehaviour : IDisposable
{
    private const int MaxNumberOfViewportRenderIntervals = 2;
    private const int MaxNumberOfRenderCopiesForWrappedLevels = 6;

    private readonly RawArray _byteBuffer;

    private readonly DimensionType _dimensionType;
    private readonly BoundaryBehaviourType _boundaryBehaviourType;
    private readonly int _levelLength;

    private readonly ViewPortRenderInterval* _viewPortRenderIntervalPointer;
    private readonly ScreenRenderInterval* _screenRenderIntervalPointer;
    private int _viewPortSpanLength;
    private int _screenSpanLength;

    private int _scaleMultiplier;

    private int _viewPortStart;
    private int _viewPortLength;

    private int _screenStart;
    private int _screenLength;

    private int _mouseViewPortCoordinate;
    private int _mouseScreenCoordinate;

    public BoundaryBehaviourType BoundaryBehaviourType => _boundaryBehaviourType;

    /// <summary>
    /// The level dimension in pixels
    /// </summary>
    public int LevelLength => _levelLength;

    /// <summary>
    /// The starting pixel of the view port
    /// </summary>
    public int ViewPortStart => _viewPortStart;

    /// <summary>
    /// The size in pixels of the view port. Note: ViewPortLength is derived from the ScreenLength and the scale multiplier
    /// </summary>
    public int ViewPortLength => _viewPortLength;

    /// <summary>
    /// The starting pixel where this should be rendered on screen
    /// </summary>
    public int ScreenStart => _screenStart;

    /// <summary>
    /// How many pixels are available on screen for rendering purposes
    /// </summary>
    public int ScreenLength => _screenLength;

    /// <summary>
    /// The coordinate of the mouse within the view port
    /// </summary>
    public int MouseViewPortCoordinate => _mouseViewPortCoordinate;

    /// <summary>
    /// The coordinate of the mouse within the screen
    /// </summary>
    public int MouseScreenCoordinate => _mouseScreenCoordinate;

    [Pure]
    public ReadOnlySpan<ViewPortRenderInterval> GetRenderViewPortIntervals() => new(_viewPortRenderIntervalPointer, _viewPortSpanLength);
    [Pure]
    public ReadOnlySpan<ScreenRenderInterval> GetScreenRenderIntervals() => new(_screenRenderIntervalPointer, _screenSpanLength);

    public BoundaryBehaviour(
        DimensionType dimensionType,
        BoundaryBehaviourType boundaryBehaviourType,
        int levelLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(levelLength);

        _dimensionType = dimensionType;
        _boundaryBehaviourType = boundaryBehaviourType;
        _levelLength = levelLength;

        var sizeOfViewPortRenderIntervals = sizeof(ViewPortRenderInterval) * MaxNumberOfViewportRenderIntervals;
        var sizeOfScreenRenderIntervals = sizeof(ScreenRenderInterval) * MaxNumberOfRenderCopiesForWrappedLevels;

        _byteBuffer = Helpers.AllocateBuffer<byte>(sizeOfViewPortRenderIntervals + sizeOfScreenRenderIntervals);
        byte* pointer = (byte*)_byteBuffer.Handle;

        _viewPortRenderIntervalPointer = (ViewPortRenderInterval*)pointer;
        _screenRenderIntervalPointer = (ScreenRenderInterval*)(pointer + sizeOfViewPortRenderIntervals);
    }

    /// <summary>
    /// <para>
    /// In the case of the Void type, this method returns the input unchanged.
    /// </para>
    /// <para>
    /// In the case of the Wrap type, this method returns the input
    /// modulo the level length.
    /// </para>
    /// </summary>
    /// <param name="n">The input point.</param>
    /// <returns>The input unchanged, in the case of the Void type, or the input
    /// modulo the level length, in the case of the Wrap type.</returns>
    [Pure]
    public int Normalise(int n)
    {
        var a = n;
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return a;

        // Most likely situation for Wrap normalisation is the input
        // being just outside the bounds [0, _levelLength - 1].
        // Therefore, we can avoid a call to the modulo operator
        // by simply adding/subtracting the level length

        var levelLength = _levelLength;
        if (a < 0)
        {
            do
            {
                a += levelLength;
            }
            while (a < 0);
        }
        else
        {
            while (a >= levelLength)
            {
                a -= levelLength;
            }
        }

        return a;
    }

    [Pure]
    public int GetNormalisedDelta(int a1, int a2)
    {
        var result = a2 - a1;
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return result;

        var levelLength = _levelLength;
        var halfLevelLength = levelLength >>> 1;
        result += halfLevelLength;

        // Most likely situation for Wrap normalisation is the input
        // being just outside the bounds [0, _levelLength - 1].
        // Therefore, we can avoid a call to the modulo operator
        // by simply adding/subtracting the level length

        if (result < 0)
        {
            do
            {
                result += levelLength;
            }
            while (result < 0);
        }
        else
        {
            while (result >= levelLength)
            {
                result -= levelLength;
            }
        }
        result -= halfLevelLength;
        return result;
    }

    [Pure]
    public bool IntervalContainsPoint(Interval interval, int n)
    {
        var intervalStart = Normalise(interval.Start);
        var intervalEnd = intervalStart + interval.Length;
        var a = Normalise(n);

        // If the interval actually contains the point, that's easy
        if (intervalStart <= a && a < intervalEnd)
            return true;

        // If the interval does not contains the point, then the Void type will never work
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return false;

        // Edge case for Wrap behaviour (literally)
        // Suppose the test point is just above zero
        // And suppose the interval is just below _levelLength
        // The interval could "wrap around" to overlap with the point
        // We check this case here

        // By this point, for the case we need to check we definitely have:
        // 0 <= a < intervalStart < _levelLength <= a + _levelLength
        // Therefore can safely eliminate one interval check

        return a + _levelLength < intervalEnd;
    }

    [Pure]
    public bool IntervalsOverlap(Interval i1, Interval i2)
    {
        var interval1Start = Normalise(i1.Start);
        var interval1End = interval1Start + i1.Length;

        var interval2Start = Normalise(i2.Start);
        var interval2End = interval2Start + i2.Length;

        // If the intervals actually overlap, that's easy
        if (interval1Start < interval2End &&
            interval2Start < interval1End)
            return true;

        // If the intervals do not overlap, then the Void type will never work
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return false;

        // Edge case for Wrap behaviour (literally)
        // Suppose one interval is just above zero
        // And suppose the other interval is just below _levelLength
        // The second interval could "wrap around" to overlap with the first
        // We check this case here

        // By this point, for the case we need to check we definitely have:
        // 0 <= leftStart < leftEnd < rightStart < _levelLength <= leftStart + _levelLength < leftEnd + _levelLength
        // Therefore can safely eliminate one interval check

        if (interval1Start < interval2Start)
        {
            // interval1 <=> leftInterval
            // interval2 <=> rightInterval

            return interval1Start + _levelLength < interval2End;
        }
        else
        {
            // interval1 <=> rightInterval
            // interval2 <=> leftInterval

            return interval2Start + _levelLength < interval1End;
        }
    }

    public void UpdateMouseCoordinate(int windowCoordinate)
    {
        _mouseViewPortCoordinate = (windowCoordinate + _scaleMultiplier - 1) / _scaleMultiplier;
        _mouseScreenCoordinate = _mouseViewPortCoordinate * _scaleMultiplier;

        _mouseViewPortCoordinate = Normalise(_mouseViewPortCoordinate + _viewPortStart - (_screenStart / _scaleMultiplier));
    }

    public void UpdateScreenDimension(
        int screenDimension,
        int scaleMultiplier)
    {
        _scaleMultiplier = scaleMultiplier;
        _viewPortLength = (screenDimension + _scaleMultiplier - 1) / _scaleMultiplier;
        _screenLength = _viewPortLength * _scaleMultiplier;

        if (_levelLength < _viewPortLength)
        {
            _viewPortStart = 0;
            _viewPortLength = _levelLength;
        }
        else
        {
            UpdateViewPortCoordinate(_viewPortStart);
        }

        UpdateViewPortRenderIntervals();
        UpdateScreenRenderIntervals();
    }

    private void UpdateViewPortCoordinate(int viewPortCoordinate)
    {
        if (_levelLength < _viewPortLength)
        {
            _viewPortStart = 0;
            return;
        }

        _viewPortStart = _boundaryBehaviourType == BoundaryBehaviourType.Void
            ? Math.Clamp(viewPortCoordinate, 0, _levelLength - _viewPortLength)
            : Normalise(viewPortCoordinate);
    }

    public void Scroll(int delta)
    {
        UpdateViewPortCoordinate(_viewPortStart + delta);

        UpdateViewPortRenderIntervals();
        UpdateScreenRenderIntervals();
    }

    private void UpdateViewPortRenderIntervals()
    {
        ViewPortRenderInterval* viewPortRenderIntervalPointer = _viewPortRenderIntervalPointer;

        if (_boundaryBehaviourType == BoundaryBehaviourType.Void ||
            _viewPortStart + _viewPortLength < _levelLength)
        {
            *viewPortRenderIntervalPointer = new ViewPortRenderInterval(_viewPortStart, _viewPortLength, -_viewPortStart);
            _viewPortSpanLength = 1;
            return;
        }

        _viewPortSpanLength = 2;
        int l1;
        if (_viewPortLength < _levelLength)
        {
            l1 = _levelLength - _viewPortStart;
            *viewPortRenderIntervalPointer = new ViewPortRenderInterval(_viewPortStart, l1, -_viewPortStart);
            viewPortRenderIntervalPointer++;
            *viewPortRenderIntervalPointer = new ViewPortRenderInterval(0, _viewPortLength - l1, _levelLength - _viewPortStart);
        }
        else
        {
            l1 = _levelLength >>> 1;
            *viewPortRenderIntervalPointer = new ViewPortRenderInterval(0, l1, 0);
            viewPortRenderIntervalPointer++;
            *viewPortRenderIntervalPointer = new ViewPortRenderInterval(l1, _levelLength - l1, 0);
        }
    }

    private void UpdateScreenRenderIntervals()
    {
        _screenSpanLength = 1;
        ScreenRenderInterval* screenRenderIntervalPointer = _screenRenderIntervalPointer;

        if (_boundaryBehaviourType == BoundaryBehaviourType.Void ||
            _viewPortStart + _viewPortLength < _levelLength)
        {
            var viewPortDimensionOnScreen = _viewPortLength * _scaleMultiplier;
            _screenStart = (_screenLength - viewPortDimensionOnScreen) >>> 1;

            *screenRenderIntervalPointer = new ScreenRenderInterval(0, _viewPortLength, _screenStart, viewPortDimensionOnScreen);

            return;
        }

        _screenStart = 0;

        if (_viewPortLength < _levelLength)
        {
            *screenRenderIntervalPointer = new ScreenRenderInterval(0, _viewPortLength, 0, _screenLength);

            return;
        }

        UpdateScreenRenderIntervalsForMultipleWrappedCopies();
    }

    private void UpdateScreenRenderIntervalsForMultipleWrappedCopies()
    {
        var deltaL = _levelLength * _scaleMultiplier;
        var maxScreenDimension = Math.Min(deltaL * MaxNumberOfRenderCopiesForWrappedLevels, _screenLength);
        var screenCoordinate = Math.Max(0, _screenLength - maxScreenDimension);
        screenCoordinate >>>= 1;

        ScreenRenderInterval* screenRenderIntervalPointer = _screenRenderIntervalPointer;

        *screenRenderIntervalPointer = new ScreenRenderInterval(
            _viewPortStart,
            _levelLength - _viewPortStart,
            screenCoordinate,
            (_levelLength - _viewPortStart) * _scaleMultiplier);

        screenCoordinate += (_levelLength - _viewPortStart) * _scaleMultiplier;

        while (screenCoordinate < maxScreenDimension &&
               _screenSpanLength < MaxNumberOfRenderCopiesForWrappedLevels - 1)
        {
            screenRenderIntervalPointer++;
            _screenSpanLength++;
            *screenRenderIntervalPointer = new ScreenRenderInterval(0, _levelLength, screenCoordinate, deltaL);
            screenCoordinate += deltaL;
        }

        if (screenCoordinate < maxScreenDimension)
        {
            screenRenderIntervalPointer++;
            _screenSpanLength++;
            *screenRenderIntervalPointer = new ScreenRenderInterval(0, 0, screenCoordinate, 0);
        }
    }

    [Pure]
    public ClipInterval GetIntersection(ClipInterval spriteClipInterval, ClipInterval viewportClipInterval)
    {
        var offset = _boundaryBehaviourType == BoundaryBehaviourType.Void
            ? 0
            : GetIntersectionAcrossBoundary(spriteClipInterval, ref viewportClipInterval);

        if (viewportClipInterval.Start < spriteClipInterval.Start + spriteClipInterval.Length &&
            spriteClipInterval.Start < viewportClipInterval.Start + viewportClipInterval.Length)
        {
            var num1 = Math.Min(
                spriteClipInterval.Start + spriteClipInterval.Length,
                viewportClipInterval.Start + viewportClipInterval.Length);

            var start = Math.Max(
                spriteClipInterval.Start,
                viewportClipInterval.Start);

            return new ClipInterval(start, num1 - start, offset);
        }

        return new ClipInterval();
    }

    private int GetIntersectionAcrossBoundary(
        ClipInterval spriteClipInterval,
        ref ClipInterval viewportClipInterval)
    {
        if (spriteClipInterval.Start < 0 &&
            spriteClipInterval.Start + spriteClipInterval.Length >= 0)
        {
            if (viewportClipInterval.Start == 0)
                return 0;

            viewportClipInterval = new ClipInterval(
                viewportClipInterval.Start - _levelLength,
                viewportClipInterval.Length,
                0);
            return _levelLength;
        }

        if (spriteClipInterval.Start + spriteClipInterval.Length <= _levelLength ||
            viewportClipInterval.Start != 0)
            return 0;

        viewportClipInterval = new ClipInterval(
            viewportClipInterval.Start + _levelLength,
            viewportClipInterval.Length,
            0);
        return -_levelLength;
    }

    public void Dispose()
    {
        _byteBuffer.Dispose();
    }
}
