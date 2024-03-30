using MGUI.Core.UI;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelStartPage : PageBase
{
    private readonly LevelScreen _levelScreen;

    public LevelStartPage(MGDesktop desktop, MenuInputController menuInputController, LevelScreen levelScreen)
        : base(desktop, menuInputController)
    {
        _levelScreen = levelScreen;
    }

    protected override void OnInitialise(MGDesktop desktop)
    {
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
    }

    public override void Tick()
    {
        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleKeyboardInput()
    {
        if (InputController.Quit.IsPressed)
        {
            GoBack();
            return;
        }

        if (InputController.Space.IsPressed)
        {
            StartLevel();
            return;
        }
    }

    private void HandleMouseInput()
    {
    }

    private static void GoBack()
    {
        var mainPage = MenuScreen.Current.MenuPageCreator.CreateMainPage();

        MenuScreen.Current.SetNextPage(mainPage);
    }

    private void StartLevel()
    {
        IGameWindow.Instance.SetScreen(_levelScreen);
    }

    protected override void OnDispose()
    {
        //   _backPanel.OnClick = null;
        //   _backPanel.OnRightClick = null;
    }
}