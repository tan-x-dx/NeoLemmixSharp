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
        _mouseScreenCoordinate = _mouseViewPortCoordinate * _scaleMultiplier;
        _mouseViewPortCoordinate -= _viewPortCoordinate;
    }

    public void UpdateScreenDimension(
        int screenDimension,
        int scaleMultiplier)
    {
        _scaleMultiplier = scaleMultiplier;
        _viewPortDimension  = (screenDimension + scaleMultiplier - 1) / scaleMultiplier;
        _screenDimension = _viewPortDimension * scaleMultiplier;
        _screenCoordinate = screenDimension - _screenDimension;
        _viewPortCoordinate = _screenCoordinate + (_levelDimension - _viewPortDimension) / 2;

        SetViewPortCoordinate(_viewPortCoordinate);
    }

    public void SetViewPortCoordinate(int viewPortCoordinate)
    {
        if (BoundaryBehaviourType == BoundaryBehaviourType.Void)
        {
            if (_levelDimension < _viewPortDimension)
            {
                _viewPortCoordinate = _screenCoordinate + (_levelDimension - _viewPortDimension) / 2;
            }
            else
            {
                _viewPortCoordinate = Math.Clamp(viewPortCoordinate, 0, _levelDimension - _viewPortDimension);
            }

            return;
        }

        _viewPortCoordinate = Normalise(viewPortCoordinate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Scroll(int delta) => SetViewPortCoordinate(_viewPortCoordinate + delta);

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
        if (BoundaryBehaviourType == BoundaryBehaviourType.Void)
            return;

        if (right < _levelDimension)
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
            baseSpan[0] = new ViewPortRenderInterval(_viewPortCoordinate, _viewPortDimension, 0);
            return baseSpan[..1];
        }

        int l1;
        if (_viewPortDimension < _levelDimension)
        {
            l1 = _levelDimension - _viewPortCoordinate;
            baseSpan[0] = new ViewPortRenderInterval(_viewPortCoordinate, l1, 0);
            baseSpan[1] = new ViewPortRenderInterval(0, _viewPortDimension - l1, _levelDimension);
        }
        else
        {
            l1 = _levelDimension >> 1;
            baseSpan[0] = new ViewPortRenderInterval(0, l1, 0);
            baseSpan[1] = new ViewPortRenderInterval(l1, _levelDimension - l1, l1);
        }

        return baseSpan;
    }

    public ReadOnlySpan<ScreenRenderInterval> GetScreenRenderIntervals(Span<ScreenRenderInterval> baseSpan)
    {
        if (baseSpan.Length != MaxNumberOfRenderCopiesForWrappedLevels)
            throw new ArgumentException($"baseSpan should have length {MaxNumberOfRenderCopiesForWrappedLevels}", nameof(baseSpan));

        if (BoundaryBehaviourType == BoundaryBehaviourType.Void ||
            _viewPortCoordinate + _viewPortDimension < _levelDimension)
        {
            baseSpan[0] = new ScreenRenderInterval(0, _viewPortDimension, _screenCoordinate, _screenDimension);
            return baseSpan[..1];
        }

        int l1;
        int l2;
        if (_viewPortDimension < _levelDimension)
        {
            l1 = _levelDimension - _viewPortCoordinate;
            l2 = l1 * _scaleMultiplier;
            baseSpan[0] = new ScreenRenderInterval(0, l1, _screenCoordinate, l2);
            baseSpan[1] = new ScreenRenderInterval(l1, _viewPortDimension - l1, l2, _screenDimension - l2);

            return baseSpan[..2];
        }

        throw new NotImplementedException();

        /*    l1 = _levelDimension >> 1;
            l2 = l1 * _scaleMultiplier;

            var length = 0;
            var spanLength = 0;
            while (length < _viewPortDimension &&
                   spanLength < baseSpan.Length)
            {

                length += _levelDimension;
            }


            baseSpan[0] = new ScreenRenderInterval(0, l1);
            baseSpan[1] = new ScreenRenderInterval(l1, _levelDimension - l1);

            return baseSpan[..spanLength];*/
    }
}