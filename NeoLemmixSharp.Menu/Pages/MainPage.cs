using Myra.Graphics2D.UI;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class MainPage : IPage
{
    private readonly MenuInputController _inputController;
    private readonly Panel _panel;

    public MainPage(MenuInputController inputController)
    {
        _inputController = inputController;
    }

    Widget IPage.RootWidget => _panel;

    public void Dispose()
    {

    }
}