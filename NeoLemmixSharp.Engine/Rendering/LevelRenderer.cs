using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class LevelRenderer : IDisposable, IPerfectHasher<IViewportObjectRenderer>
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly LevelControlPanel _levelControlPanel;
    private readonly Level.Viewport _viewport;
    private readonly SpacialHashGrid<IViewportObjectRenderer> _spriteSpacialHashGrid;

    private readonly List<IViewportObjectRenderer> _orderedSprites;
    private IBackgroundRenderer _backgroundRenderer;

    private RenderTarget2D _levelRenderTarget;
    private bool _disposed;

    public LevelRenderer(
        GraphicsDevice graphicsDevice,
        LevelControlPanel levelControlPanel,
        Level.Viewport viewport,
        List<IViewportObjectRenderer> orderedSprites,
        IBackgroundRenderer backgroundRenderer)
    {
        _graphicsDevice = graphicsDevice;
        _levelControlPanel = levelControlPanel;
        _viewport = viewport;

        _orderedSprites = orderedSprites;
        _backgroundRenderer = backgroundRenderer;

        _levelRenderTarget = GetLevelRenderTarget2D();

        _spriteSpacialHashGrid = new SpacialHashGrid<IViewportObjectRenderer>(
            this,
            ChunkSizeType.ChunkSize64,
            viewport.HorizontalBoundaryBehaviour,
            viewport.VerticalBoundaryBehaviour);

        var span = CollectionsMarshal.AsSpan(_orderedSprites);
        foreach (var renderer in span)
        {
            _spriteSpacialHashGrid.AddItem(renderer);
        }
    }

    public void RenderLevel(SpriteBatch spriteBatch)
    {
        var span = CollectionsMarshal.AsSpan(_orderedSprites);
        foreach (var renderer in span)
        {
            _spriteSpacialHashGrid.UpdateItemPosition(renderer);
        }

        _graphicsDevice.SetRenderTarget(_levelRenderTarget);
        spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

        _backgroundRenderer.RenderBackground(spriteBatch);
        RenderSprites(spriteBatch);

        spriteBatch.End();
    }

    [SkipLocalsInit]
    private void RenderSprites(SpriteBatch spriteBatch)
    {
        Span<ViewPortRenderInterval> renderIntervalSpan = stackalloc ViewPortRenderInterval[BoundaryBehaviour.MaxNumberOfRenderIntervals * 2];
        Span<uint> scratchSpaceSpan = stackalloc uint[_spriteSpacialHashGrid.ScratchSpaceSize];

        var horizontalBoundary = _viewport.HorizontalBoundaryBehaviour;
        var verticalBoundary = _viewport.VerticalBoundaryBehaviour;

        var horizontalRenderIntervals = horizontalBoundary.GetRenderIntervals(renderIntervalSpan[..BoundaryBehaviour.MaxNumberOfRenderIntervals]);
        var verticalRenderIntervals = verticalBoundary.GetRenderIntervals(renderIntervalSpan.Slice(BoundaryBehaviour.MaxNumberOfRenderIntervals, BoundaryBehaviour.MaxNumberOfRenderIntervals));

        foreach (var horizontalRenderInterval in horizontalRenderIntervals)
        {
            foreach (var verticalRenderInterval in verticalRenderIntervals)
            {
                var region = new LevelPositionPair(
                    horizontalRenderInterval.ViewPortCoordinate,
                    verticalRenderInterval.ViewPortCoordinate,
                    horizontalRenderInterval.ViewPortCoordinate + horizontalRenderInterval.ViewPortDimension - 1,
                    verticalRenderInterval.ViewPortCoordinate + verticalRenderInterval.ViewPortDimension - 1);

                var rendererSet = _spriteSpacialHashGrid.GetAllItemsNearRegion(scratchSpaceSpan, region);

                var viewportClip = new Rectangle(
                    horizontalRenderInterval.ViewPortCoordinate,
                    verticalRenderInterval.ViewPortCoordinate,
                    horizontalRenderInterval.ViewPortDimension,
                    verticalRenderInterval.ViewPortDimension);

                foreach (var renderer in rendererSet)
                {
                    var spriteClip = renderer.GetSpriteBounds();

                    Rectangle.Intersect(ref viewportClip, ref spriteClip, out var clipIntersection);

                    if (clipIntersection.IsEmpty)
                        continue;

                    var projectionX = horizontalRenderInterval.Offset + clipIntersection.X;
                    var projectionY = verticalRenderInterval.Offset + clipIntersection.Y;

                    clipIntersection.X -= spriteClip.X;
                    clipIntersection.Y -= spriteClip.Y;

                    if (renderer is TerrainRenderer)
                    {
                        ;
                    }

                    renderer.RenderAtPosition(spriteBatch, clipIntersection, projectionX, projectionY);
                }
            }
        }

        if (LevelScreen.Screenshot)
        {
            const string fileName = @"C:\Temp\foo.png";
            using (var fileStream = File.Create(fileName))
            {
                _levelRenderTarget.SaveAsPng(fileStream, _levelRenderTarget.Width, _levelRenderTarget.Height);
            }
        }
    }

    [SkipLocalsInit]
    public void DrawToScreen(SpriteBatch spriteBatch)
    {
        Span<ScreenRenderInterval> renderIntervals = stackalloc ScreenRenderInterval[BoundaryBehaviour.MaxNumberOfRenderCopiesForWrappedLevels * 2];

        var horizontalRenderIntervals = _viewport.HorizontalBoundaryBehaviour.GetScreenRenderIntervals(renderIntervals[..BoundaryBehaviour.MaxNumberOfRenderCopiesForWrappedLevels]);
        var verticalRenderIntervals = _viewport.VerticalBoundaryBehaviour.GetScreenRenderIntervals(renderIntervals.Slice(BoundaryBehaviour.MaxNumberOfRenderCopiesForWrappedLevels, BoundaryBehaviour.MaxNumberOfRenderCopiesForWrappedLevels));

        foreach (var horizontalRenderInterval in horizontalRenderIntervals)
        {
            foreach (var verticalRenderInterval in verticalRenderIntervals)
            {
                var destinationRectangle = new Rectangle(
                    horizontalRenderInterval.ScreenCoordinate,
                    verticalRenderInterval.ScreenCoordinate,
                    horizontalRenderInterval.ScreenDimension,
                    verticalRenderInterval.ScreenDimension);

                var sourceRectangle = new Rectangle(
                    horizontalRenderInterval.SourceCoordinate,
                    verticalRenderInterval.SourceCoordinate,
                    horizontalRenderInterval.SourceDimension,
                    verticalRenderInterval.SourceDimension);

                spriteBatch.Draw(
                    _levelRenderTarget,
                    destinationRectangle,
                    sourceRectangle,
                    Color.White);
            }
        }

        var b = _viewport.HorizontalBoundaryBehaviour;
        var data = $"H: vpC:{b.ViewPortCoordinate}, vpD: {b.ViewPortDimension}, sC: {b.ScreenCoordinate}, sD: {b.ScreenDimension}, mvpC{b.MouseViewPortCoordinate}, msC: {b.MouseScreenCoordinate}, lD: {b.LevelDimension}";
        FontBank.MenuFont.RenderText(spriteBatch, data, 10, 0 * MenuFont.GlyphHeight + 10, 1, Color.White);

        b = _viewport.VerticalBoundaryBehaviour;
        data = $"V: vpC:{b.ViewPortCoordinate}, vpD: {b.ViewPortDimension}, sC: {b.ScreenCoordinate}, sD: {b.ScreenDimension}, mvpC{b.MouseViewPortCoordinate}, msC: {b.MouseScreenCoordinate}, lD: {b.LevelDimension}";
        FontBank.MenuFont.RenderText(spriteBatch, data, 10, 1 * MenuFont.GlyphHeight + 20, 1, Color.White);
    }

    private RenderTarget2D GetLevelRenderTarget2D()
    {
        return new RenderTarget2D(
            _graphicsDevice,
            _viewport.HorizontalBoundaryBehaviour.LevelDimension,
            _viewport.VerticalBoundaryBehaviour.LevelDimension,
            false,
            _graphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);
    }

    public void OnWindowSizeChanged()
    {
        DisposableHelperMethods.DisposeOf(ref _levelRenderTarget);
        _levelRenderTarget = GetLevelRenderTarget2D();
    }

    public void AddLemmingRenderer(LemmingRenderer lemmingRenderer)
    {
        lemmingRenderer.RendererId = _orderedSprites.Count;
        _orderedSprites.Add(lemmingRenderer);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        DisposableHelperMethods.DisposeOf(ref _backgroundRenderer);
        DisposableHelperMethods.DisposeOfAll<IViewportObjectRenderer>(CollectionsMarshal.AsSpan(_orderedSprites));

        _disposed = true;
    }

    public int NumberOfItems => _orderedSprites.Count;
    public int Hash(IViewportObjectRenderer item) => item.RendererId;
    public IViewportObjectRenderer UnHash(int index) => _orderedSprites[index];
}