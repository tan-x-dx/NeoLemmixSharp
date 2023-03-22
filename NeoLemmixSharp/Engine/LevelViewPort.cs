using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NeoLemmixSharp.Engine;

public sealed class LevelViewPort
{
    private const int MinScale = 1;
    private const int MaxScale = 16;

    private readonly int _levelWidth;
    private readonly int _levelHeight;

    private int _previousScrollWheelValue;

    private int _windowWidth;
    private int _windowHeight;

    public int SourceX { get; private set; }
    public int SourceY { get; private set; }
    public int SourceWidth { get; private set; }
    public int SourceHeight { get; private set; }

    public int TargetX { get; private set; }
    public int TargetY { get; private set; }
    public int TargetWidth { get; private set; }
    public int TargetHeight { get; private set; }

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
        if (SourceWidth >= _levelWidth)
        {
            SourceX = 0;
            return;
        }

        SourceX += dx;
        if (SourceX < 0)
        {
            SourceX = 0;
        }
        else if (SourceX + SourceWidth >= _levelWidth)
        {
            SourceX = _levelWidth - SourceWidth;
        }
    }

    private void ScrollVertically(int dy)
    {
        if (SourceHeight >= _levelHeight)
        {
            SourceY = 0;
            return;
        }

        SourceY += dy;
        if (SourceY < 0)
        {
            SourceY = 0;
        }
        else if (SourceY + SourceHeight >= _levelHeight)
        {
            SourceY = _levelHeight - SourceHeight;
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
        SourceWidth = _windowWidth / ScaleMultiplier;
        SourceHeight = (_windowHeight - 64) / ScaleMultiplier;

        if (SourceWidth < _levelWidth)
        {
            TargetX = 0;
            TargetWidth = SourceWidth * ScaleMultiplier;
        }
        else
        {
            TargetX = ScaleMultiplier * (SourceWidth - _levelWidth) / 2;
            TargetWidth = _levelWidth * ScaleMultiplier;
            SourceWidth = _levelWidth;
        }

        if (SourceHeight < _levelHeight)
        {
            TargetY = 0;
            TargetHeight = SourceHeight * ScaleMultiplier;
        }
        else
        {
            TargetY = ScaleMultiplier * (SourceHeight - _levelHeight) / 2;
            TargetHeight = _levelHeight * ScaleMultiplier;
            SourceHeight = _levelHeight;
        }
    }

    public bool IsOnScreen(Rectangle rectangle)
    {
        return rectangle.X < SourceX + SourceWidth &&
               SourceX < rectangle.X + rectangle.Width &&
               rectangle.Y < SourceY + SourceHeight &&
               SourceY < rectangle.Y + rectangle.Height;
    }
}