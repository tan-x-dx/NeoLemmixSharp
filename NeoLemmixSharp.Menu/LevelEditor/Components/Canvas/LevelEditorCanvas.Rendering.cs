using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using Point = NeoLemmixSharp.Common.Point;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed partial class LevelEditorCanvas
{
    private readonly GraphicsDevice _graphicsDevice;

    private RenderTarget2D _levelTexture;
    private LevelEditorTerrainPainter _terrainPainter;

    public override void Render(SpriteBatch spriteBatch)
    {
        RenderCanvasBorder(spriteBatch);
        RenderCanvas(spriteBatch);
        RenderSelectedPieces(spriteBatch);

        if (_clickDragMode == ClickDragMode.SelectPieces)
        {
            var screenDragBounds = new RectangularRegion(_screenMouseDownPosition, _screenMouseMovePosition);

            spriteBatch.DrawRect(screenDragBounds.ToRectangle(), Color.White);
        }
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

    private void RenderSelectedPieces(SpriteBatch spriteBatch)
    {
        if (_selectedCanvasPieces.Count == 0)
            return;

        var viewBounds = new RectangularRegion(
            _horizontalBorderBehaviour.GetViewportSourceInterval(),
            _verticalBorderBehaviour.GetViewportSourceInterval())
            .Translate(LevelEditorConstants.InverseRenderOffset);
        var horizontalScreenBounds = _horizontalBorderBehaviour.GetScreenDestinationInterval();
        var verticalScreenBounds = _verticalBorderBehaviour.GetScreenDestinationInterval();
        var offset = Position + new Point(horizontalScreenBounds.Start, verticalScreenBounds.Start);

        foreach (var piece in _selectedCanvasPieces)
        {
            if (!piece.OverlapsRegion(viewBounds))
                continue;

            var pieceLocation = piece.Position - viewBounds.Position;
            pieceLocation = new Point(pieceLocation.X * _horizontalBorderBehaviour.ZoomValue, pieceLocation.Y * _verticalBorderBehaviour.ZoomValue);
            pieceLocation += offset;

            var pieceSize = piece.Size.Scale(_horizontalBorderBehaviour.ZoomValue, _verticalBorderBehaviour.ZoomValue);
            var outlineColor = piece.GetOutlineColor();

            spriteBatch.DrawRect(Helpers.CreateRectangle(pieceLocation, pieceSize), outlineColor);
        }
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
}
