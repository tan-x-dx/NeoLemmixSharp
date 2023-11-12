using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelStartPage : IPage
{
    private readonly MenuInputController _inputController;

    public LevelStartPage(MenuInputController inputController)
    {
        _inputController = inputController;
    }

    public void Initialise(RootPanel rootPanel)
    {
        var button = new Button("Back", anchor: Anchor.Center, size: new Vector2(50, 50))
        {
            OnClick = OnClick
        };

        rootPanel.AddChild(button);
    }

    private void OnClick(Entity entity)
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
            OnClick(null);
            return;
        }
    }

    private void HandleMouseInput()
    {
    }

    public void Dispose()
    {
    }
}