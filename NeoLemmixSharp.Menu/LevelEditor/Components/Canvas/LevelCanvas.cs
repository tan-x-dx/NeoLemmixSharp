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

public sealed class LevelCanvas : Component
{
    public const int LevelOuterBoundarySize = 128;
    private const uint CanvasBorderColourValue = 0xff696969;

    private readonly GraphicsDevice _graphicsDevice;
    private RenderTarget2D _levelTexture;
    private ArrayWrapper2D<Color> _levelColors;

    private CanvasBorderBehaviour _horizontalBorderBehaviour;
    private CanvasBorderBehaviour _verticalBorderBehaviour;

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
            var horizontalCameraInterval = _horizontalBorderBehaviour.GetViewPortSourceInterval();
            var verticalCameraInterval = _verticalBorderBehaviour.GetViewPortSourceInterval();

            return new Rectangle(
                horizontalCameraInterval.Start,
                verticalCameraInterval.Start,
                horizontalCameraInterval.Length,
                verticalCameraInterval.Length);
        }

        Rectangle GetDestinationRectangle()
        {
            var horizontalScreenInterval = _horizontalBorderBehaviour.GetScreenDestinationInterval();
            var verticalScreenCameraInterval = _verticalBorderBehaviour.GetScreenDestinationInterval();

            return new Rectangle(
                Left + horizontalScreenInterval.Start,
                Top + verticalScreenCameraInterval.Start,
                horizontalScreenInterval.Length,
                verticalScreenCameraInterval.Length);
        }
    }

    private void OnLevelDataChanged()
    {
        RecreateRenderers();
        RepaintLevel();

        _horizontalBorderBehaviour = new CanvasBorderBehaviour(_levelTexture.Width);
        _verticalBorderBehaviour = new CanvasBorderBehaviour(_levelTexture.Height);

        RecentreCamera();
    }

    public void OnCanvasResize()
    {
        _horizontalBorderBehaviour.SetScreenLength(Width);
        _verticalBorderBehaviour.SetScreenLength(Height);
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
        var levelDimensions = LevelData.LevelDimensions;
        return new RenderTarget2D(
            _graphicsDevice,
            levelDimensions.W + (2 * LevelOuterBoundarySize),
            levelDimensions.H + (2 * LevelOuterBoundarySize),
            false,
            _graphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);
    }

    public void RepaintLevel()
    {
        new Span<Color>(_levelColors.Array).Fill(Color.Black);
        var textureWidth = _levelTexture.Width;
        var textureHeight = _levelTexture.Height;

        for (var y = 0; y < 16; y++)
        {
            for (var x = 0; x < 16; x++)
            {
                var p = new Point(x, y);
                _levelColors[p] = Color.Red;

                p = new Point(textureWidth - x - 1, y);
                _levelColors[p] = Color.Red;

                p = new Point(x, textureHeight - y - 1);
                _levelColors[p] = Color.Red;

                p = new Point(textureWidth - x - 1, textureHeight - y - 1);
                _levelColors[p] = Color.Red;
            }
        }

        for (var y = 0; y < textureHeight; y++)
        {
            var color = ((y >>> 4) & 1) != 0
                ? Color.AliceBlue
                : Color.Crimson;

            var p = new Point(0, y);
            _levelColors[p] = color;

            p = new Point(textureWidth - 1, y);
            _levelColors[p] = color;
        }

        for (var x = 0; x < textureWidth; x++)
        {
            var color = ((x >>> 4) & 1) != 0
                ? Color.AliceBlue
                : Color.Crimson;

            var p = new Point(x, 0);
            _levelColors[p] = color;

            p = new Point(x, textureHeight - 1);
            _levelColors[p] = color;
        }

        _levelTexture.SetData(_levelColors.Array);
    }

    public Point GetCenterPositionOfCamera()
    {
        var cameraX = _horizontalBorderBehaviour.ViewportStart;
        var cameraY = _verticalBorderBehaviour.ViewportStart;

        var offsetX = _horizontalBorderBehaviour.ViewportLength / 2;
        var offsetY = _verticalBorderBehaviour.ViewportLength / 2;

        return new Point(cameraX + offsetX, cameraY + offsetY);
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

        return new Point(scrollDx, scrollDy);
    }

    public void Scroll(int dx, int dy)
    {
        _horizontalBorderBehaviour.Scroll(dx);
        _verticalBorderBehaviour.Scroll(dy);
    }

    private void RecentreCamera()
    {
        _horizontalBorderBehaviour.RecentreCamera();
        _verticalBorderBehaviour.RecentreCamera();
    }

    protected override void OnDispose()
    {
        _levelTexture.Dispose();
    }
}
