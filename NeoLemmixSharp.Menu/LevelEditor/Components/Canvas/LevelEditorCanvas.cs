using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.GameInput;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;
using Point = NeoLemmixSharp.Common.Point;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed partial class LevelEditorCanvas : Component
{
    private readonly CanvasBorderBehaviour _horizontalBorderBehaviour = new();
    private readonly CanvasBorderBehaviour _verticalBorderBehaviour = new();

    private LevelData _levelData;

    public LevelEditorCanvas(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;

        MouseEnter.RegisterMouseMoveEvent(OnMouseEnter);
        MouseMovement.RegisterMouseMoveEvent(OnMouseMove);
        MousePressed.RegisterMousePressEvent(OnLeftMousePressed, MouseButtonType.Left);
        MousePressed.RegisterMousePressEvent(OnRightMousePressed, MouseButtonType.Right);
        MouseHeld.RegisterMousePressEvent(OnLeftMouseHeld, MouseButtonType.Left);
        MouseReleased.RegisterMousePressEvent(OnLeftMouseReleased, MouseButtonType.Left);
        MouseExit.RegisterMouseMoveEvent(OnMouseExit);
        KeyPressed.RegisterKeyEvent(OnKeyDown);
    }

    public void SetLevelData(LevelData levelData)
    {
        _levelData = levelData;
        OnLevelDataChanged();
    }

    public void OnLevelDataChanged()
    {
        RecreateRenderers();
        _horizontalBorderBehaviour.SetLevelLength(_levelTexture.Width);
        _verticalBorderBehaviour.SetLevelLength(_levelTexture.Height);
        OnCanvasResize();

        RecreatePieces();
        RenumberAllPieces();
        RepaintLevel();

        RecentreViewport();
    }

    public void OnCanvasResize()
    {
        _horizontalBorderBehaviour.SetCanvasLength(Width);
        _verticalBorderBehaviour.SetCanvasLength(Height);
        Scroll(0, 0);
    }

    private Point GetCenterPositionOfViewport()
    {
        var viewportX = _horizontalBorderBehaviour.ViewportStart - LevelEditorConstants.LevelOuterBoundarySize;
        var viewportY = _verticalBorderBehaviour.ViewportStart - LevelEditorConstants.LevelOuterBoundarySize;

        var offsetX = _horizontalBorderBehaviour.ViewportLength / 2;
        var offsetY = _verticalBorderBehaviour.ViewportLength / 2;

        return new Point(viewportX + offsetX, viewportY + offsetY);
    }

    public void HandleUserInput(MenuController inputController)
    {
        if (!ContainsPoint(inputController.MousePosition))
            return;

        if (inputController.ScrollDelta != 0 && _clickDragMode == ClickDragMode.None)
            Zoom(inputController.ScrollDelta);

        var scrollDelta = CalculateArrowKeyScrollDelta(inputController);

        if (scrollDelta.X != 0 || scrollDelta.Y != 0)
            Scroll(scrollDelta.X, scrollDelta.Y);
    }

    private void Zoom(int scrollDelta)
    {
        _horizontalBorderBehaviour.Zoom(scrollDelta);
        _verticalBorderBehaviour.Zoom(scrollDelta);
    }

    private static Point CalculateArrowKeyScrollDelta(MenuController inputController)
    {
        if (UiHandler.Instance.SelectedTextField is not null)
            return new Point();

        var leftFrames = inputController.LeftArrow.NumberOfFramesHeldDownFor;
        var rightFrames = inputController.RightArrow.NumberOfFramesHeldDownFor;
        var scrollDx = EvaluateScrollDelta(leftFrames, rightFrames);

        var upFrames = inputController.UpArrow.NumberOfFramesHeldDownFor;
        var downFrames = inputController.DownArrow.NumberOfFramesHeldDownFor;
        var scrollDy = EvaluateScrollDelta(upFrames, downFrames);

        return new Point(scrollDx, scrollDy);

        static int EvaluateScrollDelta(int numberOfFramesNegativeHeldDownFor, int numberOfFramesPositiveHeldDownFor)
        {
            if (numberOfFramesNegativeHeldDownFor == 0 && numberOfFramesPositiveHeldDownFor == 0)
                return 0;
            if (numberOfFramesNegativeHeldDownFor > 0 && numberOfFramesPositiveHeldDownFor > 0)
                return 0;

            var mult = 1;
            if (numberOfFramesNegativeHeldDownFor > 0)
                mult = -1;

            var numberOfFramesHeldDownFor = numberOfFramesNegativeHeldDownFor | numberOfFramesPositiveHeldDownFor;

            if (numberOfFramesHeldDownFor > 1 &&
                numberOfFramesHeldDownFor < UiConstants.KeyboardInputFrameDelay)
                return 0;

            return mult;
        }
    }

    public void Scroll(int dx, int dy)
    {
        if (_selectedCanvasPieces.Count > 0)
        {
            var delta = new Point(dx, dy);
            var clampBounds = GetPieceClampBounds();

            foreach (var piece in _selectedCanvasPieces)
            {
                piece.Move(delta, clampBounds);
                piece.FixPosition(clampBounds);
            }

            RepaintLevel();
        }
        else
        {
            _horizontalBorderBehaviour.Scroll(dx * LevelEditorConstants.ArrowKeyScrollDelta);
            _verticalBorderBehaviour.Scroll(dy * LevelEditorConstants.ArrowKeyScrollDelta);
        }
    }

    private void RecentreViewport()
    {
        _horizontalBorderBehaviour.RecentreViewport();
        _verticalBorderBehaviour.RecentreViewport();
    }

    private void OnKeyDown(Component c, in BitArrayEnumerable<InputHandler, Keys> keys)
    {
    }

    protected override void OnDispose()
    {
        _levelTexture.Dispose();
    }
}
