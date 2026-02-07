using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.GameInput;
using NeoLemmixSharp.Ui.Components;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed partial class LevelEditorCanvas
{
    private readonly InputController _inputController;
    private readonly List<CanvasPiece> _selectedCanvasPieces = new(16);

    private Point _screenMouseDownPosition;
    private Point _screenMouseMovePosition;

    private Point _canvasMouseDownPosition;
    private Point _canvasMouseMovePosition;
    private bool _isPerformingClickDrag;

    private void OnMouseEnter(Component c, Point screenPosition)
    {
    }

    private void OnMouseMove(Component c, Point screenPosition)
    {
        _screenMouseMovePosition = screenPosition;
        if (_isPerformingClickDrag)
        {
            _canvasMouseMovePosition = CalculateCanvasMousePosition(screenPosition);
        }
    }

    private void OnMousePressed(Component c, Point screenPosition)
    {
        _screenMouseDownPosition = screenPosition;
        _canvasMouseDownPosition = CalculateCanvasMousePosition(screenPosition);
    }

    private void OnMouseHeld(Component c, Point screenPosition)
    {
        var numberOfFramesLeftClickHasBeenHeldFor = _inputController.LeftMouseButtonAction.NumberOfFramesHeldDownFor;

        if (numberOfFramesLeftClickHasBeenHeldFor >= LevelEditorConstants.CanvasClickDragDelay)
        {
            BeginClickDrag();
        }
    }

    private void OnMouseReleased(Component c, Point screenPosition)
    {
        if (_isPerformingClickDrag)
        {
            EndClickDrag(screenPosition);
            EvaluateSelection(true);
        }
        else
        {
            EvaluateSelection(false);
        }
    }

    private void BeginClickDrag()
    {
        _isPerformingClickDrag = true;
    }

    private void EndClickDrag(Point screenPosition)
    {
        _isPerformingClickDrag = false;
        _screenMouseDownPosition = screenPosition;
    }

    private void OnMouseExit(Component c, Point screenPosition)
    {
        _isPerformingClickDrag = false;
        _canvasMouseDownPosition = new Point(-4000, -4000);
        _canvasMouseMovePosition = new Point(-4000, -4000);
    }

    [Pure]
    private Point CalculateCanvasMousePosition(Point screenPosition)
    {
        var localX = screenPosition.X - Left;
        var canvasMouseX = _horizontalBorderBehaviour.ToLevelCoordinate(localX);
        canvasMouseX -= LevelEditorConstants.LevelOuterBoundarySize;

        var localY = screenPosition.Y - Top;
        var canvasMouseY = _verticalBorderBehaviour.ToLevelCoordinate(localY);
        canvasMouseY -= LevelEditorConstants.LevelOuterBoundarySize;

        return new Point(canvasMouseX, canvasMouseY);
    }

    private void EvaluateSelection(bool isClickDrag)
    {
        _selectedCanvasPieces.Clear();

        if (isClickDrag)
        {
            var clickDragBounds = new RectangularRegion(_canvasMouseDownPosition, _canvasMouseMovePosition);
            TrySelectMultipleItems(clickDragBounds);
        }
        else
        {
            TrySelectSingleItem();
        }
    }
}
