using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level;

public sealed class Viewport
{
    private const int MinScaleMultiplier = 1;
    private const int InitialScaleMultiplier = 6;
    private const int MaxScaleMultiplier = 12;
    private const int ScrollSpeedMultiplier = 2;

    private Size _windowDimensions;
    private Size _viewportDimensions;

    private int _scaleMultiplier = InitialScaleMultiplier;
    private int _scrollDelta = ScrollSpeedMultiplier * MaxScaleMultiplier / InitialScaleMultiplier;

    public int ScaleMultiplier => _scaleMultiplier;

    public bool MouseIsInLevelViewPort { get; private set; }

    public void SetWindowDimensions(Size gameWindowSize, int controlPanelHeight)
    {
        _windowDimensions = gameWindowSize;
        _viewportDimensions = new Size(gameWindowSize.W, gameWindowSize.H - controlPanelHeight);

        LevelScreen.HorizontalBoundaryBehaviour.UpdateScreenDimension(_viewportDimensions.W, _scaleMultiplier);
        LevelScreen.VerticalBoundaryBehaviour.UpdateScreenDimension(_viewportDimensions.H, _scaleMultiplier);
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

        var horizontalBoundaryBehaviour = LevelScreen.HorizontalBoundaryBehaviour;
        var verticalBoundaryBehaviour = LevelScreen.VerticalBoundaryBehaviour;

        var mousePosition = inputController.MousePosition;
        if (mousePosition.X == 0)
        {
            horizontalBoundaryBehaviour.Scroll(-_scrollDelta);
        }
        else if (mousePosition.X == _windowDimensions.W - 1)
        {
            horizontalBoundaryBehaviour.Scroll(_scrollDelta);
        }

        if (mousePosition.Y == 0)
        {
            verticalBoundaryBehaviour.Scroll(-_scrollDelta);
        }
        else if (mousePosition.Y == _windowDimensions.H - 1)
        {
            verticalBoundaryBehaviour.Scroll(_scrollDelta);
        }

        horizontalBoundaryBehaviour.UpdateMouseCoordinate(mousePosition.X);
        verticalBoundaryBehaviour.UpdateMouseCoordinate(mousePosition.Y);
    }

    private bool MouseIsInLevelViewport(LevelInputController inputController)
    {
        return _viewportDimensions.EncompassesPoint(inputController.MousePosition);
    }

    private void TrackScrollWheel(LevelInputController inputController)
    {
        var currentValue = _scaleMultiplier;
        var newValue = Math.Clamp(_scaleMultiplier + inputController.ScrollDelta, MinScaleMultiplier, MaxScaleMultiplier);

        if (currentValue == newValue)
            return;

        _scaleMultiplier = newValue;

        LevelScreen.HorizontalBoundaryBehaviour.UpdateScreenDimension(_viewportDimensions.W, _scaleMultiplier);
        LevelScreen.VerticalBoundaryBehaviour.UpdateScreenDimension(_viewportDimensions.H, _scaleMultiplier);

        _scrollDelta = ScrollSpeedMultiplier * MaxScaleMultiplier / _scaleMultiplier;
    }
}
