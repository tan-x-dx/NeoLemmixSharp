using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("{DimensionType}:{BoundaryBehaviourType} [{_levelLength}]")]
public sealed class BoundaryBehaviour : IDisposable
{
    private const int MaxNumberOfViewportRenderIntervals = 2;
    private const int MaxNumberOfRenderCopiesForWrappedLevels = 6;

    private readonly RawArray _viewPortRenderIntervalBuffer;
    private readonly RawArray _screenRenderIntervalBuffer;

    private readonly DimensionType _dimensionType;
    private readonly BoundaryBehaviourType _boundaryBehaviourType;
    private readonly int _levelLength;

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
    public unsafe ReadOnlySpan<ViewPortRenderInterval> GetRenderViewPortIntervals() => new((void*)_viewPortRenderIntervalBuffer.Handle, _viewPortSpanLength);
    [Pure]
    public unsafe ReadOnlySpan<ScreenRenderInterval> GetScreenRenderIntervals() => new((void*)_screenRenderIntervalBuffer.Handle, _screenSpanLength);

    public BoundaryBehaviour(
        DimensionType dimensionType,
        BoundaryBehaviourType boundaryBehaviourType,
        int levelLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(levelLength);

        _dimensionType = dimensionType;
        _boundaryBehaviourType = boundaryBehaviourType;
        _levelLength = levelLength;

        _viewPortRenderIntervalBuffer = Helpers.AllocateBuffer<ViewPortRenderInterval>(MaxNumberOfViewportRenderIntervals);
        _screenRenderIntervalBuffer = Helpers.AllocateBuffer<ScreenRenderInterval>(MaxNumberOfRenderCopiesForWrappedLevels);
    }

