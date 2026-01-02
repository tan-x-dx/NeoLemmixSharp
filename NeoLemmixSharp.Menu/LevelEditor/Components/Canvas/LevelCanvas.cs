using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.Ui.Components;
using Color = Microsoft.Xna.Framework.Color;
using Point = NeoLemmixSharp.Common.Point;
using Size = NeoLemmixSharp.Common.Size;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed class LevelCanvas : Component
{
    private const int ScrollDelta = 16;

    private const int MinZoom = 1;
    private const int MaxZoom = 12;

    private const int NegativeSpaceBoundary = 128;

    private const int MinCanvasBorderThickness = 16;
    private const uint CanvasBorderColourValue = 0xff696969;

    private readonly GraphicsDevice _graphicsDevice;
    private RenderTarget2D _levelTexture;
    private ArrayWrapper2D<Color> _levelColors;

    private BoundaryBehaviour _horizontalBoundaryBehaviour;
    private BoundaryBehaviour _verticalBoundaryBehaviour;

    private int _zoom = MinZoom;
    private RectangularRegion _cameraBounds;

    public LevelData LevelData
    {
        get => field;
        set
        {
            field = value;
            OnLevelDataChanged();
        }
    }

    public LevelCanvas(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        RenderCanvasBorder(spriteBatch);
        RenderLevel(spriteBatch);
    }

    private void RenderCanvasBorder(SpriteBatch spriteBatch)
    {
        var rectangle = Helpers.CreateRectangle(Position, Dimensions);

        spriteBatch.FillRect(rectangle, new Color(CanvasBorderColourValue));
    }

    private void RenderLevel(SpriteBatch spriteBatch)
    {
        var horizontalRenderIntervals = _horizontalBoundaryBehaviour.GetScreenRenderIntervals();
        var verticalRenderIntervals = _verticalBoundaryBehaviour.GetScreenRenderIntervals();

        foreach (var horizontalRenderInterval in horizontalRenderIntervals)
        {
            foreach (var verticalRenderInterval in verticalRenderIntervals)
            {
                var destinationRectangle = new Rectangle(
                    horizontalRenderInterval.ScreenStart,
                    verticalRenderInterval.ScreenStart,
                    horizontalRenderInterval.ScreenLength,
                    verticalRenderInterval.ScreenLength);

                var sourceRectangle = new Rectangle(
                    horizontalRenderInterval.SourceStart,
                    verticalRenderInterval.SourceStart,
                    horizontalRenderInterval.SourceLength,
                    verticalRenderInterval.SourceLength);

                destinationRectangle.X += Left;
                destinationRectangle.Y += Top;
                destinationRectangle.Width *= _zoom;
                destinationRectangle.Height *= _zoom;

                spriteBatch.Draw(
                    _levelTexture,
                    destinationRectangle,
                    sourceRectangle,
                    Color.White);
            }
        }
    }

    private void OnLevelDataChanged()
    {
        RecreateRenderers();
        RepaintLevel();

        var levelDimensions = LevelData.LevelDimensions;
        _horizontalBoundaryBehaviour = new BoundaryBehaviour(DimensionType.Horizontal, BoundaryBehaviourType.Void, levelDimensions.W);
        _verticalBoundaryBehaviour = new BoundaryBehaviour(DimensionType.Vertical, BoundaryBehaviourType.Void, levelDimensions.H);

        RecalculateCameraDimensions();
        RecentreCamera();
    }

    public void OnCanvasResize()
    {
        RecalculateCameraDimensions();
        Scroll(0, 0);
    }

    private void RecreateRenderers()
    {
        _levelTexture?.Dispose();
        _levelTexture = CreateControlPanelRenderTarget2D();

        var levelDimensions = LevelData.LevelDimensions;
        _levelColors = new ArrayWrapper2D<Color>(levelDimensions);
    }

    private RenderTarget2D CreateControlPanelRenderTarget2D()
    {
        var levelDimensions = LevelData.LevelDimensions;
        return new RenderTarget2D(
            _graphicsDevice,
            levelDimensions.W,
            levelDimensions.H,
            false,
            _graphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);
    }

    public void RepaintLevel()
    {
        new Span<Color>(_levelColors.Array).Fill(Color.Black);
        var levelDimensions = LevelData.LevelDimensions;

        for (var y = 0; y < 16; y++)
        {
            for (var x = 0; x < 16; x++)
            {
                var p = new Point(x, y);
                _levelColors[p] = Color.Red;

                p = new Point(levelDimensions.W - x - 1, y);
                _levelColors[p] = Color.Red;

                p = new Point(x, levelDimensions.H - y - 1);
                _levelColors[p] = Color.Red;

                p = new Point(levelDimensions.W - x - 1, levelDimensions.H - y - 1);
                _levelColors[p] = Color.Red;
            }
        }

        for (var y = 0; y < levelDimensions.H; y++)
        {
            var p = new Point(0, y);
            _levelColors[p] = Color.AliceBlue;

            p = new Point(levelDimensions.W - 1, y);
            _levelColors[p] = Color.AliceBlue;
        }

        for (var x = 0; x < levelDimensions.W; x++)
        {
            var p = new Point(x, 0);
            _levelColors[p] = Color.AliceBlue;

            p = new Point(x, levelDimensions.H - 1);
            _levelColors[p] = Color.AliceBlue;
        }

        _levelTexture.SetData(_levelColors.Array);
    }

    public Point GetCenterPositionOfCamera()
    {
        var cameraPosition = _cameraBounds.Position;

        var offset = new Point(_cameraBounds.W >>> 1, _cameraBounds.H >>> 1);

        return cameraPosition + offset;
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
        if (scrollDelta == 0)
            return;

        var currentZoom = Math.Clamp(_zoom + scrollDelta, MinZoom, MaxZoom);

        _zoom = currentZoom;

        RecalculateCameraDimensions();
    }

    private static Point CalculateScrollDelta(MenuInputController inputController)
    {
        var down = inputController.DownArrow.IsActionDown ? 1 : 0;
        var left = inputController.LeftArrow.IsActionDown ? 1 : 0;
        var up = inputController.UpArrow.IsActionDown ? 1 : 0;
        var right = inputController.RightArrow.IsActionDown ? 1 : 0;

        var scrollDx = right - left;
        var scrollDy = down - up;

        return new Point(scrollDx, scrollDy);
    }

    public void Scroll(int dx, int dy)
    {
        dx = Math.Sign(dx);
        dy = Math.Sign(dy);

        dx *= ScrollDelta;
        dy *= ScrollDelta;

        var newX = _cameraBounds.X + dx;
        var newY = _cameraBounds.Y + dy;

        var levelSize = LevelData.LevelDimensions;

        newX = ClampCameraPosition(newX, levelSize.W);
        newY = ClampCameraPosition(newY, levelSize.H);

        var actualScrollDeltaX = newX - _cameraBounds.X;
        var actualScrollDeltaY = newY - _cameraBounds.Y;

        _cameraBounds = new RectangularRegion(new Point(newX, newY), _cameraBounds.Size);
        _horizontalBoundaryBehaviour.Scroll(actualScrollDeltaX);
        _verticalBoundaryBehaviour.Scroll(actualScrollDeltaY);

        return;

        static int ClampCameraPosition(int cameraPosition, int levelDimension)
        {
            var result = cameraPosition;

            if (cameraPosition < -NegativeSpaceBoundary)
                result = -NegativeSpaceBoundary;

            var max = levelDimension + NegativeSpaceBoundary;
            if (cameraPosition > max)
                result = max;

            return result;
        }
    }

    private void RecentreCamera()
    {
        var levelSize = LevelData.LevelDimensions;

        var halfLevelWidth = levelSize.W / 2;
        var halfLevelHeight = levelSize.H / 2;

        var halfCameraWidth = _cameraBounds.W / 2;
        var halfCameraHeight = _cameraBounds.H / 2;

        var newCameraX = halfLevelWidth - halfCameraWidth;
        var newCameraY = halfLevelHeight - halfCameraHeight;

        _cameraBounds = new RectangularRegion(new Point(newCameraX, newCameraY), _cameraBounds.Size);
    }

    private void RecalculateCameraDimensions()
    {
        var canvasWidth = Width;
        var canvasHeight = Height;

        var newCameraWidth = canvasWidth / _zoom;
        var newCameraHeight = canvasHeight / _zoom;
        _cameraBounds = new RectangularRegion(_cameraBounds.Position, new Size(newCameraWidth, newCameraHeight));

        _horizontalBoundaryBehaviour.UpdateScreenDimension(newCameraWidth, _zoom);
        _verticalBoundaryBehaviour.UpdateScreenDimension(newCameraHeight, _zoom);
    }

    protected override void OnDispose()
    {
        _levelTexture.Dispose();
    }
}
