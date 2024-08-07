﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class LevelRenderer : IDisposable, IPerfectHasher<IViewportObjectRenderer>
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Level.Viewport _viewport;
    private readonly SpacialHashGrid<IViewportObjectRenderer> _spriteSpacialHashGrid;

    private readonly List<IViewportObjectRenderer> _orderedSprites;
    private IBackgroundRenderer _backgroundRenderer;

    private RenderTarget2D _levelRenderTarget;
    private bool _disposed;

    public LevelRenderer(
        GraphicsDevice graphicsDevice,
        Level.Viewport viewport,
        List<IViewportObjectRenderer> orderedSprites,
        IBackgroundRenderer backgroundRenderer)
    {
        _graphicsDevice = graphicsDevice;
        _viewport = viewport;

        _orderedSprites = orderedSprites;
        _backgroundRenderer = backgroundRenderer;

        _levelRenderTarget = GetLevelRenderTarget2D();

        _spriteSpacialHashGrid = new SpacialHashGrid<IViewportObjectRenderer>(
            this,
            ChunkSizeType.ChunkSize64,
            viewport.HorizontalBoundaryBehaviour,
            viewport.VerticalBoundaryBehaviour);

        var rendererSpan = CollectionsMarshal.AsSpan(_orderedSprites);
        foreach (var renderer in rendererSpan)
        {
            if (renderer is TerrainRenderer)
            {
                StartRenderingSprite(renderer);
            }

            if (renderer is GadgetLayerRenderer gadgetSprite &&
                gadgetSprite.RenderMode != GadgetRenderMode.NoRender)
            {
                StartRenderingSprite(renderer);
            }
        }
    }

    public bool IsRenderingSprite(IViewportObjectRenderer renderer) => _spriteSpacialHashGrid.IsTrackingItem(renderer);

    public void StartRenderingSprite(IViewportObjectRenderer renderer)
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

    public void StopRenderingSprite(IViewportObjectRenderer renderer)
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

    [SkipLocalsInit]
    private void RenderSprites(SpriteBatch spriteBatch)
    {
        Span<uint> scratchSpaceSpan = stackalloc uint[_spriteSpacialHashGrid.ScratchSpaceSize];

        var horizontalBoundary = _viewport.HorizontalBoundaryBehaviour;
        var verticalBoundary = _viewport.VerticalBoundaryBehaviour;

        var horizontalRenderIntervals = horizontalBoundary.GetRenderIntervals();
        var verticalRenderIntervals = verticalBoundary.GetRenderIntervals();

        foreach (var horizontalRenderInterval in horizontalRenderIntervals)
        {
            foreach (var verticalRenderInterval in verticalRenderIntervals)
            {
                var region = new LevelPositionPair(
                    horizontalRenderInterval.ViewPortStart,
                    verticalRenderInterval.ViewPortStart,
                    horizontalRenderInterval.ViewPortStart + horizontalRenderInterval.ViewPortLength - 1,
                    verticalRenderInterval.ViewPortStart + verticalRenderInterval.ViewPortLength - 1);

                var rendererSet = _spriteSpacialHashGrid.GetAllItemsNearRegion(scratchSpaceSpan, region);

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
        var horizontalRenderIntervals = _viewport.HorizontalBoundaryBehaviour.GetScreenRenderIntervals();
        var verticalRenderIntervals = _viewport.VerticalBoundaryBehaviour.GetScreenRenderIntervals();

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
            _viewport.HorizontalBoundaryBehaviour.LevelLength,
            _viewport.VerticalBoundaryBehaviour.LevelLength,
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
        _spriteSpacialHashGrid.OnNumberOfItemsChanged();
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