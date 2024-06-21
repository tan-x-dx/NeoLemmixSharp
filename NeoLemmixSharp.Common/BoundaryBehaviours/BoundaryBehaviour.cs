using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

public sealed class BoundaryBehaviour
{
    private const int MaxNumberOfRenderIntervals = 2;
    private const int MaxNumberOfRenderCopiesForWrappedLevels = 6;

    private readonly BoundaryBehaviourType _boundaryBehaviourType;
    private readonly int _levelDimension;

    private readonly ViewPortRenderInterval[] _viewPortRenderIntervals = new ViewPortRenderInterval[MaxNumberOfRenderIntervals];
    private readonly ScreenRenderInterval[] _screenRenderIntervals = new ScreenRenderInterval[MaxNumberOfRenderCopiesForWrappedLevels];
    private int _viewPortSpanLength;
    private int _screenSpanLength;

    private int _scaleMultiplier;

    private int _viewPortCoordinate;
    private int _viewPortDimension;

    private int _screenCoordinate;
    private int _screenDimension;

    private int _mouseViewPortCoordinate;
    private int _mouseScreenCoordinate;

    public BoundaryBehaviourType BoundaryBehaviourType => _boundaryBehaviourType;

    /// <summary>
    /// The level dimension in pixels
    /// </summary>
    public int LevelDimension => _levelDimension;

    /// <summary>
    /// The starting pixel of the view port
    /// </summary>
    public int ViewPortCoordinate => _viewPortCoordinate;

    /// <summary>
    /// The size in pixels of the view port. Note: ViewPortDimension is derived from the ScreenDimension and the scale multiplier
    /// </summary>
    public int ViewPortDimension => _viewPortDimension;

    /// <summary>
    /// The starting pixel where this should be rendered on screen
    /// </summary>
    public int ScreenCoordinate => _screenCoordinate;

    /// <summary>
    /// How many pixels are available on screen for rendering purposes
    /// </summary>
    public int ScreenDimension => _screenDimension;

    /// <summary>
    /// The coordinate of the mouse within the view port
    /// </summary>
    public int MouseViewPortCoordinate => _mouseViewPortCoordinate;

    /// <summary>
    /// The coordinate of the mouse within the screen
    /// </summary>
    public int MouseScreenCoordinate => _mouseScreenCoordinate;

