using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering.NineSliceRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class LevelRenderer : IDisposable, IPerfectHasher<IViewportObjectRenderer>
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly BoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly BoundaryBehaviour _verticalBoundaryBehaviour;
    private readonly SpacialHashGrid<LevelRenderer, IViewportObjectRenderer> _spriteSpacialHashGrid;
    private readonly uint[] _spriteSetScratchSpace;

    private readonly List<IViewportObjectRenderer> _orderedSprites;

    private IBackgroundRenderer _backgroundRenderer;
    private RenderTarget2D _levelRenderTarget;
    private bool _disposed;

    public LevelRenderer(GraphicsDevice graphicsDevice,
        List<IViewportObjectRenderer> orderedSprites,
        IBackgroundRenderer backgroundRenderer,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        _graphicsDevice = graphicsDevice;

        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;

        _orderedSprites = orderedSprites;

        _spriteSpacialHashGrid = new SpacialHashGrid<LevelRenderer, IViewportObjectRenderer>(
            this,
            ChunkSizeType.ChunkSize64,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        _spriteSetScratchSpace = new uint[_spriteSpacialHashGrid.ScratchSpaceSize];

        var rendererSpan = CollectionsMarshal.AsSpan(_orderedSprites);
        foreach (var renderer in rendererSpan)
        {
            if (renderer is LemmingRenderer) // LemmingRenderers are handled elsewhere
                continue;

            RegisterSpriteForRendering(renderer);
        }

        _backgroundRenderer = backgroundRenderer;
        _levelRenderTarget = GetLevelRenderTarget2D();
    }

    public bool IsRenderingSprite(IViewportObjectRenderer renderer) => _spriteSpacialHashGrid.IsTrackingItem(renderer);

    public void RegisterSpriteForRendering(IViewportObjectRenderer renderer)
    {
        if (!_spriteSpacialHashGrid.IsTrackingItem(renderer))
        {
            _spriteSpacialHashGrid.AddItem(renderer);
        }
    }

    public void UpdateSpritePosition(IViewportObjectRenderer renderer)
    {
        if (_spriteSpacialHashGrid.IsTrackingItem(renderer))
        {
            _spriteSpacialHashGrid.UpdateItemPosition(renderer);
        }
    }

    public void DeregisterSpriteForRendering(IViewportObjectRenderer renderer)
    {
        if (_spriteSpacialHashGrid.IsTrackingItem(renderer))
        {
            _spriteSpacialHashGrid.RemoveItem(renderer);
        }
    }

    public void RenderLevel(SpriteBatch spriteBatch)
    {
        _graphicsDevice.SetRenderTarget(_levelRenderTarget);
        spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

        _backgroundRenderer.RenderBackground(spriteBatch);
        RenderSprites(spriteBatch);

        spriteBatch.End();
    }

    private void RenderSprites(SpriteBatch spriteBatch)
    {
        var scratchSpaceSpan = new Span<uint>(_spriteSetScratchSpace);

        var horizontalBoundary = LevelScreen.HorizontalBoundaryBehaviour;
        var verticalBoundary = LevelScreen.VerticalBoundaryBehaviour;

        var horizontalRenderIntervals = horizontalBoundary.GetRenderIntervals();
        var verticalRenderIntervals = verticalBoundary.GetRenderIntervals();

        foreach (var horizontalRenderInterval in horizontalRenderIntervals)
        {
            foreach (var verticalRenderInterval in verticalRenderIntervals)
            {
                var region = new LevelRegion(
                    horizontalRenderInterval.ViewPortStart,
                    verticalRenderInterval.ViewPortStart,
                    horizontalRenderInterval.ViewPortStart + horizontalRenderInterval.ViewPortLength - 1,
                    verticalRenderInterval.ViewPortStart + verticalRenderInterval.ViewPortLength - 1);

                _spriteSpacialHashGrid.GetAllItemsNearRegion(scratchSpaceSpan, region, out var rendererSet);

                var viewportClip = new Rectangle(
                    horizontalRenderInterval.ViewPortStart,
                    verticalRenderInterval.ViewPortStart,
                    horizontalRenderInterval.ViewPortLength,
                    verticalRenderInterval.ViewPortLength);

                var horizontalViewportClip = new ClipInterval(viewportClip.X, viewportClip.Width, 0);
                var verticalViewportClip = new ClipInterval(viewportClip.Y, viewportClip.Height, 0);

                foreach (var renderer in rendererSet)
                {
                    var spriteClip = renderer.GetSpriteBounds();

                    var horizontalSpriteClip = new ClipInterval(spriteClip.X, spriteClip.Width, 0);
                    var horizontalClipIntersection = horizontalBoundary.GetIntersection(horizontalSpriteClip, horizontalViewportClip);
                    if (horizontalClipIntersection.Length == 0)
                        continue;

                    var verticalSpriteClip = new ClipInterval(spriteClip.Y, spriteClip.Height, 0);
                    var verticalClipIntersection = verticalBoundary.GetIntersection(verticalSpriteClip, verticalViewportClip);
                    if (verticalClipIntersection.Length == 0)
                        continue;

                    var clipIntersection = new Rectangle(
                        horizontalClipIntersection.Start - spriteClip.X,
                        verticalClipIntersection.Start - spriteClip.Y,
                        horizontalClipIntersection.Length,
                        verticalClipIntersection.Length);

                    var projectionX = horizontalClipIntersection.Start +
                                      horizontalClipIntersection.Offset +
                                      horizontalRenderInterval.Offset;
                    var projectionY = verticalClipIntersection.Start +
                                      verticalClipIntersection.Offset +
                                      verticalRenderInterval.Offset;

                    renderer.RenderAtPosition(
                        spriteBatch,
                        clipIntersection,
                        projectionX,
                        projectionY);
                }
            }
        }

        /*
        // Debug code to print a screenshot of the level's render space
        if (LevelScreen.LevelInputController.Space.IsPressed)
        {
            const string fileName = @"C:\Temp\foo.png";
            using (var fileStream = File.Create(fileName))
            {
                _levelRenderTarget.SaveAsPng(fileStream, _levelRenderTarget.Width, _levelRenderTarget.Height);
            }
        }
        */
    }

    [SkipLocalsInit]
    public void DrawToScreen(SpriteBatch spriteBatch)
    {
        var horizontalRenderIntervals = LevelScreen.HorizontalBoundaryBehaviour.GetScreenRenderIntervals();
        var verticalRenderIntervals = LevelScreen.VerticalBoundaryBehaviour.GetScreenRenderIntervals();

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

                spriteBatch.Draw(
                    _levelRenderTarget,
                    destinationRectangle,
                    sourceRectangle,
                    Color.White);
            }
        }

        /*
        var b = _viewport.HorizontalBoundaryBehaviour;
        var data = $"H: vpC:{b.ViewPortStart}, vpD: {b.ViewPortLength}, sC: {b.ScreenStart}, sD: {b.ScreenLength}, " +
                   $"mvpC{b.MouseViewPortCoordinate}, msC: {b.MouseScreenCoordinate}, lD: {b.LevelLength}";
        FontBank.MenuFont.RenderText(spriteBatch, data, 10, 0 * MenuFont.GlyphHeight + 10, 1, Color.White);

        b = _viewport.VerticalBoundaryBehaviour;
        data = $"V: vpC:{b.ViewPortStart}, vpD: {b.ViewPortLength}, sC: {b.ScreenStart}, sD: {b.ScreenLength}, " +
               $"mvpC{b.MouseViewPortCoordinate}, msC: {b.MouseScreenCoordinate}, lD: {b.LevelLength}";
        FontBank.MenuFont.RenderText(spriteBatch, data, 10, 1 * MenuFont.GlyphHeight + 20, 1, Color.White);
        */
    }

    private RenderTarget2D GetLevelRenderTarget2D()
    {
        return new RenderTarget2D(
            _graphicsDevice,
            _horizontalBoundaryBehaviour.LevelLength,
            _verticalBoundaryBehaviour.LevelLength,
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

    int IPerfectHasher<IViewportObjectRenderer>.NumberOfItems => _orderedSprites.Count;
    int IPerfectHasher<IViewportObjectRenderer>.Hash(IViewportObjectRenderer item) => item.RendererId;
    IViewportObjectRenderer IPerfectHasher<IViewportObjectRenderer>.UnHash(int index) => _orderedSprites[index];
}