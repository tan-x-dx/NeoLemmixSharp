using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.LevelEditor.Components;
using NeoLemmixSharp.Menu.Pages;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.LevelEditor;

public sealed class LevelEditorPage : PageBase
{
    private const int TopPanelHeight = 120;
    private const int LeftPanelWidth = 640;
    private const int BottomPanelHeight = 320;

    private readonly LevelCanvas _levelCanvas;

    private Tab _topPanel;
    private Tab _leftPanel;
    private Tab _bottomPanel;

    public LevelEditorPage(MenuInputController menuInputController) : base(menuInputController)
    {
        _levelCanvas = new LevelCanvas(0, 0, 64, 64);
    }

    protected override void OnInitialise()
    {
        MenuScreen.Instance.MenuScreenRenderer.RenderBackground = false;

        var root = UiHandler.RootComponent;
        root.IsVisible = false;
        root.AddComponent(_levelCanvas);

        BuildLevelEditorUI(root);

        OnResize();
    }

    protected override void OnWindowDimensionsChanged(Size windowSize)
    {
        OnResize();
    }

    private void OnResize()
    {
        var windowSize = IGameWindow.Instance.WindowSize;

        _topPanel.Left = 0;
        _topPanel.Top = 0;
        _topPanel.Width = windowSize.W;
        _topPanel.Height = TopPanelHeight;

        _leftPanel.Left = 0;
        _leftPanel.Top = TopPanelHeight;
        _leftPanel.Width = LeftPanelWidth;
        _leftPanel.Height = windowSize.H - TopPanelHeight;

        _bottomPanel.Left = _leftPanel.Right;
        _bottomPanel.Top = windowSize.H - BottomPanelHeight;
        _bottomPanel.Width = windowSize.W - _leftPanel.Width;
        _bottomPanel.Height = BottomPanelHeight;

        _levelCanvas.Left = _leftPanel.Width;
        _levelCanvas.Top = _topPanel.Height;
        _levelCanvas.Width = windowSize.W - _leftPanel.Width;
        _levelCanvas.Height = windowSize.H - _topPanel.Height - _bottomPanel.Height;
    }

    protected override void HandleUserInput()
    {
        if (InputController.Quit.IsPressed)
        {
            IGameWindow.Instance.Escape();
        }
    }

    protected override void OnDispose()
    {
        MenuScreen.Instance.MenuScreenRenderer.RenderBackground = true;
    }

    private void BuildLevelEditorUI(Component root)
    {
        if (IsInitialised)
            return;

        _topPanel = new Tab(0, 0, 10, TopPanelHeight);
        root.AddComponent(_topPanel);

        _leftPanel = new Tab(0, 0, LeftPanelWidth, 256);
        root.AddComponent(_leftPanel);

        _bottomPanel = new Tab(0, 0, 10, BottomPanelHeight);
        root.AddComponent(_bottomPanel);
    }
}
