using NeoLemmixSharp.Common;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed class CanvasBorderBehaviour
{
    private int _canvasLength;
    private readonly int _levelLength;

    private int _viewportStart;
    private int _rawViewportLength;
    private int _actualViewportLength;

    private int _screenLength;

    private int _zoom = LevelEditorConstants.CanvasMinZoom;

    public int ViewportStart => _viewportStart;
    public int ViewportLength => _actualViewportLength;

    public CanvasBorderBehaviour(int levelLength)
    {
        _levelLength = levelLength;
    }

    [Pure]
    public Interval GetViewportSourceInterval()
    {
        if (_rawViewportLength > _levelLength)
            return new Interval(0, _levelLength);

        var start = _viewportStart;
        var length = _actualViewportLength;
        if (start < 0)
        {
            length += start;
            start = 0;
        }
        else
        {
            var viewportEnd = _viewportStart + _actualViewportLength;

            if (viewportEnd > _levelLength)
            {
                length = _levelLength - _viewportStart;
            }
        }

        return new Interval(start, length);
    }

    [Pure]
    public Interval GetScreenDestinationInterval()
    {
        int screenStart;
        if (_rawViewportLength > _levelLength)
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
        if (_rawViewportLength > _levelLength)
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
        if (scrollDelta == 0)
            return;

        _zoom = Math.Clamp(_zoom + scrollDelta, LevelEditorConstants.CanvasMinZoom, LevelEditorConstants.CanvasMaxZoom);

        RecalculateViewportDimensions();
        Scroll(0);
    }

    public void SetCanvasLength(int canvasLength)
    {
        _canvasLength = canvasLength;
        RecalculateViewportDimensions();
        Scroll(0);
    }

    private void RecalculateViewportDimensions()
    {
        var newRawViewportLength = (_canvasLength + _zoom - 1) / _zoom;
        _rawViewportLength = newRawViewportLength;
        if (newRawViewportLength > _levelLength)
            newRawViewportLength = _levelLength;
        _actualViewportLength = newRawViewportLength;
        _screenLength = newRawViewportLength * _zoom;
    }
}
