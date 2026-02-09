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
    private ClickDragMode _clickDragMode = ClickDragMode.None;

    private void OnMouseEnter(Component c, Point screenPosition)
    {
    }

    private void OnMouseMove(Component c, Point screenPosition)
    {
        _screenMouseMovePosition = screenPosition;
        _canvasMouseMovePosition = CalculateCanvasMousePosition(screenPosition);
    }

    private void OnMousePressed(Component c, Point screenPosition)
    {
        _screenMouseDownPosition = screenPosition;
        _canvasMouseDownPosition = CalculateCanvasMousePosition(screenPosition);
        _screenMouseMovePosition = screenPosition;
        _canvasMouseMovePosition = _canvasMouseDownPosition;

        var pieceBeneathMouse = TrySelectSingleItem();
        if (pieceBeneathMouse is null)
        {
            _selectedCanvasPieces.Clear();
            _clickDragMode = ClickDragMode.SelectPieces;
            return;
        }

        if (_selectedCanvasPieces.Contains(pieceBeneathMouse))
            return;

        _selectedCanvasPieces.Clear();
        _selectedCanvasPieces.Add(pieceBeneathMouse);
    }

    private void OnMouseHeld(Component c, Point screenPosition)
    {
        var pieceBeneathMouse = TrySelectSingleItem();
        if (pieceBeneathMouse is null || !_selectedCanvasPieces.Contains(pieceBeneathMouse))
        {
            _selectedCanvasPieces.Clear();
            _clickDragMode = ClickDragMode.SelectPieces;
            return;
        }

        _clickDragMode = ClickDragMode.DragPieces;
        var offset = _canvasMouseMovePosition - _canvasMouseDownPosition;

        foreach (var piece in _selectedCanvasPieces)
        {
            piece.Move(offset);
        }

        RepaintLevel();
    }

    private void OnMouseReleased(Component c, Point screenPosition)
    {
        if (_clickDragMode == ClickDragMode.SelectPieces)
        {
            _screenMouseDownPosition = screenPosition;
            _selectedCanvasPieces.Clear();
            var clickDragBounds = new RectangularRegion(_canvasMouseDownPosition, _canvasMouseMovePosition);
            TrySelectMultipleItems(clickDragBounds);
        }
        else if (_clickDragMode == ClickDragMode.DragPieces)
        {
            foreach (var piece in _selectedCanvasPieces)
            {
                piece.FixPosition();
            }
        }

        _clickDragMode = ClickDragMode.None;
    }

    private void OnMouseExit(Component c, Point screenPosition)
    {
        _clickDragMode = ClickDragMode.None;
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

    private enum ClickDragMode
    {
        None,
        SelectPieces,
        DragPieces
    }
}
