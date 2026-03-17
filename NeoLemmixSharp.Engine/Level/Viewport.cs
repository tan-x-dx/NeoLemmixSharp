using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.GameInput;

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

    public void HandleMouseInput(InputHandler inputHandler)
    {
        if (MouseIsInLevelViewport(inputHandler))
        {
            MouseIsInLevelViewPort = true;
            TrackScrollWheel(inputHandler);
        }
        else
        {
            MouseIsInLevelViewPort = false;
        }

        var horizontalBoundaryBehaviour = LevelScreen.HorizontalBoundaryBehaviour;
        var verticalBoundaryBehaviour = LevelScreen.VerticalBoundaryBehaviour;

        var mousePosition = inputHandler.MousePosition;
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

    private bool MouseIsInLevelViewport(InputHandler inputHandler)
    {
        return _viewportDimensions.EncompassesPoint(inputHandler.MousePosition);
    }

    private void TrackScrollWheel(InputHandler inputHandler)
    {
        var currentValue = _scaleMultiplier;
        var newValue = Math.Clamp(_scaleMultiplier + inputHandler.ScrollDelta, MinScaleMultiplier, MaxScaleMultiplier);

        if (currentValue == newValue)
            return;

        _scaleMultiplier = newValue;

        LevelScreen.HorizontalBoundaryBehaviour.UpdateScreenDimension(_viewportDimensions.W, _scaleMultiplier);
        LevelScreen.VerticalBoundaryBehaviour.UpdateScreenDimension(_viewportDimensions.H, _scaleMultiplier);

        _scrollDelta = ScrollSpeedMultiplier * MaxScaleMultiplier / _scaleMultiplier;
    }
}
