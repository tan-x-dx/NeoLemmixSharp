using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Engine.LevelPixels;
using NeoLemmixSharp.Rendering.LevelRendering.BackgroundRendering;
using NeoLemmixSharp.Rendering.LevelRendering.ControlPanelRendering;
using NeoLemmixSharp.Rendering2;
using NeoLemmixSharp.Rendering2.Text;

namespace NeoLemmixSharp.Rendering.LevelRendering;

public sealed class LevelRenderer : ScreenRenderer
{
    private readonly LevelViewport _viewport;

    private readonly IBackgroundRenderer _backgroundRenderer;
    private readonly TerrainSprite _terrainSprite;
    private readonly LevelCursorSprite _cursorSprite;
    private readonly ISprite[] _levelSprites;
    private readonly FontBank _fontBank;

    private readonly IControlPanelRenderer _controlPanelRenderer;

    private readonly int _levelWidth;
    private readonly int _levelHeight;

    private string _mouseCoords = string.Empty;

    public LevelRenderer(
        TerrainManager terrain,
        LevelViewport viewport,
        ISprite[] levelSprites,
        UiSpriteBank spriteBank,
        FontBank fontBank,
        IControlPanelRenderer controlPanelRenderer)
    {
        _levelWidth = terrain.Width;
        _levelHeight = terrain.Height;

        _backgroundRenderer = new SolidColourBackgroundRenderer(spriteBank, viewport, new Color(24, 24, 60));
        _viewport = viewport;
        _levelSprites = levelSprites;
     //   _terrainSprite = terrain.TerrainRenderer;
        _cursorSprite = spriteBank.GetSprite<LevelCursorSprite>(SpriteBankTextureNames.LevelCursor);
        _fontBank = fontBank;
        _controlPanelRenderer = controlPanelRenderer;
    }

    public override void RenderScreen(SpriteBatch spriteBatch)
    {
        RenderLevel(spriteBatch);
        RenderControlPanel(spriteBatch);
    }

    public override void OnWindowSizeChanged(int windowWidth, int windowHeight)
    {
        throw new System.NotImplementedException();
    }

    private void RenderLevel(SpriteBatch spriteBatch)
    {
        _backgroundRenderer.RenderBackground(spriteBatch);
        _terrainSprite.Render(spriteBatch);

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
    }
}