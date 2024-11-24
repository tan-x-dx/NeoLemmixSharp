using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components.Buttons;

namespace NeoLemmixSharp.Menu.Pages.LevelSelect;

public sealed class LevelSelectPage : PageBase
{
    private readonly LevelList _levelList = new();

    public LevelSelectPage(MenuInputController inputController)
        : base(inputController)
    {
    }

    protected override void OnInitialise()
    {
        UiHandler.RootComponent.AddComponent(new Button(1600, 100, "ABCD"));

        UiHandler.RootComponent.AddComponent(_levelList);

        OnResize();
        _levelList.RefreshLevels();
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
        OnResize();
    }

    private void OnResize()
    {
        const int margin = 64;

        var windowWidth = IGameWindow.Instance.WindowWidth;
        var windowHeight = IGameWindow.Instance.WindowHeight;

        var rootComponent = UiHandler.RootComponent;
        rootComponent.Left = margin;
        rootComponent.Top = margin;
        rootComponent.Width = windowWidth - margin * 2;
        rootComponent.Height = windowHeight - margin * 2;

        _levelList.SetDimensions(
            margin * 2,
            margin * 2,
            windowWidth / 2 - margin * 4,
            windowHeight - margin * 4);
    }

    protected override void HandleUserInput()
    {
        if (InputController.Quit.IsPressed)
        {
            NavigateToMainMenuPage();
        }

        _levelList.HandleUserInput(InputController);
    }

    protected override void OnTick()
    {
        _levelList.Tick();
    }

    protected override void OnDispose()
    {
    }
}
