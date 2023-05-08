using NeoLemmixSharp.Engine.LevelBoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;
using NeoLemmixSharp.Util;
using System;

namespace NeoLemmixSharp.Engine;

public sealed class LevelViewport
{
    private const int MinScale = 1;
    private const int MaxScale = 12;

    private readonly LevelCursor _cursor;
    private readonly LevelInputController _controller;

    private readonly IHorizontalViewPortBehaviour _horizontalViewPortBehaviour;
    private readonly IVerticalViewPortBehaviour _verticalViewPortBehaviour;

    private int _windowWidth;
    private int _windowHeight;
    private int _controlPanelHeight;

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

    public LevelViewport(
        PixelManager terrain,
        LevelCursor cursor,
        LevelInputController controller)
    {
        _cursor = cursor;
        _controller = controller;
        _horizontalViewPortBehaviour = terrain.HorizontalViewPortBehaviour;
        _verticalViewPortBehaviour = terrain.VerticalViewPortBehaviour;

        _scrollDelta = 4 * MaxScale / ScaleMultiplier;
    }

    public void SetWindowDimensions(int gameWindowWidth, int gameWindowHeight, int controlPanelHeight)
    {
        _windowWidth = gameWindowWidth;
        _windowHeight = gameWindowHeight;
        _controlPanelHeight = controlPanelHeight;

        _horizontalViewPortBehaviour.RecalculateHorizontalDimensions(ScaleMultiplier, _windowWidth);
        _horizontalViewPortBehaviour.ScrollHorizontally(0);
        _horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
        _verticalViewPortBehaviour.RecalculateVerticalDimensions(ScaleMultiplier, _windowHeight, _controlPanelHeight);
        _verticalViewPortBehaviour.ScrollVertically(0);
        _verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);
    }

    public bool HandleMouseInput()
    {
        ScreenMouseX = _controller.MouseX;
        ScreenMouseY = _controller.MouseY;

        ScreenMouseX -= ScreenMouseX % ScaleMultiplier;
        ScreenMouseY -= ScreenMouseY % ScaleMultiplier;

        bool result;
        if (MouseIsInLevelViewport())
        {
            result = true;
            TrackScrollWheel();

            ViewportMouseX = (ScreenMouseX - _horizontalViewPortBehaviour.ScreenX) / ScaleMultiplier + _horizontalViewPortBehaviour.ViewPortX;
            ViewportMouseY = (ScreenMouseY - _verticalViewPortBehaviour.ScreenY) / ScaleMultiplier + _verticalViewPortBehaviour.ViewPortY;

            ViewportMouseX = _horizontalViewPortBehaviour.NormaliseX(ViewportMouseX);
            ViewportMouseY = _verticalViewPortBehaviour.NormaliseY(ViewportMouseY);
        }
        else
        {
            result = false;

            ViewportMouseX = -500000;
            ViewportMouseY = -500000;
        }

        _cursor.CursorPosition = new LevelPosition(ViewportMouseX, ViewportMouseY);

        if (_controller.MouseX == 0)
        {
            _horizontalViewPortBehaviour.ScrollHorizontally(-_scrollDelta);
            _horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
        }
        else if (_controller.MouseX == _windowWidth - 1)
        {
            _horizontalViewPortBehaviour.ScrollHorizontally(_scrollDelta);
            _horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
        }

        if (_controller.MouseY == 0)
        {
            _verticalViewPortBehaviour.ScrollVertically(-_scrollDelta);
            _verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);
        }
        else if (_controller.MouseY == _windowHeight - 1)
        {
            _verticalViewPortBehaviour.ScrollVertically(_scrollDelta);
            _verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);
        }

        return result;
    }

    private bool MouseIsInLevelViewport()
    {
        return _controller.MouseX >= 0 && _controller.MouseX <= _windowWidth &&
               _controller.MouseY >= 0 && _controller.MouseY <= _windowHeight - _controlPanelHeight;
    }

    private void TrackScrollWheel()
    {
        var scaleMultiplierDelta = (int)_controller.ScrollDelta;

        var previousValue = ScaleMultiplier;
        ScaleMultiplier = Math.Clamp(ScaleMultiplier + scaleMultiplierDelta, MinScale, MaxScale);

        if (ScaleMultiplier == previousValue)
            return;

        _horizontalViewPortBehaviour.RecalculateHorizontalDimensions(ScaleMultiplier, _windowWidth);
        _horizontalViewPortBehaviour.ScrollHorizontally(0);
        _horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
        _verticalViewPortBehaviour.RecalculateVerticalDimensions(ScaleMultiplier, _windowHeight, _controlPanelHeight);
        _verticalViewPortBehaviour.ScrollVertically(0);
        _verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);

        _scrollDelta = 4 * MaxScale / ScaleMultiplier;
    }

    public RenderInterval GetHorizontalRenderInterval(int i) => _horizontalViewPortBehaviour.GetHorizontalRenderInterval(i);
    public RenderInterval GetVerticalRenderInterval(int i) => _verticalViewPortBehaviour.GetVerticalRenderInterval(i);
}