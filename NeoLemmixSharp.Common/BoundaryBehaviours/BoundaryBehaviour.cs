using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

public sealed class BoundaryBehaviour
{
    public const int MaxNumberOfRenderIntervals = 2;
    public const int MaxNumberOfRenderCopiesForWrappedLevels = 6;

    private readonly int _levelDimension;

    private int _scaleMultiplier;

    private int _viewPortCoordinate;
    private int _viewPortDimension;

    private int _screenCoordinate;
    private int _screenDimension;

    private int _mouseViewPortCoordinate;
    private int _mouseScreenCoordinate;

    public BoundaryBehaviourType BoundaryBehaviourType { get; }

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
        BoundaryBehaviourType = boundaryBehaviourType;
        _levelDimension = levelDimension;
    }

    public void UpdateMouseCoordinate(int windowCoordinate)
    {
        _mouseViewPortCoordinate = (windowCoordinate + _scaleMultiplier - 1) / _scaleMultiplier;
        _mouseScreenCoordinate = _mouseViewPortCoordinate * _scaleMultiplier + _screenCoordinate;

        _mouseViewPortCoordinate = Normalise(_mouseViewPortCoordinate - _viewPortCoordinate);
    }

    public void UpdateScreenDimension(
        int screenDimension,
        int scaleMultiplier)
    {
        _scaleMultiplier = scaleMultiplier;
        _viewPortDimension = (screenDimension + scaleMultiplier - 1) / scaleMultiplier;
        _screenDimension = _viewPortDimension * scaleMultiplier;
        _screenCoordinate = screenDimension - _screenDimension;

        if (_levelDimension < _viewPortDimension)
        {
            _viewPortCoordinate = 0;
            _viewPortDimension = _levelDimension;
        }
        else
        {
            UpdateViewPortCoordinate(_viewPortCoordinate);
        }
    }

    private void UpdateViewPortCoordinate(int viewPortCoordinate)
    {
        if (_levelDimension < _viewPortDimension)
        {
            _viewPortCoordinate = 0;
            return;
        }

        if (BoundaryBehaviourType == BoundaryBehaviourType.Void)
        {
            _viewPortCoordinate = Math.Clamp(viewPortCoordinate, 0, _levelDimension - _viewPortDimension);
            return;
        }

        _viewPortCoordinate = Normalise(viewPortCoordinate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Scroll(int delta) => UpdateViewPortCoordinate(_viewPortCoordinate + delta);

    [Pure]
    public int Normalise(int a)
    {
        if (BoundaryBehaviourType == BoundaryBehaviourType.Void)
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
        if (BoundaryBehaviourType == BoundaryBehaviourType.Void ||
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

        if (BoundaryBehaviourType == BoundaryBehaviourType.Void)
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

    public ReadOnlySpan<ViewPortRenderInterval> GetRenderIntervals(Span<ViewPortRenderInterval> baseSpan)
    {
        if (baseSpan.Length != MaxNumberOfRenderIntervals)
            throw new ArgumentException($"baseSpan should have length {MaxNumberOfRenderIntervals}", nameof(baseSpan));

        if (BoundaryBehaviourType == BoundaryBehaviourType.Void ||
            _viewPortCoordinate + _viewPortDimension < _levelDimension)
        {
            baseSpan[0] = new ViewPortRenderInterval(_viewPortCoordinate, _viewPortDimension, -_viewPortCoordinate);
            return baseSpan[..1];
        }

        int l1;
        if (_viewPortDimension < _levelDimension)
        {
            l1 = _levelDimension - _viewPortCoordinate;
            baseSpan[0] = new ViewPortRenderInterval(_viewPortCoordinate, l1, -_viewPortCoordinate);
            baseSpan[1] = new ViewPortRenderInterval(0, _viewPortDimension - l1, _levelDimension - _viewPortCoordinate);
        }
        else
        {
            l1 = _levelDimension >> 1;
            baseSpan[0] = new ViewPortRenderInterval(0, l1, 0);
            baseSpan[1] = new ViewPortRenderInterval(l1, _levelDimension - l1, 0);
        }

        return baseSpan;
    }

    public ReadOnlySpan<ScreenRenderInterval> GetScreenRenderIntervals(Span<ScreenRenderInterval> baseSpan)
    {
        if (baseSpan.Length != MaxNumberOfRenderCopiesForWrappedLevels)
            throw new ArgumentException($"baseSpan should have length {MaxNumberOfRenderCopiesForWrappedLevels}", nameof(baseSpan));

        var viewPortDimensionOnScreen = _viewPortDimension * _scaleMultiplier;

        if (BoundaryBehaviourType == BoundaryBehaviourType.Void ||
            _viewPortCoordinate + _viewPortDimension < _levelDimension)
        {
            baseSpan[0] = new ScreenRenderInterval(0, _viewPortDimension, _screenCoordinate + (_screenDimension - viewPortDimensionOnScreen) / 2, viewPortDimensionOnScreen);
            return baseSpan[..1];
        }

        if (_viewPortDimension < _levelDimension)
        {
            baseSpan[0] = new ScreenRenderInterval(0, _viewPortDimension, _screenCoordinate, _screenDimension);

            return baseSpan[..1];
        }

        var l1 = _levelDimension - _viewPortCoordinate;
        var l2 = l1 * _scaleMultiplier;
        var deltaL = _levelDimension * _scaleMultiplier;

        baseSpan[0] = new ScreenRenderInterval(_viewPortCoordinate, l1, 0, l2);

        var length = l2;
        var spanLength = 1;

        while (length < _screenDimension &&
               spanLength < baseSpan.Length)
        {
            baseSpan[spanLength++] = new ScreenRenderInterval(0, _levelDimension, length, deltaL);
            length += deltaL;
        }

        //    baseSpan[spanLength++] = new ScreenRenderInterval(0, )

        return baseSpan[..spanLength];
    }
}