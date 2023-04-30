using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

namespace NeoLemmixSharp.Engine;

public sealed class LevelViewport
{
    private const int MinScale = 1;
    private const int MaxScale = 12;

    private readonly IHorizontalViewPortBehaviour _horizontalViewPortBehaviour;
    private readonly IVerticalViewPortBehaviour _verticalViewPortBehaviour;

    private int _previousScrollWheelValue;

    private int _windowWidth;
    private int _windowHeight;

    private int _scrollDelta;

    public int ScaleMultiplier { get; private set; } = 6;

    public int ViewportMouseX { get; private set; }
    public int ViewportMouseY { get; private set; }
    public int ScreenMouseX { get; private set; }
    public int ScreenMouseY { get; private set; }

    // Raw pixels, one-to-one with game
    public int ViewPortX => _horizontalViewPortBehaviour.ViewPortX;
    public int ViewPortY => _verticalViewPortBehaviour.ViewPortY;

    // Stretched to fit the screen
    public int ScreenX => _horizontalViewPortBehaviour.ScreenX;
    public int ScreenY => _verticalViewPortBehaviour.ScreenY;

    public int NumberOfHorizontalRenderIntervals => _horizontalViewPortBehaviour.NumberOfHorizontalRenderIntervals;
    public int NumberOfVerticalRenderIntervals => _verticalViewPortBehaviour.NumberOfVerticalRenderIntervals;

    public LevelViewport(PixelManager terrain)
    {
        _horizontalViewPortBehaviour = terrain.HorizontalViewPortBehaviour;
        _verticalViewPortBehaviour = terrain.VerticalViewPortBehaviour;

        _scrollDelta = 4 * MaxScale / ScaleMultiplier;
    }

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

    public RenderInterval GetHorizontalRenderInterval(int i) => _horizontalViewPortBehaviour.GetHorizontalRenderInterval(i);
    public RenderInterval GetVerticalRenderInterval(int i) => _verticalViewPortBehaviour.GetVerticalRenderInterval(i);
}