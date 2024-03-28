using MGUI.Core.UI;
using MGUI.Shared.Helpers;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Rendering;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class MainPage : PageBase
{
    private readonly MenuInputController _inputController;

    private MGTextureData _logoImage;
    private MGTextureData _playButtonImage;
    private MGTextureData _levelSelectButton;

    private MGTextureData _groupButton;
    private MGTextureData _groupUpButton;
    private MGTextureData _groupDownButton;

    private MGTextureData _configButton;
    private MGTextureData _quitButton;

    public MGTextureData LogoImage
    {
        get => _logoImage;
        private set => this.RaisePropertyChanged(ref _logoImage, value);
    }

    public MGTextureData PlayButtonImage
    {
        get => _playButtonImage;
        private set => this.RaisePropertyChanged(ref _playButtonImage, value);
    }

    public MGTextureData LevelSelectButton
    {
        get => _levelSelectButton;
        private set => this.RaisePropertyChanged(ref _levelSelectButton, value);
    }

    public MGTextureData GroupButton
    {
        get => _groupButton;
        private set => this.RaisePropertyChanged(ref _groupButton, value);
    }

    public MGTextureData GroupUpButton
    {
        get => _groupUpButton;
        private set => this.RaisePropertyChanged(ref _groupUpButton, value);
    }

    public MGTextureData GroupDownButton
    {
        get => _groupDownButton;
        private set => this.RaisePropertyChanged(ref _groupDownButton, value);
    }

    public MGTextureData ConfigButton
    {
        get => _configButton;
        private set => this.RaisePropertyChanged(ref _configButton, value);
    }

    public MGTextureData QuitButton
    {
        get => _quitButton;
        private set => this.RaisePropertyChanged(ref _quitButton, value);
    }

    public MainPage(
        MGDesktop desktop,
        MenuInputController inputController)
        : base(desktop)
    {
        _inputController = inputController;
    }

    protected override void OnInitialise(MGDesktop desktop)
    {
        LogoImage = new MGTextureData(MenuSpriteBank.Logo);
        PlayButtonImage = new MGTextureData(MenuSpriteBank.SignPlay);
        LevelSelectButton = new MGTextureData(MenuSpriteBank.SignLevelSelect);
        GroupButton = new MGTextureData(MenuSpriteBank.SignGroup);
        GroupUpButton = new MGTextureData(MenuSpriteBank.SignGroupUp);
        GroupDownButton = new MGTextureData(MenuSpriteBank.SignGroupDown);
        ConfigButton = new MGTextureData(MenuSpriteBank.SignConfig);
        QuitButton = new MGTextureData(MenuSpriteBank.SignQuit);

        Resources.AddCommand(nameof(PlayButtonClick), PlayButtonClick);

        Show();

        /*   UserInterface.Active.GlobalScale = 2f;

           rootPanel.Anchor = Anchor.Center;

           var logoTexture = MenuSpriteBank.GetTexture(MenuResource.LogoImage);
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
           bottomRowButtonsPanel.AddChild(_quitButton);*/
    }

    /*  private static Image CreateTextureButton(MenuResource menuResource)
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
      }*/

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
        /*if (_inputController.Quit.IsPressed)
        {
            _quitButton!.OnClick.Invoke(_quitButton);
            return;
        }

        if (_inputController.F1.IsPressed)
        {
            _playButton!.OnClick.Invoke(_playButton);
            return;
        }

        if (_inputController.F2.IsPressed)
        {
            _levelSelectButton!.OnClick.Invoke(_levelSelectButton);
            return;
        }

        if (_inputController.F3.IsPressed)
        {
            _configButton!.OnClick.Invoke(_configButton);
        }*/
    }

    private static void PlayButtonClick(MGElement entity)
    {
        var levelStartPage = MenuScreen.Current.MenuPageCreator.CreateLevelStartPage();

        if (levelStartPage is null)
            return;

        MenuScreen.Current.SetNextPage(levelStartPage);
    }

    private void LevelSelectButtonClick(MGElement entity)
    {
        var levelSelectPage = MenuScreen.Current.MenuPageCreator.CreateLevelSelectPage();

        MenuScreen.Current.SetNextPage(levelSelectPage);
    }

    private void GroupUpButtonClick(MGElement entity)
    {
    }

    private void GroupDownButtonClick(MGElement entity)
    {
    }

    private void ConfigButtonClick(MGElement entity)
    {
    }

    private void QuitButtonClick(MGElement entity)
    {
        IGameWindow.Instance.Escape();
    }

    private void HandleMouseInput()
    {
    }

    public override void Dispose()
    {
        var resources = Resources;
        resources.RemoveComand(nameof(PlayButtonClick));
        resources.RemoveComand(nameof(LevelSelectButtonClick));
        resources.RemoveComand(nameof(GroupUpButtonClick));
        resources.RemoveComand(nameof(GroupDownButtonClick));
        resources.RemoveComand(nameof(ConfigButtonClick));
        resources.RemoveComand(nameof(QuitButtonClick));

        /* UserInterface.Active.GlobalScale = 1f;

         DisposeOfEntity(ref _playButton);
         DisposeOfEntity(ref _levelSelectButton);
         DisposeOfEntity(ref _groupButton);
         DisposeOfEntity(ref _groupUpButton);
         DisposeOfEntity(ref _groupDownButton);
         DisposeOfEntity(ref _configButton);
         DisposeOfEntity(ref _quitButton);*/
    }

    /* private static void DisposeOfEntity<T>(ref T? entity)
         where T : Entity
     {
         if (entity is null)
             return;

         entity.OnClick = null;
         entity.OnRightClick = null;
         entity = null;
     }*/
}