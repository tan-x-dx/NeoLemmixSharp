using NeoLemmixSharp.Common.Util;
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
            ScaleMulitplier = menuScaleMultiplier,
        };
        _playButton.SetClickAction(PlayButtonClick);

        _levelSelectButton = new TextureButton(0, 0, MenuSpriteBank.SignLevelSelect)
        {
            ScaleMulitplier = menuScaleMultiplier,
        };
        _levelSelectButton.SetClickAction(LevelSelectButtonClick);

        _configButton = new TextureButton(0, 0, MenuSpriteBank.SignConfig)
        {
            ScaleMulitplier = menuScaleMultiplier,
        };
        _configButton.SetClickAction(ConfigButtonClick);

        _quitButton = new TextureButton(0, 0, MenuSpriteBank.SignQuit)
        {
            ScaleMulitplier = menuScaleMultiplier,
        };
        _quitButton.SetClickAction(QuitButtonClick);
    }

    protected override void OnInitialise()
    {
        var root = UiHandler.RootComponent;
        root.Visible = false;

        root.AddComponent(_playButton);
        root.AddComponent(_levelSelectButton);

        root.AddComponent(_configButton);
        root.AddComponent(_quitButton);

        OnResize();
    }

    private static void PlayButtonClick()
    {
        var levelStartPage = MenuScreen.Current.MenuPageCreator.CreateLevelStartPage();

        if (levelStartPage is null)
            return;

        MenuScreen.Current.SetNextPage(levelStartPage);
    }

    private static void LevelSelectButtonClick()
    {
        var levelSelectPage = MenuScreen.Current.MenuPageCreator.CreateLevelSelectPage();

        if (levelSelectPage is null)
            return;

        MenuScreen.Current.SetNextPage(levelSelectPage);
    }

    private static void GroupUpButtonClick()
    {
    }

    private static void GroupDownButtonClick()
    {
    }

    private static void ConfigButtonClick()
    {
    }

    private static void QuitButtonClick()
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