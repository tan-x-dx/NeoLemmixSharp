using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Components.Buttons;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class MainPage : PageBase
{
    private readonly TextureButton _playButton;
    private readonly TextureButton _levelSelectButton;

    private readonly TextureButton _groupButton;
    private readonly TextureButton _groupUpButton;

    private readonly TextureButton _configButton;
    private readonly TextureButton _quitButton;

    public MainPage(
        MenuInputController inputController)
        : base(inputController)
    {
        const float menuScaleMultiplier = 2.0f;

        _playButton = new TextureButton(0, 0, MenuSpriteBank.SignPlay)
        {
            ScaleMultiplier = menuScaleMultiplier,
        };
        _playButton.MouseDown.RegisterMouseEvent(PlayButtonClick);

        _levelSelectButton = new TextureButton(0, 0, MenuSpriteBank.SignLevelSelect)
        {
            ScaleMultiplier = menuScaleMultiplier,
        };
        _levelSelectButton.MouseDown.RegisterMouseEvent(LevelSelectButtonClick);

        _configButton = new TextureButton(0, 0, MenuSpriteBank.SignConfig)
        {
            ScaleMultiplier = menuScaleMultiplier,
        };
        _configButton.MouseDown.RegisterMouseEvent(ConfigButtonClick);

        _quitButton = new TextureButton(0, 0, MenuSpriteBank.SignQuit)
        {
            ScaleMultiplier = menuScaleMultiplier,
        };
        _quitButton.MouseDown.RegisterMouseEvent(QuitButtonClick);
    }

    protected override void OnInitialise()
    {
        var root = UiHandler.RootComponent;
        root.IsVisible = false;

        root.AddComponent(_playButton);
        root.AddComponent(_levelSelectButton);

        root.AddComponent(_configButton);
        root.AddComponent(_quitButton);

        OnResize();
    }

    private static void PlayButtonClick(Component _, LevelPosition position)
    {
        var levelStartPage = MenuScreen.Current.MenuPageCreator.CreateLevelStartPage();

        if (levelStartPage is null)
            return;

        MenuScreen.Current.SetNextPage(levelStartPage);
    }

    private static void LevelSelectButtonClick(Component _, LevelPosition position)
    {
        var levelSelectPage = MenuScreen.Current.MenuPageCreator.CreateLevelSelectPage();

        if (levelSelectPage is null)
            return;

        MenuScreen.Current.SetNextPage(levelSelectPage);
    }

    private static void GroupUpButtonClick(Component _, LevelPosition position)
    {
    }

    private static void GroupDownButtonClick(Component _, LevelPosition position)
    {
    }

    private static void ConfigButtonClick(Component _, LevelPosition position)
    {
    }

    private static void QuitButtonClick(Component _, LevelPosition position)
    {
        IGameWindow.Instance.Escape();
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
        OnResize();
    }

    private void OnResize()
    {
        var windowWidth = IGameWindow.Instance.WindowWidth;
        var windowHeight = IGameWindow.Instance.WindowHeight;

        UiHandler.RootComponent.Width = windowWidth;
        UiHandler.RootComponent.Height = windowHeight;

        var deltaX = windowWidth / 8;

        _playButton.Left = deltaX * 2;
        _levelSelectButton.Left = deltaX * 3;

        deltaX = windowWidth / 6;

        _configButton.Left = deltaX * 2;
        _quitButton.Left = deltaX * 3;

        var deltaY = windowHeight / 6;

        _playButton.Top = deltaY * 2;
        _levelSelectButton.Top = deltaY * 2;

        _configButton.Top = deltaY * 3;
        _quitButton.Top = deltaY * 3;
    }

    protected override void HandleUserInput()
    {
        if (InputController.Quit.IsPressed)
        {
            IGameWindow.Instance.Escape();
        }
    }

    protected override void OnDispose()
    {
    }
}