using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;
using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine;

public sealed class LevelViewPort
{
    private const int MinScale = 1;
    private const int MaxScale = 12;

    private readonly IHorizontalViewPortBehaviour _horizontalViewPortBehaviour;
    private readonly IVerticalViewPortBehaviour _verticalViewPortBehaviour;

    private int _previousScrollWheelValue;

    private int _windowWidth;
    private int _windowHeight;

    private int _scrollDelta;

    // Raw pixels, one-to-one with game
    public int ViewPortX => _horizontalViewPortBehaviour.ViewPortX;
    public int ViewPortY => _verticalViewPortBehaviour.ViewPortY;
    public int ViewPortWidth => _horizontalViewPortBehaviour.ViewPortWidth;
    public int ViewPortHeight => _verticalViewPortBehaviour.ViewPortHeight;

    // Stretched to fit the screen
    public int ScreenX => _horizontalViewPortBehaviour.ScreenX;
    public int ScreenY => _verticalViewPortBehaviour.ScreenY;
    public int ScreenWidth => _horizontalViewPortBehaviour.ScreenWidth;
    public int ScreenHeight => _verticalViewPortBehaviour.ScreenHeight;

    public LevelViewPort(PixelManager terrain)
    {
        _horizontalViewPortBehaviour = terrain.HorizontalViewPortBehaviour;
        _verticalViewPortBehaviour = terrain.VerticalViewPortBehaviour;

        _scrollDelta = 4 * MaxScale / ScaleMultiplier;
    }

    public int ScaleMultiplier { get; private set; } = 1;

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

    public bool GetRenderDestinationRectangle(Rectangle rectangle, out Rectangle renderDestination)
    {
        if (rectangle.X < ViewPortX + ViewPortWidth &&
            ViewPortX < rectangle.X + rectangle.Width &&
            rectangle.Y < ViewPortY + ViewPortHeight &&
            ViewPortY < rectangle.Y + rectangle.Height)
        {
            var x0 = (rectangle.X - ViewPortX) * ScaleMultiplier + ScreenX;
            var y0 = (rectangle.Y - ViewPortY) * ScaleMultiplier + ScreenY;

            renderDestination = new Rectangle(
                x0,
                y0,
                rectangle.Width * ScaleMultiplier,
                rectangle.Height * ScaleMultiplier);
            return true;
        }

        renderDestination = Rectangle.Empty;
        return false;
    }

    public void RenderTerrain(SpriteBatch spriteBatch, Texture2D texture)
    {
        var horizontalRenderIntervals = _horizontalViewPortBehaviour.HorizontalRenderIntervals;
        var verticalRenderIntervals = _verticalViewPortBehaviour.VerticalRenderIntervals;

        for (var i = 0; i < horizontalRenderIntervals.Count; i++)
        {
            var hInterval = horizontalRenderIntervals[i];
            for (var j = 0; j < verticalRenderIntervals.Count; j++)
            {
                var vInterval = verticalRenderIntervals[j];
                var sourceRect = new Rectangle(hInterval.PixelStart, vInterval.PixelStart, hInterval.PixelLength, vInterval.PixelLength);
                var screenRect = new Rectangle(hInterval.ScreenStart, vInterval.ScreenStart, hInterval.ScreenLength, vInterval.ScreenLength);

                spriteBatch.Draw(texture, screenRect, sourceRect, Color.White);
            }
        }
    }

    public void RenderSprite(SpriteBatch spriteBatch, IRenderable sprite)
    {
        var textureRectangle = sprite.GetTextureSourceRectangle();
        var texture = sprite.RenderTexture;
        var spriteLocation = sprite.GetLocationRectangle();

        var spriteWidth = spriteLocation.Width * ScaleMultiplier;
        var spriteHeight = spriteLocation.Height * ScaleMultiplier;

        var horizontalRenderIntervals = _horizontalViewPortBehaviour.HorizontalRenderIntervals;
        var verticalRenderIntervals = _verticalViewPortBehaviour.VerticalRenderIntervals;

        var x0 = (spriteLocation.X - ViewPortX) * ScaleMultiplier + ScreenX;
        var y0 = (spriteLocation.Y - ViewPortY) * ScaleMultiplier + ScreenY;

        var w = _horizontalViewPortBehaviour.LevelWidthInPixels * ScaleMultiplier;
        var h = _verticalViewPortBehaviour.LevelHeightInPixels * ScaleMultiplier;

        for (var i = 0; i < horizontalRenderIntervals.Count; i++)
        {
            var hInterval = horizontalRenderIntervals[i];
            if (hInterval.Overlaps(spriteLocation.X, spriteLocation.Width))
            {
                for (var j = 0; j < verticalRenderIntervals.Count; j++)
                {
                    var vInterval = verticalRenderIntervals[j];
                    if (vInterval.Overlaps(spriteLocation.Y, spriteLocation.Height))
                    {
                        var screenRect = new Rectangle(
                            x0,
                            y0,
                            spriteWidth,
                            spriteHeight);

                        spriteBatch.Draw(texture, screenRect, textureRectangle, Color.White);
                    }

                    y0 += h;
                }
            }

            x0 += w;
        }
    }
}