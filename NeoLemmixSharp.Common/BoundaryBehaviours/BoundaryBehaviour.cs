﻿using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

public sealed class BoundaryBehaviour
{
    private const int MaxNumberOfRenderIntervals = 2;
    private const int MaxNumberOfRenderCopiesForWrappedLevels = 6;


    private readonly DimensionType _dimensionType;
    private readonly BoundaryBehaviourType _boundaryBehaviourType;
    private readonly int _levelLength;

    private ViewPortRenderIntervalBuffer _viewPortRenderIntervals;
    private ScreenRenderIntervalBuffer _screenRenderIntervals;
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

    public BoundaryBehaviour(
        DimensionType dimensionType,
        BoundaryBehaviourType boundaryBehaviourType,
        int levelLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(levelLength);

        _dimensionType = dimensionType;
        _boundaryBehaviourType = boundaryBehaviourType;
        _levelLength = levelLength;
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

        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
        {
            _viewPortStart = Math.Clamp(viewPortCoordinate, 0, _levelLength - _viewPortLength);
            return;
        }

        _viewPortStart = Normalise(viewPortCoordinate);
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
            a += _levelLength;
            while (a < 0)
            {
                a += _levelLength;
            }

            return a;
        }

        while (a >= _levelLength)
        {
            a -= _levelLength;
        }

