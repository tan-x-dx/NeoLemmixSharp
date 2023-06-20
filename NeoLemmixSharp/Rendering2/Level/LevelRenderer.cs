using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Rendering2.Level.Ui;
using NeoLemmixSharp.Rendering2.Level.ViewportSprites;
using NeoLemmixSharp.Rendering2.Level.ViewportSprites.BackgroundRendering;
using NeoLemmixSharp.Rendering2.Level.ViewportSprites.Gadgets;
using NeoLemmixSharp.Rendering2.Level.ViewportSprites.LemmingRendering;
using NeoLemmixSharp.Rendering2.Text;

namespace NeoLemmixSharp.Rendering2.Level;

public sealed class LevelRenderer : ScreenRenderer
{
    public static LevelRenderer Current { get; private set; }

    private readonly int _levelWidth;
    private readonly int _levelHeight;
    private readonly LevelViewport _viewport;

    private readonly IBackgroundRenderer _backgroundRenderer;
    private readonly TerrainRenderer _terrainRenderer;
    private readonly ILevelObjectRenderer[] _levelSprites;
    private readonly LevelCursorSprite _cursorSprite;

    private readonly IControlPanelRenderer _controlPanelRenderer;
    private readonly FontBank _fontBank;

    private string _mouseCoords = string.Empty;

    public LemmingSpriteBank LemmingSpriteBank { get; }
    public GadgetSpriteBank GadgetSpriteBank { get; }

    public LevelRenderer(
        int levelWidth,
        int levelHeight,
        LevelViewport viewport,
        IBackgroundRenderer backgroundRenderer,
        TerrainRenderer terrainRenderer,
        ILevelObjectRenderer[] levelSprites,
        LevelCursorSprite levelCursorSprite,
        IControlPanelRenderer controlPanelRenderer,
        LemmingSpriteBank lemmingSpriteBank,
        GadgetSpriteBank gadgetSpriteBank,
        FontBank fontBank)
    {
        _levelWidth = levelWidth;
        _levelHeight = levelHeight;
        _viewport = viewport;

        _backgroundRenderer = backgroundRenderer;
        _terrainRenderer = terrainRenderer;
        _levelSprites = levelSprites;
        _cursorSprite = levelCursorSprite;
        _controlPanelRenderer = controlPanelRenderer;
        _fontBank = fontBank;
        LemmingSpriteBank = lemmingSpriteBank;
        GadgetSpriteBank = gadgetSpriteBank;

        Current = this;
    }

    public override void RenderScreen(SpriteBatch spriteBatch)
    {
        RenderLevel(spriteBatch);
        RenderControlPanel(spriteBatch);
    }

    public override void OnWindowSizeChanged(int windowWidth, int windowHeight)
    {
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

        for (var t = 0; t < _levelSprites.Length; t++)
        {
            var sprite = _levelSprites[t];
            var spriteLocation = sprite.GetLocationRectangle();

            var x0 = spriteLocation.X * _viewport.ScaleMultiplier + dx;
            var y0 = spriteLocation.Y * _viewport.ScaleMultiplier + dy;

            var y1 = y0;
            for (var i = 0; i < maxX; i++)
            {
                var hInterval = _viewport.GetHorizontalRenderInterval(i);
                if (hInterval.Overlaps(spriteLocation.X, spriteLocation.Width))
                {
                    for (var j = 0; j < maxY; j++)
                    {
                        var vInterval = _viewport.GetVerticalRenderInterval(j);
                        if (vInterval.Overlaps(spriteLocation.Y, spriteLocation.Height))
                        {
                            sprite.RenderAtPosition(spriteBatch, x0, y1, _viewport.ScaleMultiplier);
                        }

                        y1 += h;
                    }
                }

                x0 += w;
                y1 = y0;
            }
        }
    }

    private void RenderControlPanel(SpriteBatch spriteBatch)
    {
        _controlPanelRenderer.RenderControlPanel(spriteBatch);
        _cursorSprite.RenderAtPosition(spriteBatch, _viewport.ScreenMouseX, _viewport.ScreenMouseY, _viewport.ScaleMultiplier);

        _mouseCoords = $"({_viewport.ScreenMouseX},{_viewport.ScreenMouseY}) - ({_viewport.ViewportMouseX},{_viewport.ViewportMouseY})";
        _fontBank.MenuFont.RenderText(spriteBatch, _mouseCoords, 20, 20);
    }

    public override void Dispose()
    {

        Current = null;
    }
}