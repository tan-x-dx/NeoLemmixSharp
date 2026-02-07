using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.GameInput;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.Ui.Components;
using Point = NeoLemmixSharp.Common.Point;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed partial class LevelEditorCanvas : Component
{
    private readonly CanvasBorderBehaviour _horizontalBorderBehaviour = new();
    private readonly CanvasBorderBehaviour _verticalBorderBehaviour = new();

    private LevelData _levelData;

    public LevelEditorCanvas(GraphicsDevice graphicsDevice, InputController inputController)
    {
        _graphicsDevice = graphicsDevice;
        _inputController = inputController;

        MouseEnter.RegisterMouseEvent(OnMouseEnter);
        MouseMovement.RegisterMouseEvent(OnMouseMove);
        MousePressed.RegisterMouseEvent(OnMousePressed);
        MouseHeld.RegisterMouseEvent(OnMouseHeld);
        MouseReleased.RegisterMouseEvent(OnMouseReleased);
        MouseExit.RegisterMouseEvent(OnMouseExit);
        KeyPressed.RegisterKeyEvent(OnKeyDown);
    }

    public void SetLevelData(LevelData levelData)
    {
        _levelData = levelData;
        OnLevelDataChanged();
    }

    private void OnLevelDataChanged()
    {
        RecreateRenderers();
        RepaintLevel();

        _horizontalBorderBehaviour.SetLevelLength(_levelTexture.Width);
        _verticalBorderBehaviour.SetLevelLength(_levelTexture.Height);

        RecentreViewport();
    }

    public void OnCanvasResize()
    {
        _horizontalBorderBehaviour.SetCanvasLength(Width);
        _verticalBorderBehaviour.SetCanvasLength(Height);
        Scroll(0, 0);
    }

    public Point GetCenterPositionOfViewport()
    {
        var viewportX = _horizontalBorderBehaviour.ViewportStart;
        var viewportY = _verticalBorderBehaviour.ViewportStart;

        var offsetX = _horizontalBorderBehaviour.ViewportLength / 2;
        var offsetY = _verticalBorderBehaviour.ViewportLength / 2;

        return new Point(viewportX + offsetX - Left, viewportY + offsetY - Top);
    }

    public void HandleUserInput(MenuInputController inputController)
    {
        if (!ContainsPoint(inputController.MousePosition))
            return;

        if (inputController.ScrollDelta != 0)
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

    private static Point CalculateArrowKeyScrollDelta(MenuInputController inputController)
    {
        var down = inputController.DownArrow.IsActionDown ? 1 : 0;
        var left = inputController.LeftArrow.IsActionDown ? 1 : 0;
        var up = inputController.UpArrow.IsActionDown ? 1 : 0;
        var right = inputController.RightArrow.IsActionDown ? 1 : 0;

        var scrollDx = right - left;
        var scrollDy = down - up;

        scrollDx *= LevelEditorConstants.ArrowKeyScrollDelta;
        scrollDy *= LevelEditorConstants.ArrowKeyScrollDelta;

        return new Point(scrollDx, scrollDy);
    }

    public void Scroll(int dx, int dy)
    {
        _horizontalBorderBehaviour.Scroll(dx);
        _verticalBorderBehaviour.Scroll(dy);
    }

    private void RecentreViewport()
    {
        _horizontalBorderBehaviour.RecentreViewport();
        _verticalBorderBehaviour.RecentreViewport();
    }

    private void OnKeyDown(Component c, in BitArrayEnumerable<InputController, Keys> keys)
    {
    }

    protected override void OnDispose()
    {
        _levelTexture.Dispose();
    }
}
