using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Engine;

public sealed class LevelViewport
{
    private const int MinScale = 1;
    private const int MaxScale = 12;

    private readonly LevelCursor _cursor;
    private readonly LevelInputController _controller;

    private readonly IHorizontalViewPortBehaviour _horizontalViewPortBehaviour;
    private readonly IVerticalViewPortBehaviour _verticalViewPortBehaviour;
    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

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
    public int ScreenWidth => _horizontalViewPortBehaviour.ScreenWidth;
    public int ScreenHeight => _verticalViewPortBehaviour.ScreenHeight;

    public int NumberOfHorizontalRenderIntervals => _horizontalViewPortBehaviour.NumberOfHorizontalRenderIntervals;
    public int NumberOfVerticalRenderIntervals => _verticalViewPortBehaviour.NumberOfVerticalRenderIntervals;

    public LevelViewport(
        LevelCursor cursor,
        LevelInputController controller,
        IHorizontalViewPortBehaviour horizontalViewPortBehaviour,
        IVerticalViewPortBehaviour verticalViewPortBehaviour,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _cursor = cursor;
        _controller = controller;
        _horizontalViewPortBehaviour = horizontalViewPortBehaviour;
        _verticalViewPortBehaviour = verticalViewPortBehaviour;
        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;

        _scrollDelta = MaxScale / ScaleMultiplier;
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

        ScreenMouseX = ScaleMultiplier * ((ScreenMouseX + ScaleMultiplier / 2) / ScaleMultiplier);
        ScreenMouseY = ScaleMultiplier * ((ScreenMouseY + ScaleMultiplier / 2) / ScaleMultiplier);

        bool result;
        if (MouseIsInLevelViewport())
        {
            result = true;
            TrackScrollWheel();

            ViewportMouseX = (ScreenMouseX - _horizontalViewPortBehaviour.ScreenX) / ScaleMultiplier + _horizontalViewPortBehaviour.ViewPortX;
            ViewportMouseY = (ScreenMouseY - _verticalViewPortBehaviour.ScreenY) / ScaleMultiplier + _verticalViewPortBehaviour.ViewPortY;

            ViewportMouseX = _horizontalBoundaryBehaviour.NormaliseX(ViewportMouseX);
            ViewportMouseY = _verticalBoundaryBehaviour.NormaliseY(ViewportMouseY);
        }
        else
        {
            result = false;

            ViewportMouseX = -4000;
            ViewportMouseY = -4000;
        }

        _cursor.CursorPosition = new LevelPosition(ViewportMouseX, ViewportMouseY);
        _cursor.CursorOnLevel = result;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MouseIsInLevelViewport()
    {
        return _controller.MouseX >= 0 && _controller.MouseX <= _windowWidth &&
               _controller.MouseY >= 0 && _controller.MouseY <= _windowHeight - _controlPanelHeight;
    }

    private void TrackScrollWheel()
    {
        var previousValue = ScaleMultiplier;
        ScaleMultiplier = Math.Clamp(ScaleMultiplier + _controller.ScrollDelta, MinScale, MaxScale);

        if (ScaleMultiplier == previousValue)
            return;

        _horizontalViewPortBehaviour.RecalculateHorizontalDimensions(ScaleMultiplier, _windowWidth);
        _horizontalViewPortBehaviour.ScrollHorizontally(0);
        _horizontalViewPortBehaviour.RecalculateHorizontalRenderIntervals(ScaleMultiplier);
        _verticalViewPortBehaviour.RecalculateVerticalDimensions(ScaleMultiplier, _windowHeight, _controlPanelHeight);
        _verticalViewPortBehaviour.ScrollVertically(0);
        _verticalViewPortBehaviour.RecalculateVerticalRenderIntervals(ScaleMultiplier);

        _scrollDelta = MaxScale / ScaleMultiplier;
    }

    public RenderInterval GetHorizontalRenderInterval(int i) => _horizontalViewPortBehaviour.GetHorizontalRenderInterval(i);
    public RenderInterval GetVerticalRenderInterval(int i) => _verticalViewPortBehaviour.GetVerticalRenderInterval(i);
}