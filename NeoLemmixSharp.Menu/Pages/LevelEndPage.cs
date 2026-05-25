using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelEndPage : PageBase
{
    private readonly MenuController _menuController;

    public LevelEndPage(
        InputHandler inputHandler)
        : base(inputHandler)
    {
        _menuController = new MenuController(inputHandler);
    }

    protected override void OnInitialise()
    {
        throw new NotImplementedException();
    }

    protected override void OnWindowDimensionsChanged(Size windowSize)
    {
        throw new NotImplementedException();
    }

    protected override void HandleUserInput()
    {
        if (_menuController.Quit.IsPressed)
        {
            var openMenu = UiHandler.CurrentMenu;
            if (openMenu is null)
            {
                NavigateToMainMenuPage();
            }
            else
            {
                UiHandler.ClosePopupMenu(openMenu);
            }
        }
    }

    protected override void OnDispose()
    {
        throw new NotImplementedException();
    }
}