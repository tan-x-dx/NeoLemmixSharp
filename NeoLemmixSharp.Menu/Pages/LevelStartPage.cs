using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelStartPage : IPage
{
    private readonly IGameWindow _gameWindow;
    private readonly MenuInputController _inputController;
    private readonly LevelScreen _levelScreen;

    // Panel to cover the entire screen,
    // therefore we can click anywhere to begin the level
    private Panel _backPanel;

    public LevelStartPage(
        IGameWindow gameWindow,
        MenuInputController inputController,
        LevelScreen levelScreen)
    {
        _gameWindow = gameWindow;
        _inputController = inputController;
        _levelScreen = levelScreen;
    }

    public void Initialise(RootPanel rootPanel)
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
    }

    private void GoBack(Entity entity)
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
            GoBack(null!);
            return;
        }

        if (_inputController.Space.IsPressed)
        {
            StartLevel(null!);
            return;
        }
    }

    private void HandleMouseInput()
    {
    }

    private void StartLevel(Entity entity)
    {
        _gameWindow.SetScreen(_levelScreen);
    }

    public void Dispose()
    {
        _backPanel.OnClick = null;
        _backPanel.OnRightClick = null;
    }
}