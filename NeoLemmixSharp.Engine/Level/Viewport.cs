using NeoLemmixSharp.Common.BoundaryBehaviours;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level;

public sealed class Viewport
{
    private const int MinScaleMultiplier = 1;
    private const int InitialScaleMultiplier = 6;
    private const int MaxScaleMultiplier = 12;
    private const int ScrollSpeedMultiplier = 2;

    private int _windowWidth;
    private int _windowHeight;
    private int _controlPanelHeight;

    private int _scaleMultiplier = InitialScaleMultiplier;
    private int _scrollDelta = ScrollSpeedMultiplier * MaxScaleMultiplier / InitialScaleMultiplier;

    public BoundaryBehaviour HorizontalBoundaryBehaviour { get; }
    public BoundaryBehaviour VerticalBoundaryBehaviour { get; }

    public int ScaleMultiplier => _scaleMultiplier;

    public bool MouseIsInLevelViewPort { get; private set; }

    public Viewport(
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        HorizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        VerticalBoundaryBehaviour = verticalBoundaryBehaviour;
    }

    public void SetWindowDimensions(int gameWindowWidth, int gameWindowHeight, int controlPanelHeight)
    {
        _windowWidth = gameWindowWidth;
        _windowHeight = gameWindowHeight;
        _controlPanelHeight = controlPanelHeight;

        HorizontalBoundaryBehaviour.UpdateScreenDimension(_windowWidth, _scaleMultiplier);

        VerticalBoundaryBehaviour.UpdateScreenDimension(_windowHeight - _controlPanelHeight, _scaleMultiplier);
    }

    public void HandleMouseInput(LevelInputController inputController)
    {
        if (MouseIsInLevelViewport(inputController))
        {
            MouseIsInLevelViewPort = true;
            TrackScrollWheel(inputController);
        }
        else
        {
            MouseIsInLevelViewPort = false;
        }

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

        HorizontalBoundaryBehaviour.UpdateMouseCoordinate(inputController.MouseX);
        VerticalBoundaryBehaviour.UpdateMouseCoordinate(inputController.MouseY);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MouseIsInLevelViewport(LevelInputController inputController)
    {
        return inputController.MouseX >= 0 && inputController.MouseX <= _windowWidth &&
               inputController.MouseY >= 0 && inputController.MouseY <= _windowHeight - _controlPanelHeight;
    }

    private void TrackScrollWheel(LevelInputController inputController)
    {
        var previousValue = _scaleMultiplier;
        _scaleMultiplier = Math.Clamp(_scaleMultiplier + inputController.ScrollDelta, MinScaleMultiplier, MaxScaleMultiplier);

        if (_scaleMultiplier == previousValue)
            return;

        HorizontalBoundaryBehaviour.UpdateScreenDimension(_windowWidth, _scaleMultiplier);

        VerticalBoundaryBehaviour.UpdateScreenDimension(_windowHeight - _controlPanelHeight, _scaleMultiplier);

        _scrollDelta = ScrollSpeedMultiplier * MaxScaleMultiplier / _scaleMultiplier;
    }
}