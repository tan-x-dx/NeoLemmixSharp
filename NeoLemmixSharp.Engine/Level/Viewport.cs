using NeoLemmixSharp.Common.BoundaryBehaviours;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level;

public sealed class Viewport
{
    private const int MinScale = 1;
    private const int MaxScale = 12;
    private const int ScrollSpeedMultiplier = 2;

    private int _windowWidth;
    private int _windowHeight;
    private int _controlPanelHeight;

    private int _scrollDelta;

    public BoundaryBehaviour HorizontalBoundaryBehaviour { get; }
    public BoundaryBehaviour VerticalBoundaryBehaviour { get; }

    public int ScaleMultiplier { get; private set; } = 6;

    public bool MouseIsInLevelViewPort { get; private set; }

    public Viewport(
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        HorizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        VerticalBoundaryBehaviour = verticalBoundaryBehaviour;

        _scrollDelta = ScrollSpeedMultiplier * MaxScale / ScaleMultiplier;
    }

    public void SetWindowDimensions(int gameWindowWidth, int gameWindowHeight, int controlPanelHeight)
    {
        _windowWidth = gameWindowWidth;
        _windowHeight = gameWindowHeight;
        _controlPanelHeight = controlPanelHeight;

        HorizontalBoundaryBehaviour.UpdateScreenDimension(_windowWidth, ScaleMultiplier);
        HorizontalBoundaryBehaviour.Scroll(0);

        VerticalBoundaryBehaviour.UpdateScreenDimension(_windowHeight - _controlPanelHeight, ScaleMultiplier);
        VerticalBoundaryBehaviour.Scroll(0);
    }

    public void HandleMouseInput(LevelInputController inputController)
    {
        // ScreenMouseX = ScaleMultiplier * ((inputController.MouseX + ScaleMultiplier / 2) / ScaleMultiplier);
        // ScreenMouseY = ScaleMultiplier * ((inputController.MouseY + ScaleMultiplier / 2) / ScaleMultiplier);

        if (MouseIsInLevelViewport(inputController))
        {
            MouseIsInLevelViewPort = true;
            TrackScrollWheel(inputController);
        }
        else
        {
            MouseIsInLevelViewPort = false;
        }

        HorizontalBoundaryBehaviour.UpdateMouseCoordinate(inputController.MouseX);
        VerticalBoundaryBehaviour.UpdateMouseCoordinate(inputController.MouseY);

        // ViewportMouseX = (ScreenMouseX - HorizontalBoundaryBehaviour.ScreenCoordinate) / ScaleMultiplier + HorizontalBoundaryBehaviour.ViewPortCoordinate;
        // ViewportMouseY = (ScreenMouseY - VerticalBoundaryBehaviour.ScreenCoordinate) / ScaleMultiplier + VerticalBoundaryBehaviour.ViewPortCoordinate;

        // ViewportMouseX = HorizontalBoundaryBehaviour.Normalise(ViewportMouseX);
        // ViewportMouseY = VerticalBoundaryBehaviour.Normalise(ViewportMouseY);

        if (inputController.MouseX == 0)
        {
            HorizontalBoundaryBehaviour.Scroll(-_scrollDelta);
        }
        else if (inputController.MouseX == _windowWidth - 1)
        {
            HorizontalBoundaryBehaviour.Scroll(_scrollDelta);
        }

        if (inputController.MouseY == 0)
        {
            VerticalBoundaryBehaviour.Scroll(-_scrollDelta);
        }
        else if (inputController.MouseY == _windowHeight - 1)
        {
            VerticalBoundaryBehaviour.Scroll(_scrollDelta);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MouseIsInLevelViewport(LevelInputController inputController)
    {
        return inputController.MouseX >= 0 && inputController.MouseX <= _windowWidth &&
               inputController.MouseY >= 0 && inputController.MouseY <= _windowHeight - _controlPanelHeight;
    }

    private void TrackScrollWheel(LevelInputController inputController)
    {
        var previousValue = ScaleMultiplier;
        ScaleMultiplier = Math.Clamp(ScaleMultiplier + inputController.ScrollDelta, MinScale, MaxScale);

        if (ScaleMultiplier == previousValue)
            return;

        HorizontalBoundaryBehaviour.UpdateScreenDimension(_windowWidth, ScaleMultiplier);
        HorizontalBoundaryBehaviour.Scroll(0);

        VerticalBoundaryBehaviour.UpdateScreenDimension(_windowHeight - _controlPanelHeight, ScaleMultiplier);
        VerticalBoundaryBehaviour.Scroll(0);

        _scrollDelta = ScrollSpeedMultiplier * MaxScale / ScaleMultiplier;
    }
}