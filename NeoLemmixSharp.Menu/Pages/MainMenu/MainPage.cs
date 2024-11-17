using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components.Buttons;

namespace NeoLemmixSharp.Menu.Pages.MainMenu;

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
        // _playButton = new TextureButton(MainMenuButtonTextureInfo.PlayButtonTextureInfo, 1);
        // _playButton.Click += PlayButtonClick;

        //_levelSelectButton = new TextureButton(MainMenuButtonTextureInfo.LevelSelectButtonTextureInfo, 1);
        //_levelSelectButton.Click += LevelSelectButtonClick;

        //_configButton = new TextureButton(MainMenuButtonTextureInfo.ConfigButtonTextureInfo, 1);
        //_configButton.Click += ConfigButtonClick;

        //_quitButton = new TextureButton(MainMenuButtonTextureInfo.QuitButtonTextureInfo, 1);
        //_quitButton.Click += QuitButtonClick;
    }

    protected override void OnInitialise()
    {
        //root.Children.Add(_playButton);
        //root.Children.Add(_levelSelectButton);
        //root.Children.Add(_configButton);
        //root.Children.Add(_quitButton);

        OnResize();
    }

    private static void PlayButtonClick(object? sender, EventArgs e)
    {
        var levelStartPage = MenuScreen.Current.MenuPageCreator.CreateLevelStartPage();

        if (levelStartPage is null)
            return;

        MenuScreen.Current.SetNextPage(levelStartPage);
    }

    private static void LevelSelectButtonClick(object? sender, EventArgs e)
    {
        var levelSelectPage = MenuScreen.Current.MenuPageCreator.CreateLevelSelectPage();

        if (levelSelectPage is null)
            return;

        MenuScreen.Current.SetNextPage(levelSelectPage);
    }

    private static void GroupUpButtonClick(object? sender, EventArgs e)
    {
    }

    private static void GroupDownButtonClick(object? sender, EventArgs e)
    {
    }

    private static void ConfigButtonClick(object? sender, EventArgs e)
    {
    }

    private static void QuitButtonClick(object? sender, EventArgs e)
    {
        IGameWindow.Instance.Escape();
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
        OnResize();
    }

    private void OnResize()
    {
        //var windowWidth = IGameWindow.Instance.WindowWidth;
        //var windowHeight = IGameWindow.Instance.WindowHeight;

        //var deltaX = windowWidth / 8;

        //_playButton.X = deltaX * 2;
        //_levelSelectButton.X = deltaX * 3;

        //deltaX = windowWidth / 6;

        //_configButton.X = deltaX * 2;
        //_quitButton.X = deltaX * 3;

        //var deltaY = windowHeight / 6;

        //_playButton.Y = deltaY * 3;
        //_levelSelectButton.Y = deltaY * 3;

        //_configButton.Y = deltaY * 4;
        //_quitButton.Y = deltaY * 4;
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
        //_playButton.Click -= PlayButtonClick;
        //_levelSelectButton.Click -= LevelSelectButtonClick;
        //_configButton.Click -= ConfigButtonClick;
        //_quitButton.Click -= QuitButtonClick;
    }
}