        return a;
    }

    [Pure]
    public unsafe bool IntervalContainsPoint(LevelInterval interval, int a)
    {
        int* p1 = (int*)&interval;
        p1[0] = Normalise(p1[0]);

        // If it works, it always works
        if (interval.Start <= a && a < interval.Start + interval.Length)
            return true;

        // If it doesn't work, it certainly doesn't work for void
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return false;

        a += _levelLength;
        if (interval.Start <= a && a < interval.Start + interval.Length)
            return true;

        a -= (_levelLength << 1);
        return interval.Start <= a && a < interval.Start + interval.Length;
    }

    [Pure]
    public unsafe bool IntervalsOverlap(LevelInterval l1, LevelInterval l2)
    {
        int* startPointer = (int*)&l1;
        startPointer[0] = Normalise(startPointer[0]);

        startPointer = (int*)&l2;
        startPointer[0] = Normalise(startPointer[0]);

        // If it works, it always works
        if (l1.Start < l1.Start + l1.Length &&
            l1.Start < l2.Start + l2.Length)
            return true;

        // If it doesn't work, it certainly doesn't work for void
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return false;

        startPointer[0] += _levelLength;
        if (l1.Start < l1.Start + l1.Length &&
            l1.Start < l2.Start + l2.Length)
            return true;

        startPointer[0] -= (_levelLength << 1);
        return l1.Start < l1.Start + l1.Length &&
               l1.Start < l2.Start + l2.Length;
    }

    [Pure]
    public int GetDelta(int a1, int a2)
    {
        var delta = a2 - a1;

        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return delta;

        if (delta >= 0)
        {
            if (delta * 2 > _levelLength)
                return delta - _levelLength;

            return delta;
        }

        if (delta * -2 > _levelLength)
            return delta + _levelLength;

        return delta;
    }

    private void UpdateViewPortRenderIntervals()
    {
        Span<ViewPortRenderInterval> viewPortRenderIntervalSpan = _viewPortRenderIntervals;

        if (_boundaryBehaviourType == BoundaryBehaviourType.Void ||
            _viewPortStart + _viewPortLength < _levelLength)
        {
            viewPortRenderIntervalSpan[0] = new ViewPortRenderInterval(_viewPortStart, _viewPortLength, -_viewPortStart);
            _viewPortSpanLength = 1;
            return;
        }

        _viewPortSpanLength = 2;
        int l1;
        if (_viewPortLength < _levelLength)
        {
            l1 = _levelLength - _viewPortStart;
            viewPortRenderIntervalSpan[0] = new ViewPortRenderInterval(_viewPortStart, l1, -_viewPortStart);
            viewPortRenderIntervalSpan[1] = new ViewPortRenderInterval(0, _viewPortLength - l1, _levelLength - _viewPortStart);
        }
        else
        {
            l1 = _levelLength >> 1;
            viewPortRenderIntervalSpan[0] = new ViewPortRenderInterval(0, l1, 0);
            viewPortRenderIntervalSpan[1] = new ViewPortRenderInterval(l1, _levelLength - l1, 0);
        }
    }

    private void UpdateScreenRenderIntervals()
    {
        Span<ScreenRenderInterval> screenRenderIntervalSpan = _screenRenderIntervals;

        if (_boundaryBehaviourType == BoundaryBehaviourType.Void ||
            _viewPortStart + _viewPortLength < _levelLength)
        {
            var viewPortDimensionOnScreen = _viewPortLength * _scaleMultiplier;
            _screenStart = (_screenLength - viewPortDimensionOnScreen) >> 1;

            screenRenderIntervalSpan[0] = new ScreenRenderInterval(0, _viewPortLength, _screenStart, viewPortDimensionOnScreen);
            _screenSpanLength = 1;

            return;
        }

        _screenStart = 0;

        if (_viewPortLength < _levelLength)
        {
            screenRenderIntervalSpan[0] = new ScreenRenderInterval(0, _viewPortLength, 0, _screenLength);
            _screenSpanLength = 1;

            return;
        }

        UpdateScreenRenderIntervalsForMultipleWrappedCopies();
    }

    private void UpdateScreenRenderIntervalsForMultipleWrappedCopies()
    {
        var deltaL = _levelLength * _scaleMultiplier;

        int screenCoordinate;

        var maxScreenDimension = Math.Min(deltaL * MaxNumberOfRenderCopiesForWrappedLevels, _screenLength);

        if (maxScreenDimension < _screenLength)
        {
            screenCoordinate = (_screenLength - maxScreenDimension) >> 1;
        }
        else
        {
            screenCoordinate = 0;
        }

        Span<ScreenRenderInterval> screenRenderIntervalSpan = _screenRenderIntervals;

        screenRenderIntervalSpan[0] = new ScreenRenderInterval(
            _viewPortStart,
            _levelLength - _viewPortStart,
            screenCoordinate,
            (_levelLength - _viewPortStart) * _scaleMultiplier);

        screenCoordinate += (_levelLength - _viewPortStart) * _scaleMultiplier;

        _screenSpanLength = 1;

        while (screenCoordinate < maxScreenDimension &&
               _screenSpanLength < screenRenderIntervalSpan.Length - 1)
        {
            screenRenderIntervalSpan[_screenSpanLength++] = new ScreenRenderInterval(0, _levelLength, screenCoordinate, deltaL);
            screenCoordinate += deltaL;
        }

        if (screenCoordinate < maxScreenDimension)
        {
            screenRenderIntervalSpan[_screenSpanLength++] = new ScreenRenderInterval(0, 0, screenCoordinate, 0);
        }
    }

    [Pure]
    public ReadOnlySpan<ViewPortRenderInterval> GetRenderIntervals() => MemoryMarshal.CreateReadOnlySpan(
        ref Unsafe.As<ViewPortRenderIntervalBuffer, ViewPortRenderInterval>(ref _viewPortRenderIntervals), _viewPortSpanLength);

    [Pure]
    public ReadOnlySpan<ScreenRenderInterval> GetScreenRenderIntervals() => MemoryMarshal.CreateReadOnlySpan(
        ref Unsafe.As<ScreenRenderIntervalBuffer, ScreenRenderInterval>(ref _screenRenderIntervals), _screenSpanLength);

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

    [InlineArray(MaxNumberOfRenderIntervals)]
    private struct ViewPortRenderIntervalBuffer
    {
        private ViewPortRenderInterval _0;
    }

    [InlineArray(MaxNumberOfRenderCopiesForWrappedLevels)]
    private struct ScreenRenderIntervalBuffer
    {
        private ScreenRenderInterval _0;
    }

    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[10 + 3 + 4 + 3 + 10];

        var i = 0;

        var source = _dimensionType == DimensionType.Horizontal
            ? HorizontalString()
            : VerticalString();
        source.CopyTo(buffer);
        i += source.Length;

        buffer[i++] = ' ';
        buffer[i++] = '-';
        buffer[i++] = ' ';

        source = _boundaryBehaviourType == BoundaryBehaviourType.Void
            ? VoidString()
            : WrapString();
        source.CopyTo(buffer[i..]);
        i += source.Length;

        buffer[i++] = ' ';
        buffer[i++] = '-';
        buffer[i++] = ' ';

        _levelLength.TryFormat(buffer[i..], out var charsWritten);
        i += charsWritten;

        return buffer[..i].ToString();

        static ReadOnlySpan<char> HorizontalString() => "Horizontal";
        static ReadOnlySpan<char> VerticalString() => "Vertical";

        static ReadOnlySpan<char> VoidString() => "Void";
        static ReadOnlySpan<char> WrapString() => "Wrap";
    }
}