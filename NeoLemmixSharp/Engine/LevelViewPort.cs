using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NeoLemmixSharp.Engine;

public sealed class LevelViewPort
{
    private const int MinScale = 1;
    private const int MaxScale = 16;

    private int _previousScrollWheelValue;

    private int _windowWidth;
    private int _windowHeight;

    private int _pixelWidth;
    private int _pixelHeight;

    private int _sourceX;
    private int _sourceY;

    public int ScaleMultiplier { get; private set; } = 6;

    private int InverseScale => 1 + MaxScale - ScaleMultiplier;

    public Rectangle DestinationBounds => new(0, 0, _windowWidth, _windowHeight);
    public Rectangle SourceBounds => new(_sourceX, _sourceY, _pixelWidth, _pixelHeight);

    public void HandleMouseInput(MouseState mouseState)
    {
        TrackScrollWheel(mouseState.ScrollWheelValue);

        if (mouseState.X == 0)
        {
            ScrollHorizontally(-4 * MaxScale / ScaleMultiplier);
        }
        else if (mouseState.X == LevelScreen.CurrentLevel!.GameWindow.WindowWidth - 1)
        {
            ScrollHorizontally(4 * MaxScale / ScaleMultiplier);
        }

        if (mouseState.Y == 0)
        {
            ScrollVertically(-4 * MaxScale / ScaleMultiplier);
        }
        else if (mouseState.Y == LevelScreen.CurrentLevel!.GameWindow.WindowHeight - 1)
        {
            ScrollVertically(4 * MaxScale / ScaleMultiplier);
        }
    }

    private void ScrollHorizontally(int dx)
    {
        _sourceX += dx;
        if (_sourceX < 0)
        {
            _sourceX = 0;
        }
        else if (_sourceX + _pixelWidth >= LevelScreen.CurrentLevel!.Width)
        {
            _sourceX = LevelScreen.CurrentLevel.Width - _pixelWidth;
        }
    }

    private void ScrollVertically(int dy)
    {
        _sourceY += dy;
        if (_sourceY < 0)
        {
            _sourceY = 0;
        }
        else if (_sourceY + _pixelHeight >= LevelScreen.CurrentLevel!.Height)
        {
            _sourceY = LevelScreen.CurrentLevel.Height - _pixelHeight;
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
        _pixelWidth = _windowWidth / ScaleMultiplier;
        _pixelHeight = _windowHeight / ScaleMultiplier;
    }

    public bool IsVisible(Rectangle rectangle)
    {
        return true;
    }

    public void SetWindowDimensions(int gameWindowWidth, int gameWindowHeight)
    {
        _windowWidth = gameWindowWidth;
        _windowHeight = gameWindowHeight;

        RecalculateDimensions();
    }
}