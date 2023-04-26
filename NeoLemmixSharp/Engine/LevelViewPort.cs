using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Text;

namespace NeoLemmixSharp.Engine;

public sealed class LevelViewPort
{
    private const int MinScale = 1;
    private const int MaxScale = 12;

    private readonly SpriteBank _spriteBank;
    private readonly INeoLemmixFont _menuFont;

    private readonly IHorizontalViewPortBehaviour _horizontalViewPortBehaviour;
    private readonly IVerticalViewPortBehaviour _verticalViewPortBehaviour;

    private int _previousScrollWheelValue;

    private int _windowWidth;
    private int _windowHeight;

    private int _scrollDelta;

    private string _mouseCoords = string.Empty;
    public int ViewportMouseX { get; private set; }
    public int ViewportMouseY { get; private set; }
    public int ScreenMouseX { get; private set; }
    public int ScreenMouseY { get; private set; }

    // Raw pixels, one-to-one with game
    /* public int ViewPortX => _horizontalViewPortBehaviour.ViewPortX;
     public int ViewPortY => _verticalViewPortBehaviour.ViewPortY;

     // Stretched to fit the screen
     public int ScreenX => _horizontalViewPortBehaviour.ScreenX;
     public int ScreenY => _verticalViewPortBehaviour.ScreenY;*/

    public LevelViewPort(PixelManager terrain, FontBank fontBank, SpriteBank spriteBank)
    {
        _spriteBank = spriteBank;
        _horizontalViewPortBehaviour = terrain.HorizontalViewPortBehaviour;
        _verticalViewPortBehaviour = terrain.VerticalViewPortBehaviour;

        _scrollDelta = 4 * MaxScale / ScaleMultiplier;

        _menuFont = fontBank.MenuFont;
    }

    public int ScaleMultiplier { get; private set; } = 6;

