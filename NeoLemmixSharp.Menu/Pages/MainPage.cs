using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Menu.Rendering;
using NeoLemmixSharp.Menu.Widgets;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class MainPage : IPage
{
    private readonly MenuInputController _inputController;

    private Image _playButton;
    private TextureButton _levelSelectButton;

    private TextureButton _groupButton;
    private TextureButton _groupUpButton;
    private TextureButton _groupDownButton;

    private TextureButton _configButton;
    private TextureButton _quitButton;

    public UserInterface UserInterface { get; } = new();

    public MainPage(MenuInputController inputController)
    {
        _inputController = inputController;
    }

    public void Initialise()
    {
        var menuSpriteBank = MenuScreen.Current.MenuSpriteBank;

        _playButton = new Image(menuSpriteBank.GetTexture(MenuResource.SignPlay))
        { 
            OnClick = PlayButtonClick
        };










/*
        _panel = new VerticalStackPanel
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var logo = new Image
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Renderable = new TextureRegion(menuSpriteBank.GetTexture(MenuResource.Logo)),
            Scale = new Vector2(MenuConstants.ScaleFactor, MenuConstants.ScaleFactor)
        };

        var rowOneButtonsPanel = new HorizontalStackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var rowTwoButtonsPanel = new HorizontalStackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center
        };

        _playButton = CreateTextureButton(MenuResource.SignPlay);
        _levelSelectButton = CreateTextureButton(MenuResource.SignLevelSelect);

        var groupPanel = new Panel();
        _groupButton = CreateTextureButton(MenuResource.SignGroup);
        _groupUpButton = new TextureButton(menuSpriteBank.GetTexture(MenuResource.SignGroupUp))
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Left = 11,
            Top = 35
        };
        _groupDownButton = new TextureButton(menuSpriteBank.GetTexture(MenuResource.SignGroupDown))
        {
            Left = 11,
            Top = 53
        };
        _groupName = new MenuFontText("abcd", MenuFont.DefaultColor)
        {
            Left = 14,
            Top = 68
        };

        groupPanel.Widgets.Add(_groupButton);
        groupPanel.Widgets.Add(_groupUpButton);
        groupPanel.Widgets.Add(_groupDownButton);
        groupPanel.Widgets.Add(_groupName);
        AddToStackPanel(rowOneButtonsPanel, _playButton, 2);
        AddToStackPanel(rowOneButtonsPanel, _levelSelectButton, 2);
        AddToStackPanel(rowOneButtonsPanel, groupPanel, 2);

        _configButton = CreateTextureButton(MenuResource.SignConfig);
        _quitButton = CreateTextureButton(MenuResource.SignQuit);

        AddToStackPanel(rowTwoButtonsPanel, _configButton, 2);
        AddToStackPanel(rowTwoButtonsPanel, _quitButton, 2);

        AddToStackPanel(_panel, logo, 3);
        AddToStackPanel(_panel, rowOneButtonsPanel, 3);
        AddToStackPanel(_panel, rowTwoButtonsPanel, 3);


        var b = new Button
        {
            Content = new Label
            {
                Text = "AAAAA"
            }
        };
        AddToStackPanel(rowTwoButtonsPanel, b, 4);


        _playButton.Click += PlayButtonClick;
        _levelSelectButton.Click += LevelSelectButtonClick;
        _groupUpButton.Click += GroupUpButtonClick;
        _groupDownButton.Click += GroupDownButtonClick;
        _configButton.Click += ConfigButtonClick;
        _quitButton.Click += QuitButtonClick;

        b.PressedChanged += PlayButtonClick;

        return;

        TextureButton CreateTextureButton(MenuResource menuResource)
        {
            return new TextureButton(menuSpriteBank.GetTexture(menuResource));
        }

        static void AddToStackPanel(StackPanel stackPanel, Widget widget, int value)
        {
            stackPanel.Widgets.Add(widget);
            StackPanel.SetProportionType(widget, ProportionType.Part);
            StackPanel.SetProportionValue(widget, value);
        }*/
    }

    public void SetWindowDimensions(int windowWidth, int windowHeight)
    {
        UserInterface.ScreenWidth = windowWidth;
        UserInterface.ScreenHeight = windowHeight;
    }

    public void Tick()
    {
        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleKeyboardInput()
    {
        /*  if (_inputController.Quit.IsPressed)
          {
              _quitButton.DoClick();
              return;
          }

          if (_inputController.F1.IsPressed)
          {
              _playButton.DoClick();
              return;
          }

          if (_inputController.F2.IsPressed)
          {
              _levelSelectButton.DoClick();
              return;
          }

          if (_inputController.F3.IsPressed)
          {
              _configButton.DoClick();
              return;
          }*/
    }

    private void PlayButtonClick(Entity entity)
    {
    }

    private void LevelSelectButtonClick(Entity entity)
    {
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
        MenuScreen.Current.GameWindow.Escape();
    }

    private void HandleMouseInput()
    {
    }

    public void Dispose()
    {
        UserInterface.Dispose();
    }
}