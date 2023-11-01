using Myra.Graphics2D.UI;
using NeoLemmixSharp.Menu.Rendering;
using NeoLemmixSharp.Menu.Rendering.Pages;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class MainPage : IPage
{
    private readonly MenuInputController _inputController;

    public MainPage(
        MenuInputController inputController)
    {
        _inputController = inputController;
    }

    public IPageRenderer GetPageRenderer(MenuSpriteBank menuSpriteBank, Desktop desktop)
    {
        return new MainPageRenderer(menuSpriteBank);
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
            MenuScreen.Current.GameWindow.Escape();
        }
    }

    private void HandleMouseInput()
    {

    }

    public void Dispose()
    {

    }
}