using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Menu.Rendering;
using NeoLemmixSharp.Menu.Widgets;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class MainPage : IPage
{
    private readonly MenuInputController _inputController;

    private VerticalStackPanel _panel;
    private TextureButton _playButton;
    private TextureButton _levelSelectButton;

    private TextureButton _groupButton;
    private TextureButton _groupUpButton;
    private TextureButton _groupDownButton;
    private MenuFontText _groupName;

    private TextureButton _configButton;
    private TextureButton _quitButton;

    public MainPage(MenuInputController inputController)
    {
        _inputController = inputController;
    }

    public void Initialise()
    {
        var menuSpriteBank = MenuScreen.Current.MenuSpriteBank;

        _panel = new VerticalStackPanel
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        var logo = new Image
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Renderable = new TextureRegion(menuSpriteBank.GetTexture(MenuResource.Logo))
        };

        var rowOneButtonsPanel = new HorizontalStackPanel
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var rowTwoButtonsPanel = new HorizontalStackPanel
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        SetProportion(logo, 3);
        SetProportion(rowOneButtonsPanel, 3);
        SetProportion(rowTwoButtonsPanel, 3);

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
        groupPanel.Width = _groupButton.Width;
        groupPanel.Height = _groupButton.Height;

        rowOneButtonsPanel.Widgets.Add(_playButton);
        rowOneButtonsPanel.Widgets.Add(_levelSelectButton);
        rowOneButtonsPanel.Widgets.Add(groupPanel);

        _configButton = CreateTextureButton(MenuResource.SignConfig);
        _quitButton = CreateTextureButton(MenuResource.SignQuit);

        rowTwoButtonsPanel.Widgets.Add(_configButton);
        rowTwoButtonsPanel.Widgets.Add(_quitButton);

        _panel.Widgets.Add(logo);
        _panel.Widgets.Add(rowOneButtonsPanel);
        _panel.Widgets.Add(rowTwoButtonsPanel);

        SetProportion(_playButton, 3);
        SetProportion(_levelSelectButton, 3);
        SetProportion(groupPanel, 3);

        SetProportion(_configButton, 3);
        SetProportion(_quitButton, 3);

        return;

        TextureButton CreateTextureButton(MenuResource menuResource)
        {
            return new TextureButton(menuSpriteBank.GetTexture(menuResource));
        }

        static void SetProportion(Widget widget, int value)
        {
            StackPanel.SetProportionType(widget, ProportionType.Part);
            StackPanel.SetProportionValue(widget, value);
        }
    }

    public Widget GetRootWidget()
    {
        return _panel;
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
            MenuScreen.Current.GameWindow.Escape();

            return;
        }

        if (_inputController.F1.IsPressed)
        {

            return;
        }

        if (_inputController.F2.IsPressed)
        {

            return;
        }

        if (_inputController.F3.IsPressed)
        {

            return;
        }
    }

    private void HandleMouseInput()
    {
    }

    public void Dispose()
    {

    }
}