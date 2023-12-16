using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.Background;
using NeoLemmixSharp.Engine.Rendering.Viewport.Gadget;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class LevelRenderer : IScreenRenderer
{
    public static LevelRenderer Current { get; private set; } = null!;

    private readonly int _levelWidth;
    private readonly int _levelHeight;
    private readonly Level.Viewport _viewport;

    private readonly ControlPanelSpriteBank _controlPanelSpriteBank;
    private readonly LemmingSpriteBank _lemmingSpriteBank;
    private readonly GadgetSpriteBank _gadgetSpriteBank;

    private readonly IBackgroundRenderer _backgroundRenderer;
    private readonly TerrainRenderer _terrainRenderer;
    private readonly IViewportObjectRenderer[] _levelSprites;
    private readonly LevelCursorSprite _cursorSprite;

    private readonly IControlPanelRenderer _controlPanelRenderer;

    public LevelRenderer(
        int levelWidth,
        int levelHeight,
        Level.Viewport viewport,
        IBackgroundRenderer backgroundRenderer,
        TerrainRenderer terrainRenderer,
        IViewportObjectRenderer[] levelSprites,
        LevelCursorSprite levelCursorSprite,
        IControlPanelRenderer controlPanelRenderer,
        ControlPanelSpriteBank controlPanelSpriteBank,
        LemmingSpriteBank lemmingSpriteBank,
        GadgetSpriteBank gadgetSpriteBank)
    {
        _levelWidth = levelWidth;
        _levelHeight = levelHeight;
        _viewport = viewport;

        _backgroundRenderer = backgroundRenderer;
        _terrainRenderer = terrainRenderer;
        _levelSprites = levelSprites;
        _cursorSprite = levelCursorSprite;
        _controlPanelRenderer = controlPanelRenderer;

        _controlPanelSpriteBank = controlPanelSpriteBank;
        _lemmingSpriteBank = lemmingSpriteBank;
        _gadgetSpriteBank = gadgetSpriteBank;

        Current = this;
    }

    public bool IsDisposed { get; set; }

    public void RenderScreen(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp);

        RenderLevel(spriteBatch);
        RenderControlPanel(spriteBatch);

        spriteBatch.End();
    }

    private void RenderLevel(SpriteBatch spriteBatch)
    {
        _backgroundRenderer.RenderBackground(spriteBatch);
        _terrainRenderer.Render(spriteBatch);

        RenderSprites(spriteBatch);
    }

    private void RenderSprites(SpriteBatch spriteBatch)
    {
        var w = _levelWidth * _viewport.ScaleMultiplier;
        var h = _levelHeight * _viewport.ScaleMultiplier;
        var maxX = _viewport.NumberOfHorizontalRenderIntervals;
        var maxY = _viewport.NumberOfVerticalRenderIntervals;

        var dx = _viewport.ScreenX - _viewport.ViewPortX * _viewport.ScaleMultiplier;
        var dy = _viewport.ScreenY - _viewport.ViewPortY * _viewport.ScaleMultiplier;

        var w0 = dx;
        var h0 = dy;

        for (var i = 0; i < maxX; i++)
        {
            var hInterval = _viewport.GetHorizontalRenderInterval(i);
            for (var j = 0; j < maxY; j++)
            {
                var vInterval = _viewport.GetVerticalRenderInterval(j);
                var viewportClip = new Rectangle(hInterval.PixelStart, vInterval.PixelStart, hInterval.PixelLength, vInterval.PixelLength);

                for (var t = 0; t < _levelSprites.Length; t++)
                {
                    var sprite = _levelSprites[t];
                    var spriteClip = sprite.GetSpriteBounds();

                    Rectangle.Intersect(ref viewportClip, ref spriteClip, out var clipIntersection);

                    if (!clipIntersection.IsEmpty)
                    {
                        clipIntersection.X -= spriteClip.X;
                        clipIntersection.Y -= spriteClip.Y;

                        var screenX = (spriteClip.X + clipIntersection.X) * _viewport.ScaleMultiplier + w0;
                        var screenY = (spriteClip.Y + clipIntersection.Y) * _viewport.ScaleMultiplier + h0;

                        sprite.RenderAtPosition(spriteBatch, clipIntersection, screenX, screenY, _viewport.ScaleMultiplier);
                    }
                }

                h0 += h;
            }

            h0 = dy;
            w0 += w;
        }
    }

    private void RenderControlPanel(SpriteBatch spriteBatch)
    {
        _controlPanelRenderer.RenderControlPanel(spriteBatch);
        _cursorSprite.RenderAtPosition(spriteBatch, _viewport.ScreenMouseX, _viewport.ScreenMouseY, _viewport.ScaleMultiplier);
    }

    public void OnWindowSizeChanged()
    {
    }

    public void Dispose()
    {
        _controlPanelSpriteBank.Dispose();
        _lemmingSpriteBank.Dispose();
        _gadgetSpriteBank.Dispose();

        DisposableHelperMethods.DisposeOfAll(new ReadOnlySpan<IViewportObjectRenderer>(_levelSprites));

        Current = null!;
    }
}