using MGUI.Core.UI;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelWriting;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelStartPage : PageBase
{
    private readonly LevelScreen _levelScreen;
    private readonly LevelData _levelData;

    public LevelStartPage(
        MGDesktop desktop,
        MenuInputController menuInputController,
        LevelScreen levelScreen,
        Engine.LevelBuilding.Data.LevelData levelData)
        : base(desktop, menuInputController)
    {
        _levelScreen = levelScreen;
        _levelData = levelData;
    }

    protected override void OnInitialise()
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

        if (InputController.Enter.IsPressed)
        {
            const string fileName = @"C:\Temp\levelTest.ullv";
            new LevelWriter(_levelData, new Version(1, 0, 0, 0)).WriteToFile(fileName);
        }
    }

    private void HandleMouseInput()
    {
        if (InputController.LeftMouseButtonAction.IsPressed)
        {
            StartLevel();
            return;
        }
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