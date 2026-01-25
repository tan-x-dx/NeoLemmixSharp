using NeoLemmixSharp.Common;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed class CanvasBorderBehaviour
{
    private int _canvasLength;
    private int _levelLength = 100;

    private int _viewportStart;
    private int _rawViewportLength;
    private int _actualViewportLength;

    private int _screenLength;

    private int _zoom = LevelEditorConstants.CanvasMinZoom;

    public int ViewportStart => _viewportStart;
    public int ViewportLength => _actualViewportLength;

    private bool ViewportIsLargerThanLevel => _rawViewportLength > _levelLength;

    [Pure]
    public Interval GetViewportSourceInterval()
    {
        if (ViewportIsLargerThanLevel)
            return new Interval(0, _levelLength);

        if (_viewportStart < 0)
            return new Interval(0, _viewportStart + _actualViewportLength);

        var length = _actualViewportLength;
        var viewportEnd = _viewportStart + _actualViewportLength;

        if (viewportEnd > _levelLength)
        {
            length = _levelLength - _viewportStart;
        }

        return new Interval(_viewportStart, length);
    }

    [Pure]
    public Interval GetScreenDestinationInterval()
    {
        int screenStart;
        if (ViewportIsLargerThanLevel)
        {
            screenStart = (_canvasLength - _screenLength) / 2;
            return new Interval(screenStart, _screenLength);
        }

        screenStart = 0;
        var screenLengthModifier = 0;

        if (_viewportStart < 0)
        {
            screenLengthModifier = _viewportStart;
            screenLengthModifier *= _zoom;
            screenStart = -screenLengthModifier;
        }
        else
        {
            var viewportEnd = _viewportStart + _actualViewportLength;

            if (viewportEnd > _levelLength)
            {
                screenLengthModifier = _levelLength - _viewportStart;
                screenLengthModifier *= _zoom;
                screenLengthModifier -= _screenLength;
            }
        }

        return new Interval(screenStart, _screenLength + screenLengthModifier);
    }

    public void Scroll(int delta)
    {
        _viewportStart += delta;
        ClampViewportPosition();
    }

    private void ClampViewportPosition()
    {
        if (ViewportIsLargerThanLevel)
        {
            _viewportStart = 0;
        }
        else if (_viewportStart < -LevelEditorConstants.CanvasExtraSpaceBoundary)
        {
            _viewportStart = -LevelEditorConstants.CanvasExtraSpaceBoundary;
        }
        else
        {
            var max = _levelLength - _actualViewportLength + LevelEditorConstants.CanvasExtraSpaceBoundary;
            if (_viewportStart > max)
            {
                _viewportStart = max;
            }
        }
    }

    public void RecentreViewport()
    {
        var halfLevelLength = _levelLength / 2;
        var halfViewportLength = _actualViewportLength / 2;

        _viewportStart = halfLevelLength - halfViewportLength;
        ClampViewportPosition();
    }

    public void Zoom(int scrollDelta)
    {
        _zoom = Math.Clamp(_zoom + scrollDelta, LevelEditorConstants.CanvasMinZoom, LevelEditorConstants.CanvasMaxZoom);
        RecalculateViewportDimensions();
    }

    public void SetLevelLength(int levelLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(levelLength);

        _levelLength = levelLength;
        RecalculateViewportDimensions();
    }

    public void SetCanvasLength(int canvasLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(canvasLength);

        _canvasLength = canvasLength;
        RecalculateViewportDimensions();
    }

    private void RecalculateViewportDimensions()
    {
        var newRawViewportLength = (_canvasLength + _zoom - 1) / _zoom;
        _rawViewportLength = newRawViewportLength;
        if (newRawViewportLength > _levelLength)
            newRawViewportLength = _levelLength;
        _actualViewportLength = newRawViewportLength;
        _screenLength = newRawViewportLength * _zoom;

        ClampViewportPosition();
    }

    public int ToLevelCoordinate(int screenCoordinate)
    {
        if (ViewportIsLargerThanLevel)
        {
            var screenStart = (_canvasLength - _screenLength) / 2;
            var result =  screenCoordinate - screenStart;

            return result /= _zoom;
        }

        screenCoordinate /= _zoom;

        return screenCoordinate + _viewportStart;
    }
}
