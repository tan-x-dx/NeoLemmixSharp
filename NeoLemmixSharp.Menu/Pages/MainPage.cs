using Microsoft.Xna.Framework;
using MLEM.Textures;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Ui.Style;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class MainPage : PageBase
{
    private readonly Button _playButton;
    private readonly Button _levelSelectButton;

    private readonly Button _groupButton;
    private readonly Button _groupUpButton;
    private readonly Button _groupDownButton;
    //  private MenuFontText _groupName;

    private readonly Button _configButton;
    private readonly Button _quitButton;

    public MainPage(MenuInputController inputController) : base(inputController)
    {
        var mainPanel = new Panel(Anchor.Center, UiRoot.Size / 4);
        mainPanel.Texture = null;

        _playButton = new Button(Anchor.AutoCenter, MenuSpriteBank.SignPlay.GetSize() * 2);
        _playButton.Texture = new StyleProp<NinePatch>(new NinePatch(MenuSpriteBank.SignPlay, 0f));
        _playButton.OnPressed += PlayButtonClick;

        _levelSelectButton = new Button(Anchor.AutoCenter, MenuSpriteBank.SignLevelSelect.GetSize() * 2);
        _levelSelectButton.Texture = new StyleProp<NinePatch>(new NinePatch(MenuSpriteBank.SignLevelSelect, 0f));
        _levelSelectButton.OnPressed += LevelSelectButtonClick;

        _configButton = new Button(Anchor.AutoCenter, Vector2.One);
        _quitButton = new Button(Anchor.AutoCenter, Vector2.One);
        _quitButton.OnPressed += QuitButtonClick;

        mainPanel.AddChild(_playButton);
        mainPanel.AddChild(_levelSelectButton);
        UiRoot.AddChild(mainPanel);
    }

    private void PlayButtonClick(Element element)
    {
        var levelStartPage = MenuScreen.Current.MenuPageCreator.CreateLevelStartPage();

        if (levelStartPage is null)
            return;

        MenuScreen.Current.SetNextPage(levelStartPage);
    }

    private void LevelSelectButtonClick(Element element)
    {
    }

    private void GroupUpButtonClick(Element element)
    {
    }

    private void GroupDownButtonClick(Element element)
    {
    }

    private void ConfigButtonClick(Element element)
    {
    }

    private void QuitButtonClick(Element element)
    {
        IGameWindow.Instance.Escape();
    }

    protected override void OnInitialise()
    {
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
    }

    public override void Tick()
    {
        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleKeyboardInput()
    {
        if (InputController.Quit.IsPressed)
        {
            IGameWindow.Instance.Escape();
        }
    }

    private void HandleMouseInput()
    {
    }

    protected override void OnDispose()
    {
        _playButton.OnPressed -= PlayButtonClick;
        _levelSelectButton.OnPressed -= LevelSelectButtonClick;
        _quitButton.OnPressed -= QuitButtonClick;
    }
}