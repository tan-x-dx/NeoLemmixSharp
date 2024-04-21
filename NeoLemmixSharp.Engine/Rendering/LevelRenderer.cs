using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class LevelRenderer : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly LevelControlPanel _levelControlPanel;
    private readonly Level.Viewport _viewport;
    private readonly IViewportObjectRenderer[] _levelSprites;

    private IBackgroundRenderer _backgroundRenderer;
    private TerrainRenderer _terrainRenderer;

    private RenderTarget2D _levelRenderTarget;
    private bool _disposed;

    public LevelRenderer(
        GraphicsDevice graphicsDevice,
        LevelControlPanel levelControlPanel,
        Level.Viewport viewport,
        IViewportObjectRenderer[] levelSprites,
        IBackgroundRenderer backgroundRenderer,
        TerrainRenderer terrainRenderer)
    {
        _graphicsDevice = graphicsDevice;
        _levelControlPanel = levelControlPanel;
        _viewport = viewport;
        _levelSprites = levelSprites;

        _backgroundRenderer = backgroundRenderer;
        _terrainRenderer = terrainRenderer;
        _levelRenderTarget = GetLevelRenderTarget2D();
    }

    public void RenderLevel(SpriteBatch spriteBatch)
    {
        _graphicsDevice.SetRenderTarget(_levelRenderTarget);
        spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

        _backgroundRenderer.RenderBackground(spriteBatch);
        _terrainRenderer.RenderTerrain(spriteBatch);
        RenderSprites(spriteBatch);

        spriteBatch.End();
    }

    private void RenderSprites(SpriteBatch spriteBatch)
    {
        var viewportX = _viewport.ViewPortX;
        var viewportY = _viewport.ViewPortY;
        var maxX = _viewport.NumberOfHorizontalRenderIntervals;
        var maxY = _viewport.NumberOfVerticalRenderIntervals;
        var levelSpritesSpan = new ReadOnlySpan<IViewportObjectRenderer>(_levelSprites);

        for (var i = 0; i < maxX; i++)
        {
            var hInterval = _viewport.GetHorizontalRenderInterval(i);
            for (var j = 0; j < maxY; j++)
            {
                var vInterval = _viewport.GetVerticalRenderInterval(j);
                var viewportClip = new Rectangle(hInterval.PixelStart, vInterval.PixelStart, hInterval.PixelLength, vInterval.PixelLength);

                foreach (var sprite in levelSpritesSpan)
                {
                    var spriteClip = sprite.GetSpriteBounds();

                    Rectangle.Intersect(ref viewportClip, ref spriteClip, out var clipIntersection);

                    if (!clipIntersection.IsEmpty)
                    {
                        clipIntersection.X -= spriteClip.X;
                        clipIntersection.Y -= spriteClip.Y;

                        var screenX = spriteClip.X + clipIntersection.X - viewportX;
                        var screenY = spriteClip.Y + clipIntersection.Y - viewportY;

                        sprite.RenderAtPosition(spriteBatch, clipIntersection, screenX, screenY);
                    }
                }
            }
        }
    }

    public void DrawToScreen(SpriteBatch spriteBatch)
    {
        var destinationRectangle = new Rectangle(
            _viewport.ScreenX,
            _viewport.ScreenY,
            _viewport.ScreenWidth,
            _viewport.ScreenHeight);

        var sourceRectangle = new Rectangle(
            0,
            0,
            _viewport.ViewPortWidth,
            _viewport.ViewPortHeight);

        spriteBatch.Draw(
            _levelRenderTarget,
            destinationRectangle,
            sourceRectangle,
            Color.White);
    }

    private RenderTarget2D GetLevelRenderTarget2D()
    {
        return new RenderTarget2D(
            _graphicsDevice,
            _graphicsDevice.PresentationParameters.BackBufferWidth,
            _graphicsDevice.PresentationParameters.BackBufferHeight - _levelControlPanel.ScreenHeight,
            false,
            _graphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);
    }

    public void OnWindowSizeChanged()
    {
        DisposableHelperMethods.DisposeOf(ref _levelRenderTarget);
        _levelRenderTarget = GetLevelRenderTarget2D();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        DisposableHelperMethods.DisposeOf(ref _backgroundRenderer);
        DisposableHelperMethods.DisposeOf(ref _terrainRenderer);
        DisposableHelperMethods.DisposeOf(ref _levelRenderTarget);
        DisposableHelperMethods.DisposeOfAll(new ReadOnlySpan<IViewportObjectRenderer>(_levelSprites));

        _disposed = true;
    }
}