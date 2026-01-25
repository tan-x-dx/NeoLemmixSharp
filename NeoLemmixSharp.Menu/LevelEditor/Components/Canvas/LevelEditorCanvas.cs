using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.GameInput;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.Ui.Components;
using Color = Microsoft.Xna.Framework.Color;
using Point = NeoLemmixSharp.Common.Point;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed partial class LevelEditorCanvas : Component
{
    private readonly GraphicsDevice _graphicsDevice;
    private RenderTarget2D _levelTexture;

    private readonly CanvasBorderBehaviour _horizontalBorderBehaviour = new();
    private readonly CanvasBorderBehaviour _verticalBorderBehaviour = new();

    private readonly List<CanvasPiece> _terrainPieces = new(64);
    private readonly List<CanvasPiece> _gadgetPieces = new(16);
    private readonly List<CanvasPiece> _preplacedLemmingPieces = new(16);

    private readonly List<CanvasPiece> _selectedCanvasPieces = new(16);

    private Point _canvasMousePosition;

    private LevelEditorTerrainPainter _terrainPainter;

    private LevelData _levelData;

    public LevelEditorCanvas(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;

        MouseEnter.RegisterMouseEvent(OnMouseEnter);
        MouseMovement.RegisterMouseEvent(OnMouseMove);
        MousePressed.RegisterMouseEvent(OnMouseDown);
        MouseHeld.RegisterMouseEvent(OnMouseHeld);
        MouseExit.RegisterMouseEvent(OnMouseExit);
        KeyPressed.RegisterKeyEvent(OnKeyDown);
    }

    public void SetLevelData(LevelData levelData)
    {
        _levelData = levelData;
        OnLevelDataChanged();
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        RenderCanvasBorder(spriteBatch);
        RenderCanvas(spriteBatch);
    }

    private void RenderCanvasBorder(SpriteBatch spriteBatch)
    {
        var rectangle = Helpers.CreateRectangle(Position, Dimensions);

        spriteBatch.FillRect(rectangle, LevelEditorConstants.CanvasBorderColour);
    }

    private void RenderCanvas(SpriteBatch spriteBatch)
    {
        var sourceRectangle = GetSourceRectangle();
        var destinationRectangle = GetDestinationRectangle();

        spriteBatch.Draw(
            _levelTexture,
            destinationRectangle,
            sourceRectangle,
            Color.White);

        return;

        Rectangle GetSourceRectangle()
        {
            var horizontalViewportInterval = _horizontalBorderBehaviour.GetViewportSourceInterval();
            var verticalViewportInterval = _verticalBorderBehaviour.GetViewportSourceInterval();

            return new Rectangle(
                horizontalViewportInterval.Start,
                verticalViewportInterval.Start,
                horizontalViewportInterval.Length,
                verticalViewportInterval.Length);
        }

        Rectangle GetDestinationRectangle()
        {
            var horizontalScreenInterval = _horizontalBorderBehaviour.GetScreenDestinationInterval();
            var verticalScreenInterval = _verticalBorderBehaviour.GetScreenDestinationInterval();

            return new Rectangle(
                Left + horizontalScreenInterval.Start,
                Top + verticalScreenInterval.Start,
                horizontalScreenInterval.Length,
                verticalScreenInterval.Length);
        }
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

    private void RecreateRenderers()
    {
        _levelTexture?.Dispose();
        _levelTexture = CreateControlPanelRenderTarget2D();

        _terrainPainter = new LevelEditorTerrainPainter(_levelData, _levelTexture);
    }

    private RenderTarget2D CreateControlPanelRenderTarget2D()
    {
        var levelDimensions = _levelData.LevelDimensions;
        return new RenderTarget2D(
            _graphicsDevice,
            levelDimensions.W + (2 * LevelEditorConstants.LevelOuterBoundarySize),
            levelDimensions.H + (2 * LevelEditorConstants.LevelOuterBoundarySize),
            false,
            _graphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);
    }
    public void RepaintLevel()
    {
        SortCanvasPieces();
        RepopulateLevelDataContents();

        _terrainPainter.RepaintTerrain();
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

    private void OnMouseEnter(Component c, Point screenPosition)
    {
        RecalculateCanvasMousePosition(screenPosition);
    }

    private void OnMouseMove(Component c, Point screenPosition)
    {
        RecalculateCanvasMousePosition(screenPosition);
    }

    private void RecalculateCanvasMousePosition(Point screenPosition)
    {
        var localX = screenPosition.X - Left;
        var canvasMouseX = _horizontalBorderBehaviour.ToLevelCoordinate(localX);
        canvasMouseX -= LevelEditorConstants.LevelOuterBoundarySize;

        var localY = screenPosition.Y - Top;
        var canvasMouseY = _verticalBorderBehaviour.ToLevelCoordinate(localY);
        canvasMouseY -= LevelEditorConstants.LevelOuterBoundarySize;

        _canvasMousePosition = new Point(canvasMouseX, canvasMouseY);
    }

    private void OnMouseDown(Component c, Point position)
    {
    }

    private void OnMouseHeld(Component c, Point position)
    {
    }

    private void OnMouseExit(Component c, Point position)
    {
        _canvasMousePosition = new Point(-4000, -4000);
    }

    private void OnKeyDown(Component c, in BitArrayEnumerable<InputController, Keys> keys)
    {
    }

    protected override void OnDispose()
    {
        _levelTexture.Dispose();
    }
}
