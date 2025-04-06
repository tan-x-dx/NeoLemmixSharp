using NeoLemmixSharp.Common;
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

    protected override void OnWindowDimensionsChanged(Size windowSize)
    {
        OnResize();
    }

    private void OnResize()
    {
        const int margin = 64;

        var windowSize = IGameWindow.Instance.WindowSize;

        var rootComponent = UiHandler.RootComponent;
        rootComponent.Left = margin;
        rootComponent.Top = margin;
        rootComponent.Width = windowSize.W - margin * 2;
        rootComponent.Height = windowSize.H - margin * 2;

        _levelList.SetDimensions(
            margin * 2,
            margin * 2,
            windowSize.W / 2 - margin * 4,
            windowSize.H - margin * 4);
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