    public BoundaryBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int levelDimension)
    {
        _boundaryBehaviourType = boundaryBehaviourType;
        _levelDimension = levelDimension;
    }

    public void UpdateMouseCoordinate(int windowCoordinate)
    {
        _mouseViewPortCoordinate = (windowCoordinate + _scaleMultiplier - 1) / _scaleMultiplier;
        _mouseScreenCoordinate = _mouseViewPortCoordinate * _scaleMultiplier;

        _mouseViewPortCoordinate = Normalise(_mouseViewPortCoordinate - _viewPortCoordinate);
    }

    public void UpdateScreenDimension(
        int screenDimension,
        int scaleMultiplier)
    {
        _scaleMultiplier = scaleMultiplier;
        _viewPortDimension = (screenDimension + _scaleMultiplier - 1) / _scaleMultiplier;
        _screenDimension = _viewPortDimension * _scaleMultiplier;

        if (_levelDimension < _viewPortDimension)
        {
            _viewPortCoordinate = 0;
            _viewPortDimension = _levelDimension;
        }
        else
        {
            UpdateViewPortCoordinate(_viewPortCoordinate);
        }

        UpdateViewPortRenderIntervals();
        UpdateScreenRenderIntervals();
    }

    private void UpdateViewPortCoordinate(int viewPortCoordinate)
    {
        if (_levelDimension < _viewPortDimension)
        {
            _viewPortCoordinate = 0;
            return;
        }

        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
        {
            _viewPortCoordinate = Math.Clamp(viewPortCoordinate, 0, _levelDimension - _viewPortDimension);
            return;
        }

        _viewPortCoordinate = Normalise(viewPortCoordinate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Scroll(int delta)
    {
        UpdateViewPortCoordinate(_viewPortCoordinate + delta);

        UpdateViewPortRenderIntervals();
        UpdateScreenRenderIntervals();
    }

    [Pure]
    public int Normalise(int a)
    {
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return a;

        if (a < 0)
        {
            do
            {
                a += _levelDimension;
            } while (a < 0);

            return a;
        }

        while (a >= _levelDimension)
        {
            a -= _levelDimension;
        }

        return a;
    }

    public void NormaliseCoords(ref int left, ref int right, ref int a)
    {
        // Do nothing - coords will already be fine
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void ||
            (left > 0 &&
             right < _levelDimension))
            return;

        var halfLevelWidth = _levelDimension / 2;
        left -= halfLevelWidth;
        right -= halfLevelWidth;
        a -= halfLevelWidth;
    }

    [Pure]
    public int GetDelta(int a1, int a2)
    {
        var delta = a2 - a1;

        if (_boundaryBehaviourType == BoundaryBehaviourType.Void)
            return delta;

        if (delta > 0)
        {
            if (delta * 2 > _levelDimension)
                return delta - _levelDimension;

            return delta;
        }

        if (delta * 2 < -_levelDimension)
            return delta + _levelDimension;

        return delta;
    }

    public bool CrossesBoundary(int spriteClipX, int spriteClipWidth)
    {
        return _boundaryBehaviourType == BoundaryBehaviourType.Wrap &&
               spriteClipX + spriteClipWidth > _levelDimension;
    }

    private void UpdateViewPortRenderIntervals()
    {
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void ||
            _viewPortCoordinate + _viewPortDimension < _levelDimension)
        {
            _viewPortRenderIntervals[0] = new ViewPortRenderInterval(_viewPortCoordinate, _viewPortDimension, -_viewPortCoordinate, 0);
            _viewPortSpanLength = 1;
            return;
        }

        _viewPortSpanLength = 2;
        int l1;
        if (_viewPortDimension < _levelDimension)
        {
            l1 = _levelDimension - _viewPortCoordinate;
            _viewPortRenderIntervals[0] = new ViewPortRenderInterval(_viewPortCoordinate, l1, -_viewPortCoordinate, 0);
            _viewPortRenderIntervals[1] = new ViewPortRenderInterval(0, _viewPortDimension - l1, _levelDimension - _viewPortCoordinate, -_levelDimension);
        }
        else
        {
            l1 = _levelDimension >> 1;
            _viewPortRenderIntervals[0] = new ViewPortRenderInterval(0, l1, 0, 0);
            _viewPortRenderIntervals[1] = new ViewPortRenderInterval(l1, _levelDimension - l1, 0, -_levelDimension);
        }
    }

    private void UpdateScreenRenderIntervals()
    {
        if (_boundaryBehaviourType == BoundaryBehaviourType.Void ||
            _viewPortCoordinate + _viewPortDimension < _levelDimension)
        {
            var viewPortDimensionOnScreen = _viewPortDimension * _scaleMultiplier;
            _screenCoordinate = (_screenDimension - viewPortDimensionOnScreen) >> 1;

            _screenRenderIntervals[0] = new ScreenRenderInterval(0, _viewPortDimension, _screenCoordinate, viewPortDimensionOnScreen);
            _screenSpanLength = 1;

            return;
        }

        _screenCoordinate = 0;

        if (_viewPortDimension < _levelDimension)
        {
            _screenRenderIntervals[0] = new ScreenRenderInterval(0, _viewPortDimension, 0, _screenDimension);
            _screenSpanLength = 1;

            return;
        }

        var l1 = _levelDimension - _viewPortCoordinate;
        var l2 = l1 * _scaleMultiplier;
        var deltaL = _levelDimension * _scaleMultiplier;

        _screenRenderIntervals[0] = new ScreenRenderInterval(_viewPortCoordinate, l1, 0, l2);

        var length = l2;
        _screenSpanLength = 1;

        while (length < _screenDimension &&
               _screenSpanLength < _screenRenderIntervals.Length)
        {
            _screenRenderIntervals[_screenSpanLength++] = new ScreenRenderInterval(0, _levelDimension, length, deltaL);
            length += deltaL;
        }

        //    baseSpan[spanLength++] = new ScreenRenderInterval(0, )

    }

    public ReadOnlySpan<ViewPortRenderInterval> GetRenderIntervals() => new(_viewPortRenderIntervals, 0, _viewPortSpanLength);
    public ReadOnlySpan<ScreenRenderInterval> GetScreenRenderIntervals() => new(_screenRenderIntervals, 0, _screenSpanLength);
}