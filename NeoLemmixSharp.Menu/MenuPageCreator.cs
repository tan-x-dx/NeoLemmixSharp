using NeoLemmixSharp.Menu.Pages;

namespace NeoLemmixSharp.Menu;

public sealed class MenuPageCreator
{
    private readonly MenuInputController _inputController;

    public MenuPageCreator(MenuInputController inputController)
    {
        _inputController = inputController;
    }

    public MainPage CreateMainPage()
    {
        return new MainPage(_inputController);
    }

    public LevelStartPage CreateLevelStartPage()
    {
        return new LevelStartPage(_inputController);
    }
}