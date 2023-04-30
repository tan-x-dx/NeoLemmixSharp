using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Rendering.Text;

namespace NeoLemmixSharp.Rendering.LevelRendering;

public sealed class LevelRenderer : ScreenRenderer
{
    private readonly LevelViewport _viewport;

    private readonly ISprite[] _levelSprites;
    private readonly SpriteBank _spriteBank;
    private readonly FontBank _fontBank;

    private readonly int _levelWidth, _levelHeight;

    private string _mouseCoords = string.Empty;

    public LevelRenderer(
        PixelManager terrain,
        LevelViewport viewport,
        ISprite[] levelSprites,
        SpriteBank spriteBank,
        FontBank fontBank)
    {
        _levelWidth = terrain.Width;
        _levelHeight = terrain.Height;

        _viewport = viewport;
        _levelSprites = levelSprites;
        _spriteBank = spriteBank;
        _fontBank = fontBank;
    }

    public override void RenderScreen(SpriteBatch spriteBatch)
    {
        _spriteBank.Render(spriteBatch);

        RenderSprites(spriteBatch);

        _spriteBank.LevelCursorSprite.RenderAtPosition(spriteBatch, _viewport.ScreenMouseX, _viewport.ScreenMouseY, _viewport.ScaleMultiplier);

        _mouseCoords = $"({_viewport.ScreenMouseX},{_viewport.ScreenMouseY}) - ({_viewport.ViewportMouseX},{_viewport.ViewportMouseY})";
        _fontBank.MenuFont.RenderText(spriteBatch, _mouseCoords, 20, 20);
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

    public override void Dispose()
    {
    }
}