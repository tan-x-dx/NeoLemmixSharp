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

    public int ScaleMultiplier => _scaleMultiplier;

    public bool MouseIsInLevelViewPort { get; private set; }

    public void SetWindowDimensions(int gameWindowWidth, int gameWindowHeight, int controlPanelHeight)
    {
        _windowWidth = gameWindowWidth;
        _windowHeight = gameWindowHeight;
        _controlPanelHeight = controlPanelHeight;

        LevelScreen.HorizontalBoundaryBehaviour.UpdateScreenDimension(_windowWidth, _scaleMultiplier);
        LevelScreen.VerticalBoundaryBehaviour.UpdateScreenDimension(_windowHeight - _controlPanelHeight, _scaleMultiplier);
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
        else if (mousePosition.X == _windowWidth - 1)
        {
            horizontalBoundaryBehaviour.Scroll(_scrollDelta);
        }

        if (mousePosition.Y == 0)
        {
            verticalBoundaryBehaviour.Scroll(-_scrollDelta);
        }
        else if (mousePosition.Y == _windowHeight - 1)
        {
            verticalBoundaryBehaviour.Scroll(_scrollDelta);
        }

        horizontalBoundaryBehaviour.UpdateMouseCoordinate(mousePosition.X);
        verticalBoundaryBehaviour.UpdateMouseCoordinate(mousePosition.Y);
    }

    private bool MouseIsInLevelViewport(LevelInputController inputController)
    {
        var mousePosition = inputController.MousePosition;
        return (uint)mousePosition.X <= (uint)_windowWidth &&
               (uint)mousePosition.Y <= (uint)(_windowHeight - _controlPanelHeight);
    }

    private void TrackScrollWheel(LevelInputController inputController)
    {
        var previousValue = _scaleMultiplier;
        _scaleMultiplier = Math.Clamp(_scaleMultiplier + inputController.ScrollDelta, MinScaleMultiplier, MaxScaleMultiplier);

        if (_scaleMultiplier == previousValue)
            return;

        LevelScreen.HorizontalBoundaryBehaviour.UpdateScreenDimension(_windowWidth, _scaleMultiplier);
        LevelScreen.VerticalBoundaryBehaviour.UpdateScreenDimension(_windowHeight - _controlPanelHeight, _scaleMultiplier);

        _scrollDelta = ScrollSpeedMultiplier * MaxScaleMultiplier / _scaleMultiplier;
    }
}