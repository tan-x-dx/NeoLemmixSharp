using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NeoLemmixSharp.Engine;

public sealed class LevelViewPort
{
    private const int MinScale = 1;
    private const int MaxScale = 12;

    private readonly int _levelWidth;
    private readonly int _levelHeight;

    private int _previousScrollWheelValue;

    private int _windowWidth;
    private int _windowHeight;
    
    public int ViewPortX { get; private set; }
    public int ViewPortY { get; private set; }
    public int ViewPortWidth { get; private set; }
    public int ViewPortHeight { get; private set; }

    public int ScreenX { get; private set; }
    public int ScreenY { get; private set; }
    public int ScreenWidth { get; private set; }
    public int ScreenHeight { get; private set; }

    public LevelViewPort(int levelWidth, int levelHeight)
    {
        _levelWidth = levelWidth;
        _levelHeight = levelHeight;
    }

    public int ScaleMultiplier { get; private set; } = 6;

    public void SetWindowDimensions(int gameWindowWidth, int gameWindowHeight)
    {
        _windowWidth = gameWindowWidth;
        _windowHeight = gameWindowHeight;

        RecalculateDimensions();
    }

    public void HandleMouseInput(MouseState mouseState)
    {
        TrackScrollWheel(mouseState.ScrollWheelValue);

        if (mouseState.X == 0)
        {
            ScrollHorizontally(-4 * MaxScale / ScaleMultiplier);
        }
        else if (mouseState.X == _windowWidth - 1)
        {
            ScrollHorizontally(4 * MaxScale / ScaleMultiplier);
        }

        if (mouseState.Y == 0)
        {
            ScrollVertically(-4 * MaxScale / ScaleMultiplier);
        }
        else if (mouseState.Y == _windowHeight - 1)
        {
            ScrollVertically(4 * MaxScale / ScaleMultiplier);
        }
    }

    private void ScrollHorizontally(int dx)
    {
        if (ViewPortWidth >= _levelWidth)
        {
            ViewPortX = 0;
            return;
        }

        ViewPortX += dx;
        if (ViewPortX < 0)
        {
            ViewPortX = 0;
        }
        else if (ViewPortX + ViewPortWidth >= _levelWidth)
        {
            ViewPortX = _levelWidth - ViewPortWidth;
        }
    }

    private void ScrollVertically(int dy)
    {
        if (ViewPortHeight >= _levelHeight)
        {
            ViewPortY = 0;
            return;
        }

        ViewPortY += dy;
        if (ViewPortY < 0)
        {
            ViewPortY = 0;
        }
        else if (ViewPortY + ViewPortHeight >= _levelHeight)
        {
            ViewPortY = _levelHeight - ViewPortHeight;
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
        if (ScaleMultiplier >= MaxScale)
        {
            ScaleMultiplier = MaxScale;
        }
        else
        {
            ScaleMultiplier++;
        }

        RecalculateDimensions();
        ScrollHorizontally(0);
        ScrollVertically(0);
    }

    private void ZoomOut()
    {
        if (ScaleMultiplier <= MinScale)
        {
            ScaleMultiplier = MinScale;
        }
        else
        {
            ScaleMultiplier--;
        }

        RecalculateDimensions();
        ScrollHorizontally(0);
        ScrollVertically(0);
    }

    private void RecalculateDimensions()
    {
        ViewPortWidth = _windowWidth / ScaleMultiplier;
        ViewPortHeight = (_windowHeight - 64) / ScaleMultiplier;

        if (ViewPortWidth < _levelWidth)
        {
            ScreenX = 0;
            ScreenWidth = ViewPortWidth * ScaleMultiplier;
        }
        else
        {
            ScreenX = ScaleMultiplier * (ViewPortWidth - _levelWidth) / 2;
            ScreenWidth = _levelWidth * ScaleMultiplier;
            ViewPortWidth = _levelWidth;
        }

        if (ViewPortHeight < _levelHeight)
        {
            ScreenY = 0;
            ScreenHeight = ViewPortHeight * ScaleMultiplier;
        }
        else
        {
            ScreenY = ScaleMultiplier * (ViewPortHeight - _levelHeight) / 2;
            ScreenHeight = _levelHeight * ScaleMultiplier;
            ViewPortHeight = _levelHeight;
        }
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
}