    public void UpdateMouseCoordinate(int windowCoordinate)
    {
        _mouseViewPortCoordinate = (windowCoordinate + _scaleMultiplier - 1) / _scaleMultiplier;
        _mouseScreenCoordinate = _mouseViewPortCoordinate * _scaleMultiplier;

        _mouseViewPortCoordinate =
            Normalise(_mouseViewPortCoordinate + _viewPortStart - (_screenStart / _scaleMultiplier));
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

    [Pure]
    public int Normalise(int a)
    {
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return a;

        // Most likely situation for wrap normalisation is the input
        // being just outside the bounds [0, _levelLength - 1].
        // Therefore, we can avoid a call to the modulo operator
        // by simply adding/subtracting the level length

        if (a < 0)
        {
            do
            {
                a += _levelLength;
            }
            while (a < 0);
        }
        else
        {
            while (a >= _levelLength)
            {
                a -= _levelLength;
            }
        }

        return a;
    }

    [Pure]
    public bool IntervalContainsPoint(Interval interval, int a)
    {
        var intervalStart = Normalise(interval.Start);
        var intervalEnd = intervalStart + interval.Length;
        a = Normalise(a);

        // If it works, it always works
        if (intervalStart <= a && a < intervalEnd)
            return true;

        // If it doesn't work, it certainly doesn't work for void
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return false;

        // Edge case for wrap behaviour (literally)
        // Suppose the test point is just to the right of zero
        // And suppose the interval is just to the left of _levelLength
        // The interval could "wrap around" to overlap with the point
        // We check this case here

        a += _levelLength;
        return intervalStart <= a && a < intervalEnd;
    }

    [Pure]
    public bool IntervalsOverlap(Interval i1, Interval i2)
    {
        var interval1Start = Normalise(i1.Start);
        var interval1End = interval1Start + i1.Length;

        var interval2Start = Normalise(i2.Start);
        var interval2End = interval2Start + i2.Length;

        // If it works, it always works
        if (interval1Start < interval2End &&
            interval2Start < interval1End)
            return true;

        // If it doesn't work, it certainly doesn't work for void
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return false;

        // Edge case for wrap behaviour (literally)
        // Suppose the first interval is just to the right of zero
        // And suppose the other interval is just to the left of _levelLength
        // The second interval could "wrap around" to overlap with the first
        // We check this case here

        if (interval1Start < interval2Start)
        {
            interval1Start += _levelLength;
            interval1End += _levelLength;
        }
        else
        {
            interval2Start += _levelLength;
            interval2End += _levelLength;
        }

        return interval1Start < interval2End &&
               interval2Start < interval1End;
    }

    [Pure]
    public int GetDelta(int a1, int a2)
    {
        var delta = a2 - a1;

        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return delta;

        var twiceDelta = delta * 2;

        if (delta < 0)
        {
            twiceDelta = -twiceDelta;

            if (twiceDelta > _levelLength)
                return delta + _levelLength;
        }
        else
        {
            if (twiceDelta > _levelLength)
                return delta - _levelLength;
        }

        return delta;
    }

    private unsafe void UpdateViewPortRenderIntervals()
    {
        ViewPortRenderInterval* viewPortRenderIntervalPointer = (ViewPortRenderInterval*)_viewPortRenderIntervalBuffer.Handle;

        if (_boundaryBehaviourType == BoundaryBehaviourType.Void ||
            _viewPortStart + _viewPortLength < _levelLength)
        {
            viewPortRenderIntervalPointer[0] = new ViewPortRenderInterval(_viewPortStart, _viewPortLength, -_viewPortStart);
            _viewPortSpanLength = 1;
            return;
        }

        _viewPortSpanLength = 2;
        int l1;
        if (_viewPortLength < _levelLength)
        {
            l1 = _levelLength - _viewPortStart;
            viewPortRenderIntervalPointer[0] = new ViewPortRenderInterval(_viewPortStart, l1, -_viewPortStart);
            viewPortRenderIntervalPointer[1] = new ViewPortRenderInterval(0, _viewPortLength - l1, _levelLength - _viewPortStart);
        }
        else
        {
            l1 = _levelLength >>> 1;
            viewPortRenderIntervalPointer[0] = new ViewPortRenderInterval(0, l1, 0);
            viewPortRenderIntervalPointer[1] = new ViewPortRenderInterval(l1, _levelLength - l1, 0);
        }
    }

    private unsafe void UpdateScreenRenderIntervals()
    {
        ScreenRenderInterval* screenRenderIntervalPointer = (ScreenRenderInterval*)_screenRenderIntervalBuffer.Handle;

        if (_boundaryBehaviourType == BoundaryBehaviourType.Void ||
            _viewPortStart + _viewPortLength < _levelLength)
        {
            var viewPortDimensionOnScreen = _viewPortLength * _scaleMultiplier;
            _screenStart = (_screenLength - viewPortDimensionOnScreen) >>> 1;

            screenRenderIntervalPointer[0] = new ScreenRenderInterval(0, _viewPortLength, _screenStart, viewPortDimensionOnScreen);
            _screenSpanLength = 1;

            return;
        }

        _screenStart = 0;

        if (_viewPortLength < _levelLength)
        {
            screenRenderIntervalPointer[0] = new ScreenRenderInterval(0, _viewPortLength, 0, _screenLength);
            _screenSpanLength = 1;

            return;
        }

        UpdateScreenRenderIntervalsForMultipleWrappedCopies();
    }

    private unsafe void UpdateScreenRenderIntervalsForMultipleWrappedCopies()
    {
        var deltaL = _levelLength * _scaleMultiplier;
        var maxScreenDimension = Math.Min(deltaL * MaxNumberOfRenderCopiesForWrappedLevels, _screenLength);
        int screenCoordinate = Math.Max(0, _screenLength - maxScreenDimension);
        screenCoordinate >>>= 1;

        ScreenRenderInterval* screenRenderIntervalPointer = (ScreenRenderInterval*)_screenRenderIntervalBuffer.Handle;

        screenRenderIntervalPointer[0] = new ScreenRenderInterval(
            _viewPortStart,
            _levelLength - _viewPortStart,
            screenCoordinate,
            (_levelLength - _viewPortStart) * _scaleMultiplier);

        screenCoordinate += (_levelLength - _viewPortStart) * _scaleMultiplier;

        _screenSpanLength = 1;

        while (screenCoordinate < maxScreenDimension &&
               _screenSpanLength < MaxNumberOfRenderCopiesForWrappedLevels - 1)
        {
            screenRenderIntervalPointer[_screenSpanLength++] = new ScreenRenderInterval(0, _levelLength, screenCoordinate, deltaL);
            screenCoordinate += deltaL;
        }

        if (screenCoordinate < maxScreenDimension)
        {
            screenRenderIntervalPointer[_screenSpanLength++] = new ScreenRenderInterval(0, 0, screenCoordinate, 0);
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
        _viewPortRenderIntervalBuffer.Dispose();
        _screenRenderIntervalBuffer.Dispose();
    }
}
