using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("{_dimensionType}:{_boundaryBehaviourType} [{_levelLength}]")]
public sealed class BoundaryBehaviour
{
    private const int MaxNumberOfViewportRenderIntervals = 2;
    private const int MaxNumberOfRenderCopiesForWrappedLevels = 6;

    private ViewPortRenderIntervalBuffer _viewPortRenderIntervalBuffer;
    private ScreenRenderIntervalBuffer _screenRenderIntervalBuffer;

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
    public ReadOnlySpan<ViewPortRenderInterval> GetRenderViewPortIntervals() => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<ViewPortRenderIntervalBuffer, ViewPortRenderInterval>(ref Unsafe.AsRef(in _viewPortRenderIntervalBuffer)), _viewPortSpanLength);
    [Pure]
    public ReadOnlySpan<ScreenRenderInterval> GetScreenRenderIntervals() => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<ScreenRenderIntervalBuffer, ScreenRenderInterval>(ref Unsafe.AsRef(in _screenRenderIntervalBuffer)), _screenSpanLength);

    public BoundaryBehaviour(
        DimensionType dimensionType,
        BoundaryBehaviourType boundaryBehaviourType,
        int levelLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(levelLength);

        _dimensionType = dimensionType;
        _boundaryBehaviourType = boundaryBehaviourType;
        _levelLength = levelLength;

        _viewPortRenderIntervalBuffer = new ViewPortRenderIntervalBuffer();
        _screenRenderIntervalBuffer = new ScreenRenderIntervalBuffer();
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
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return n;

        return NormaliseWrap(n);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int NormaliseWrap(int n)
    {
        // Most likely situation for Wrap normalisation is the input
        // being just outside the bounds [0, _levelLength - 1].
        // Therefore, we can avoid a call to the modulo operator
        // by simply adding/subtracting the level length

        var levelLength = _levelLength;
        if (n < 0)
        {
            do
            {
                n += levelLength;
            }
            while (n < 0);
        }
        else
        {
            while (n >= levelLength)
            {
                n -= levelLength;
            }
        }

        return n;
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
        result = NormaliseWrap(result);
        result -= halfLevelLength;
        return result;
    }

    [Pure]
    public bool IntervalContainsPoint(Interval interval, int n)
    {
        // Shift both inputs over by the same amount
        // This does not change whether or not they intersect
        n -= interval.Start;

        // The interval now starts at zero, simplifying a check
        // If the point is smaller than the interval length, they intersect in all cases
        if (n >= 0 && n < interval.Length)
            return true;

        // By this point, the inputs definitely do not overlap for the Void type
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return false;

        n = NormaliseWrap(n);
        // Save a check since normalisation implies n >= 0 anyway
        return n < interval.Length;
    }

    [Pure]
    public bool IntervalsOverlap(Interval i1, Interval i2)
    {
        // Shift both intervals over by the same amount
        // This does not change whether or not they intersect

        // This variable corresponds to the start point of the second interval
        // The first interval now starts at zero, simplifying a check
        var s = i2.Start - i1.Start;

        // If this check succeeds, the intervals intersect in all cases
        if (s + i2.Length >= 0 &&
            s < i1.Length)
            return true;

        // By this point, the intervals definitely do not intersect for the Void type
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return false;

        s = NormaliseWrap(s);

        // After normalisation, s >= 0, so skip a check
        return s < i1.Length ||
        // Check for overlap across level boundary
               s + i2.Length > _levelLength;
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
            _viewPortStart = GetUpdatedViewPortCoordinate(_viewPortStart);
        }

        UpdateViewPortRenderIntervals();
        UpdateScreenRenderIntervals();
    }

    [Pure]
    private int GetUpdatedViewPortCoordinate(int viewPortCoordinate)
    {
        var v = _levelLength - _viewPortLength;
        if (v < 0)
            return 0;

        if (_boundaryBehaviourType != BoundaryBehaviourType.Void)
            return NormaliseWrap(viewPortCoordinate);

        if (viewPortCoordinate < 0)
            return 0;

        if (viewPortCoordinate <= v)
            return viewPortCoordinate;
        return v;
    }

    public void Scroll(int delta)
    {
        _viewPortStart = GetUpdatedViewPortCoordinate(_viewPortStart + delta);

        UpdateViewPortRenderIntervals();
        UpdateScreenRenderIntervals();
    }

    private void UpdateViewPortRenderIntervals()
    {
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void ||
            _viewPortStart + _viewPortLength < _levelLength)
        {
            _viewPortRenderIntervalBuffer[0] = new ViewPortRenderInterval(_viewPortStart, _viewPortLength, -_viewPortStart);
            _viewPortSpanLength = 1;
            return;
        }

        _viewPortSpanLength = 2;
        int l1;
        if (_viewPortLength < _levelLength)
        {
            l1 = _levelLength - _viewPortStart;
            _viewPortRenderIntervalBuffer[0] = new ViewPortRenderInterval(_viewPortStart, l1, -_viewPortStart);
            _viewPortRenderIntervalBuffer[1] = new ViewPortRenderInterval(0, _viewPortLength - l1, _levelLength - _viewPortStart);
        }
        else
        {
            l1 = _levelLength >>> 1;
            _viewPortRenderIntervalBuffer[0] = new ViewPortRenderInterval(0, l1, 0);
            _viewPortRenderIntervalBuffer[1] = new ViewPortRenderInterval(l1, _levelLength - l1, 0);
        }
    }

    private void UpdateScreenRenderIntervals()
    {
        _screenSpanLength = 1;
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void ||
            _viewPortStart + _viewPortLength < _levelLength)
        {
            var viewPortDimensionOnScreen = _viewPortLength * _scaleMultiplier;
            _screenStart = (_screenLength - viewPortDimensionOnScreen) >>> 1;

            _screenRenderIntervalBuffer[0] = new ScreenRenderInterval(0, _viewPortLength, _screenStart, viewPortDimensionOnScreen);

            return;
        }

        _screenStart = 0;

        if (_viewPortLength < _levelLength)
        {
            _screenRenderIntervalBuffer[0] = new ScreenRenderInterval(0, _viewPortLength, 0, _screenLength);

            return;
        }

        UpdateScreenRenderIntervalsForMultipleWrappedCopies();
    }

    private void UpdateScreenRenderIntervalsForMultipleWrappedCopies()
    {
        var deltaLength = _levelLength * _scaleMultiplier;
        var maxScreenDimension = Math.Min(deltaLength * MaxNumberOfRenderCopiesForWrappedLevels, _screenLength);
        var screenCoordinate = Math.Max(0, _screenLength - maxScreenDimension);
        screenCoordinate >>>= 1;
        var sourceLength = _levelLength - _viewPortStart;
        var scaledSourceLength = sourceLength * _scaleMultiplier;

        Span<ScreenRenderInterval> span = _screenRenderIntervalBuffer;

        span.At(0) = new ScreenRenderInterval(
            _viewPortStart,
            sourceLength,
            screenCoordinate,
            scaledSourceLength);

        screenCoordinate += scaledSourceLength;
        var screenSpanLength = 1;

        while (screenCoordinate < maxScreenDimension &&
               screenSpanLength < MaxNumberOfRenderCopiesForWrappedLevels - 1)
        {
            span.At(screenSpanLength++) = new ScreenRenderInterval(0, _levelLength, screenCoordinate, deltaLength);
            screenCoordinate += deltaLength;
        }

        if (screenCoordinate < maxScreenDimension)
        {
            span.At(screenSpanLength++) = new ScreenRenderInterval(0, 0, screenCoordinate, 0);
        }

        _screenSpanLength = screenSpanLength;
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

    [InlineArray(MaxNumberOfViewportRenderIntervals)]
    private struct ViewPortRenderIntervalBuffer
    {
        private ViewPortRenderInterval _0;
    }

    [InlineArray(MaxNumberOfRenderCopiesForWrappedLevels)]
    private struct ScreenRenderIntervalBuffer
    {
        private ScreenRenderInterval _0;
    }
}
