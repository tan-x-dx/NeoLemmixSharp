using NeoLemmixSharp.Common;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed class CanvasBorderBehaviour
{
    private const int ExtraSpaceBoundary = 16;

    private const int ScrollDelta = 16;

    private const int MinZoom = 1;
    private const int MaxZoom = 12;

    private readonly int _levelLength;

    private int _viewportStart;
    private int _viewPortLength;

    private int _screenStart;
    private int _screenLength;

    private int _zoom = MinZoom;

    public int ViewportStart => _viewportStart;
    public int ViewportLength => _viewPortLength;

    public CanvasBorderBehaviour(int levelLength)
    {
        _levelLength = levelLength;
    }

    [Pure]
    public Interval GetViewPortSourceInterval()
    {
        var levelInterval = new Interval(0, _levelLength);
        return new Interval(_viewportStart, _viewPortLength).Intersect(levelInterval);
    }

    [Pure]
    public Interval GetScreenDestinationInterval()
    {
        return new Interval(_screenStart, _screenLength);
    }

    public void Scroll(int delta)
    {
        delta = Math.Sign(delta);
        delta *= ScrollDelta;

        _viewportStart += delta;
        ClampViewportPosition();
    }

    private void ClampViewportPosition()
    {
        if (_viewPortLength >= _levelLength)
        {
            _viewportStart = 0;
        }
        else if (_viewportStart < -ExtraSpaceBoundary)
        {
            _viewportStart = -ExtraSpaceBoundary;
        }
        else
        {
            var max = _levelLength - _viewPortLength + ExtraSpaceBoundary + ExtraSpaceBoundary;
            if (_viewportStart > max)
            {
                _viewportStart = max;
            }
        }

        UpdateScreenPosition();
    }

    private void UpdateScreenPosition()
    {
        _screenStart = _viewportStart < 0
            ? -1 * _viewportStart * _zoom
            : 0;
    }

    public void RecentreCamera()
    {
        var halfLevelLength = _levelLength / 2;
        var halfCameraLength = _viewPortLength / 2;

        _viewportStart = halfLevelLength - halfCameraLength;
        ClampViewportPosition();
    }

    public void Zoom(int scrollDelta)
    {
        if (scrollDelta == 0)
            return;

        _zoom = Math.Clamp(_zoom + scrollDelta, MinZoom, MaxZoom);

        RecalculateCameraDimensions();
        Scroll(0);
    }

    public void SetScreenLength(int screenLength)
    {
        _screenLength = screenLength;
        RecalculateCameraDimensions();
        Scroll(0);
    }

    private void RecalculateCameraDimensions()
    {
        _viewPortLength = (_screenLength + _zoom - 1) / _zoom;
        _screenLength = _viewPortLength * _zoom;
    }
}
