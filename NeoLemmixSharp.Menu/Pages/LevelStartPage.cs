using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.GameInput;
using NeoLemmixSharp.Engine.Level;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelStartPage : PageBase
{
    private readonly MenuController _menuController;
    private readonly LevelScreen _levelScreen;

    public LevelStartPage(
        InputHandler inputHandler,
        LevelScreen levelScreen)
        : base(inputHandler)
    {
        _menuController = new MenuController(inputHandler);
        _levelScreen = levelScreen;
    }

    protected override void OnInitialise()
    {
    }

    protected override void OnWindowDimensionsChanged(Size windowSize)
    {
    }

    protected override void HandleUserInput()
    {
        if (_menuController.Quit.IsPressed)
        {
            NavigateToMainMenuPage();
        }

        if (_menuController.Space.IsPressed)
        {
            StartLevel();
            return;
        }
    }

    private void StartLevel()
    {
        IGameWindow.Instance.SetScreen(_levelScreen);
        _levelScreen.Initialise();
    }

    protected override void OnDispose()
    {
    }
}
