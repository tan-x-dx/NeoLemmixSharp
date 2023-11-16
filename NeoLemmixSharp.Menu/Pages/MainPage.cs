using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Rendering;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class MainPage : IPage
{
    private readonly MenuInputController _inputController;

    private Image _playButton;
    private Image _levelSelectButton;

    private Image _groupButton;
    private Image _groupUpButton;
    private Image _groupDownButton;

    private Image _configButton;
    private Image _quitButton;

    public MainPage(MenuInputController inputController)
    {
        _inputController = inputController;
    }

    public void Initialise(RootPanel rootPanel)
    {
        UserInterface.Active.GlobalScale = 2f;

        rootPanel.Anchor = Anchor.Center;

        var logoTexture = MenuSpriteBank.GetTexture(MenuResource.Logo);
        var logoImage = new Image(logoTexture, logoTexture.GetSize(), anchor: Anchor.AutoCenter, offset: new Vector2(0, 20));
        rootPanel.AddChild(logoImage);

        _playButton = CreateTextureButton(MenuResource.SignPlay);
        _playButton.OnClick = PlayButtonClick;

        _levelSelectButton = CreateTextureButton(MenuResource.SignLevelSelect);
        _levelSelectButton.OnClick = LevelSelectButtonClick;

        _groupButton = CreateTextureButton(MenuResource.SignGroup);

        _groupUpButton = CreateTextureButton(MenuResource.SignGroupUp);
        _groupUpButton.OnClick = GroupUpButtonClick;

        _groupDownButton = CreateTextureButton(MenuResource.SignGroupDown);
        _groupDownButton.OnClick = GroupDownButtonClick;

        _configButton = CreateTextureButton(MenuResource.SignConfig);
        _configButton.OnClick = ConfigButtonClick;

        _quitButton = CreateTextureButton(MenuResource.SignQuit);
        _quitButton.OnClick = QuitButtonClick;

        var topRowButtonsPanelWidth = _playButton.Size.X +
                                      _levelSelectButton.Size.X +
                                      _groupButton.Size.X +
                                      50;

        var topRowButtonsPanel = new Panel(new Vector2(topRowButtonsPanelWidth, -1))
        {
            Anchor = Anchor.AutoCenter,

            FillColor = Color.Transparent,
            OutlineColor = Color.Transparent,
            ShadowColor = Color.Transparent,
        };

        var bottomRowButtonsPanelWidth = _configButton.Size.X +
                                         _quitButton.Size.X +
                                         50;

        var bottomRowButtonsPanel = new Panel(new Vector2(bottomRowButtonsPanelWidth, -1))
        {
            Anchor = Anchor.AutoCenter,

            FillColor = Color.Transparent,
            OutlineColor = Color.Transparent,
            ShadowColor = Color.Transparent,
        };

        rootPanel.AddChild(topRowButtonsPanel);
        rootPanel.AddChild(bottomRowButtonsPanel);

        topRowButtonsPanel.AddChild(_playButton);
        topRowButtonsPanel.AddChild(_levelSelectButton);
        topRowButtonsPanel.AddChild(_groupButton);

        //  rootPanel.AddChild(_groupUpButton);
        //   rootPanel.AddChild(_groupDownButton);
        bottomRowButtonsPanel.AddChild(_configButton);
        bottomRowButtonsPanel.AddChild(_quitButton);

        return;

        Image CreateTextureButton(MenuResource menuResource)
        {
            var texture = MenuSpriteBank.GetTexture(menuResource);

            return new Image(
                texture,
                anchor: Anchor.AutoInlineNoBreak,
                size: texture.GetSize())
            {
                Padding = Vector2.Zero,
                Offset = Vector2.Zero,
                UseActualSizeForCollision = true,
            };
        }
    }

    public void SetWindowDimensions(int windowWidth, int windowHeight)
    {
    }

    public void Tick()
    {
        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleKeyboardInput()
    {
        if (_inputController.Quit.IsPressed)
        {
            _quitButton.OnClick.Invoke(_quitButton);
            return;
        }

        if (_inputController.F1.IsPressed)
        {
            _playButton.OnClick.Invoke(_playButton);
            return;
        }

        if (_inputController.F2.IsPressed)
        {
            _levelSelectButton.OnClick.Invoke(_levelSelectButton);
            return;
        }

        if (_inputController.F3.IsPressed)
        {
            _configButton.OnClick.Invoke(_configButton);
            return;
        }
    }

    private void PlayButtonClick(Entity entity)
    {
        var levelStartPage = MenuScreen.Current.MenuPageCreator.CreateLevelStartPage();

        MenuScreen.Current.SetNextPage(levelStartPage);
    }

    private void LevelSelectButtonClick(Entity entity)
    {
        var levelSelectPage = MenuScreen.Current.MenuPageCreator.CreateLevelSelectPage();

        MenuScreen.Current.SetNextPage(levelSelectPage);
    }

    private void GroupUpButtonClick(Entity entity)
    {
    }

    private void GroupDownButtonClick(Entity entity)
    {
    }

    private void ConfigButtonClick(Entity entity)
    {
    }

    private void QuitButtonClick(Entity entity)
    {
        IGameWindow.Instance.Escape();
    }

    private void HandleMouseInput()
    {
    }

    public void Dispose()
    {
        UserInterface.Active.GlobalScale = 1f;

        _playButton.OnClick = null;
        _levelSelectButton.OnClick = null;
        _groupButton.OnClick = null;
        _groupUpButton.OnClick = null;
        _groupDownButton.OnClick = null;
        _configButton.OnClick = null;
        _quitButton.OnClick = null;
    }
}