    public void SetWindowDimensions(int gameWindowWidth, int gameWindowHeight)
    {
        _windowWidth = gameWindowWidth;
        _windowHeight = gameWindowHeight;

        _horizontalViewPortBehaviour.RecalculateHorizontalDimensions(ScaleMultiplier, _windowWidth);
        _horizontalViewPortBehaviour.ScrollHorizontally(0);
        _horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
        _verticalViewPortBehaviour.RecalculateVerticalDimensions(ScaleMultiplier, _windowHeight);
        _verticalViewPortBehaviour.ScrollVertically(0);
        _verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);
    }

    public void HandleMouseInput(MouseState mouseState)
    {
        TrackScrollWheel(mouseState.ScrollWheelValue);

        if (mouseState.X == 0)
        {
            _horizontalViewPortBehaviour.ScrollHorizontally(-_scrollDelta);
            _horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
        }
        else if (mouseState.X == _windowWidth - 1)
        {
            _horizontalViewPortBehaviour.ScrollHorizontally(_scrollDelta);
            _horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
        }

        if (mouseState.Y == 0)
        {
            _verticalViewPortBehaviour.ScrollVertically(-_scrollDelta);
            _verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);
        }
        else if (mouseState.Y == _windowHeight - 1)
        {
            _verticalViewPortBehaviour.ScrollVertically(_scrollDelta);
            _verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);
        }

        ScreenMouseX = mouseState.X;
        ScreenMouseY = mouseState.Y;

        ViewportMouseX = (ScreenMouseX - _horizontalViewPortBehaviour.ScreenX) / ScaleMultiplier + _horizontalViewPortBehaviour.ViewPortX;
        ViewportMouseY = (ScreenMouseY - _verticalViewPortBehaviour.ScreenY) / ScaleMultiplier + _verticalViewPortBehaviour.ViewPortY;
        
        ScreenMouseX -= ScreenMouseX % ScaleMultiplier;
        ScreenMouseY -= ScreenMouseY % ScaleMultiplier;

        ViewportMouseX = _horizontalViewPortBehaviour.NormaliseX(ViewportMouseX);
        ViewportMouseY = _verticalViewPortBehaviour.NormaliseY(ViewportMouseY);

        _mouseCoords = $"({ScreenMouseX},{ScreenMouseY}) - ({ViewportMouseX},{ViewportMouseY})";
    }

    private void TrackScrollWheel(int scrollWheelValue)
    {
        var delta = scrollWheelValue - _previousScrollWheelValue;
        _previousScrollWheelValue = scrollWheelValue;

        if (delta > 0)
        {
            ZoomIn();
        }
        else if (delta < 0)
        {
            ZoomOut();
        }
    }

    private void ZoomIn()
    {
        var previousValue = ScaleMultiplier;
        if (ScaleMultiplier < MaxScale)
        {
            ScaleMultiplier++;
        }
        else
        {
            ScaleMultiplier = MaxScale;
        }

        if (ScaleMultiplier == previousValue)
            return;

        _horizontalViewPortBehaviour.RecalculateHorizontalDimensions(ScaleMultiplier, _windowWidth);
        _horizontalViewPortBehaviour.ScrollHorizontally(0);
        _horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
        _verticalViewPortBehaviour.RecalculateVerticalDimensions(ScaleMultiplier, _windowHeight);
        _verticalViewPortBehaviour.ScrollVertically(0);
        _verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);

        _scrollDelta = 4 * MaxScale / ScaleMultiplier;
    }

    private void ZoomOut()
    {
        var previousValue = ScaleMultiplier;
        if (ScaleMultiplier > MinScale)
        {
            ScaleMultiplier--;
        }
        else
        {
            ScaleMultiplier = MinScale;
        }

        if (ScaleMultiplier == previousValue)
            return;

        _horizontalViewPortBehaviour.RecalculateHorizontalDimensions(ScaleMultiplier, _windowWidth);
        _horizontalViewPortBehaviour.ScrollHorizontally(0);
        _horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
        _verticalViewPortBehaviour.RecalculateVerticalDimensions(ScaleMultiplier, _windowHeight);
        _verticalViewPortBehaviour.ScrollVertically(0);
        _verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);

        _scrollDelta = 4 * MaxScale / ScaleMultiplier;
    }

    public void RenderTerrain(SpriteBatch spriteBatch, Texture2D texture)
    {
        for (var i = 0; i < _horizontalViewPortBehaviour.NumberOfHorizontalRenderIntervals; i++)
        {
            var hInterval = _horizontalViewPortBehaviour.GetHorizontalRenderInterval(i);
            for (var j = 0; j < _verticalViewPortBehaviour.NumberOfVerticalRenderIntervals; j++)
            {
                var vInterval = _verticalViewPortBehaviour.GetVerticalRenderInterval(j);
                var sourceRect = new Rectangle(hInterval.PixelStart, vInterval.PixelStart, hInterval.PixelLength, vInterval.PixelLength);
                var screenRect = new Rectangle(hInterval.ScreenStart, vInterval.ScreenStart, hInterval.ScreenLength, vInterval.ScreenLength);

                spriteBatch.Draw(texture, screenRect, sourceRect, Color.White);
            }
        }

        _menuFont.RenderText(spriteBatch, _mouseCoords, 20, 20);
    }

    public void RenderSprite(SpriteBatch spriteBatch, ISprite sprite)
    {
        var spriteLocation = sprite.GetLocationRectangle();

        var x0 = (spriteLocation.X - _horizontalViewPortBehaviour.ViewPortX) * ScaleMultiplier + _horizontalViewPortBehaviour.ScreenX;
        var y0 = (spriteLocation.Y - _verticalViewPortBehaviour.ViewPortY) * ScaleMultiplier + _verticalViewPortBehaviour.ScreenY;

        var y1 = y0;
        var w = _horizontalViewPortBehaviour.LevelWidthInPixels * ScaleMultiplier;
        var h = _verticalViewPortBehaviour.LevelHeightInPixels * ScaleMultiplier;

        for (var i = 0; i < _horizontalViewPortBehaviour.NumberOfHorizontalRenderIntervals; i++)
        {
            var hInterval = _horizontalViewPortBehaviour.GetHorizontalRenderInterval(i);
            if (hInterval.Overlaps(spriteLocation.X, spriteLocation.Width))
            {
                for (var j = 0; j < _verticalViewPortBehaviour.NumberOfVerticalRenderIntervals; j++)
                {
                    var vInterval = _verticalViewPortBehaviour.GetVerticalRenderInterval(j);
                    if (vInterval.Overlaps(spriteLocation.Y, spriteLocation.Height))
                    {
                        sprite.RenderAtPosition(spriteBatch, x0, y1, ScaleMultiplier);
                    }

                    y1 += h;
                }
            }

            x0 += w;
            y1 = y0;
        }
    }

    public void RenderCursor(SpriteBatch spriteBatch, ISprite cursorSprite)
    {
        cursorSprite.RenderAtPosition(spriteBatch, ScreenMouseX, ScreenMouseY, ScaleMultiplier);
    }
}