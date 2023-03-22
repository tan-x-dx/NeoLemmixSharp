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

    private int _sourceX;
    private int _sourceY;
    private int _sourceWidth;
    private int _sourceHeight;

    private int _targetX;
    private int _targetY;
    private int _targetWidth;
    private int _targetHeight;

    public LevelViewPort(int levelWidth, int levelHeight)
    {
        _levelWidth = levelWidth;
        _levelHeight = levelHeight;
    }

    public int ScaleMultiplier { get; private set; } = 6;

    public Rectangle DestinationBounds => new(_targetX, _targetY, _targetWidth, _targetHeight);
    public Rectangle SourceBounds => new(_sourceX, _sourceY, _sourceWidth, _sourceHeight);

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
        if (_sourceWidth >= _levelWidth)
        {
            _sourceX = 0;
            return;
        }

        _sourceX += dx;
        if (_sourceX < 0)
        {
            _sourceX = 0;
        }
        else if (_sourceX + _sourceWidth >= _levelWidth)
        {
            _sourceX = _levelWidth - _sourceWidth;
        }
    }

    private void ScrollVertically(int dy)
    {
        if (_sourceHeight >= _levelHeight)
        {
            _sourceY = 0;
            return;
        }

        _sourceY += dy;
        if (_sourceY < 0)
        {
            _sourceY = 0;
        }
        else if (_sourceY + _sourceHeight >= _levelHeight)
        {
            _sourceY = _levelHeight - _sourceHeight;
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
        _sourceWidth = _windowWidth / ScaleMultiplier;
        _sourceHeight = (_windowHeight - 64) / ScaleMultiplier;

        if (_sourceWidth < _levelWidth)
        {
            _targetX = 0;
            _targetWidth = _windowWidth;
        }
        else
        {
            _targetX = ScaleMultiplier * (_sourceWidth - _levelWidth) / 2;
            _targetWidth = _levelWidth * ScaleMultiplier;
            _sourceWidth = _levelWidth;
        }

        if (_sourceHeight < _levelHeight)
        {
            _targetY = 0;
            _targetHeight = _windowHeight - 64;
        }
        else
        {
            _targetY = ScaleMultiplier * (_sourceHeight - _levelHeight) / 2;
            _targetHeight = _levelHeight * ScaleMultiplier;
            _sourceHeight = _levelHeight;
        }
    }

    public bool IsVisible(Rectangle rectangle)
    {
        return true;
    }

}