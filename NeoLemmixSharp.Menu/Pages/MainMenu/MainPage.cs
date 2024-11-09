using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Components;

namespace NeoLemmixSharp.Menu.Pages.MainMenu;

public sealed class MainPage : PageBase
{
    private readonly TextureButton _playButton;
    private readonly TextureButton _levelSelectButton;

    private readonly TextureButton _groupButton;
    private readonly TextureButton _groupUpButton;
    private readonly TextureButton _groupDownButton;
    //  private MenuFontText _groupName;

    private readonly TextureButton _configButton;
    private readonly TextureButton _quitButton;

    Button button;

    public MainPage(
        MenuInputController inputController,
        ContainerRuntime root)
        : base(inputController, root)
    {
        _playButton = new TextureButton(MainMenuButtonTextureInfo.PlayButtonTextureInfo);
        _playButton.Click += PlayButtonClick;

        _playButton.X = 500;
        _playButton.Y = 500;
    }

    private int _clickCount;
    private int _clickCount2;

    protected override void OnInitialise(ContainerRuntime root)
    {
        root.Children.Add(_playButton);

        button = new Button();
        button.X = 100;
        button.Y = 100;
        button.Width = 150;
        button.Height = 150;
        button.Text = "Aaaa";
        button.Click += (_, _) =>
        {
            _clickCount++;
            button.Text = $"Clicked {_clickCount} times";
        };
        button.Push += Button_Push;

        root.Children.Add(button.Visual);
    }

    private static void Button_Push(object? sender, EventArgs e)
    {
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

    protected override void OnTick()
    {
        button.Text = $"Clicked {_clickCount} times - {_clickCount2++}";
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
        _playButton.Click -= PlayButtonClick;
    }
}