using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelStartPage : IPage
{
    private readonly MenuInputController _inputController;
    private readonly LevelScreen _levelScreen;

    // Panel to cover the entire screen, so
    // we can click anywhere to begin the level
    //private Panel _backPanel;

    public LevelStartPage(
        MenuInputController inputController,
        LevelScreen levelScreen)
    {
        _inputController = inputController;
        _levelScreen = levelScreen;
    }

    /*public void Initialise(RootPanel rootPanel)
    {
        _backPanel = new Panel(Vector2.Zero, anchor: Anchor.Center)
        {
            OnClick = StartLevel,
            OnRightClick = GoBack,
            UseActualSizeForCollision = true,

            OutlineColor = Color.Transparent,
            FillColor = Color.Transparent,
            ShadowColor = Color.Transparent
        };

        rootPanel.AddChild(_backPanel);
    }*/

    private static void GoBack()
    {
        var mainPage = MenuScreen.Current.MenuPageCreator.CreateMainPage();

        MenuScreen.Current.SetNextPage(mainPage);
    }

    public void SetWindowDimensions(int windowWidth, int windowHeight)
    {
    }

    public void Tick()
    {
        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleKeyboardInput()
    {
        if (_inputController.Quit.IsPressed)
        {
            GoBack();
            return;
        }

        if (_inputController.Space.IsPressed)
        {
            StartLevel();
            return;
        }
    }

    private void HandleMouseInput()
    {
    }

    private void StartLevel()
    {
        IGameWindow.Instance.SetScreen(_levelScreen);
    }

    public void Dispose()
    {
        //   _backPanel.OnClick = null;
        //   _backPanel.OnRightClick = null;
    }
}