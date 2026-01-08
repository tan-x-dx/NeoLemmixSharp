using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.Ui.Components;
using Color = Microsoft.Xna.Framework.Color;
using Point = NeoLemmixSharp.Common.Point;
using Size = NeoLemmixSharp.Common.Size;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed class LevelEditorCanvas : Component
{
    private readonly GraphicsDevice _graphicsDevice;
    private RenderTarget2D _levelTexture;
    private ArrayWrapper2D<Color> _levelColors;

    private CanvasBorderBehaviour _horizontalBorderBehaviour;
    private CanvasBorderBehaviour _verticalBorderBehaviour;

    private CanvasPainter _canvasPainter;

    private LevelData _levelData;

    public LevelEditorCanvas(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public void SetLevelData(LevelData levelData)
    {
        _levelData = levelData;
        OnLevelDataChanged();
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        RenderCanvasBorder(spriteBatch);
        RenderLevel(spriteBatch);
    }

    private void RenderCanvasBorder(SpriteBatch spriteBatch)
    {
        var rectangle = Helpers.CreateRectangle(Position, Dimensions);

        spriteBatch.FillRect(rectangle, LevelEditorConstants.CanvasBorderColour);
    }

    private void RenderLevel(SpriteBatch spriteBatch)
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

        _horizontalBorderBehaviour = new CanvasBorderBehaviour(_levelTexture.Width);
        _verticalBorderBehaviour = new CanvasBorderBehaviour(_levelTexture.Height);

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

        var textureDimensions = new Size(_levelTexture.Width, _levelTexture.Height);

        _levelColors = new ArrayWrapper2D<Color>(textureDimensions);
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
        new Span<Color>(_levelColors.Array).Fill(Color.Black);

        _levelTexture.SetData(_levelColors.Array);
    }

    public Point GetCenterPositionOfViewport()
    {
        var viewportX = _horizontalBorderBehaviour.ViewportStart;
        var viewportY = _verticalBorderBehaviour.ViewportStart;

        var offsetX = _horizontalBorderBehaviour.ViewportLength / 2;
        var offsetY = _verticalBorderBehaviour.ViewportLength / 2;

        return new Point(viewportX + offsetX, viewportY + offsetY);
    }

    public void HandleUserInput(MenuInputController inputController)
    {
        if (!ContainsPoint(inputController.MousePosition))
            return;

        Zoom(inputController.ScrollDelta);

        var scrollDelta = CalculateScrollDelta(inputController);

        if (scrollDelta.X != 0 || scrollDelta.Y != 0)
            Scroll(scrollDelta.X, scrollDelta.Y);
    }

    private void Zoom(int scrollDelta)
    {
        _horizontalBorderBehaviour.Zoom(scrollDelta);
        _verticalBorderBehaviour.Zoom(scrollDelta);
    }

    private static Point CalculateScrollDelta(MenuInputController inputController)
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

    protected override void OnDispose()
    {
        _levelTexture.Dispose();
    }
}
