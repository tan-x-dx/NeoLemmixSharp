﻿using MGUI.Core.UI;
using MGUI.Shared.Helpers;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Rendering;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class MainPage : PageBase
{
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
        : base(desktop, inputController)
    {
    }

    protected override void OnInitialise()
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
        Resources.AddCommand(nameof(LevelSelectButtonClick), LevelSelectButtonClick);
        Resources.AddCommand(nameof(GroupDownButtonClick), GroupDownButtonClick);
        Resources.AddCommand(nameof(GroupUpButtonClick), GroupUpButtonClick);
        Resources.AddCommand(nameof(ConfigButtonClick), ConfigButtonClick);
        Resources.AddCommand(nameof(QuitButtonClick), QuitButtonClick);
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
            QuitButtonClick(null!);
            return;
        }

        if (InputController.F1.IsPressed)
        {
            PlayButtonClick(null!);
            return;
        }

        if (InputController.F2.IsPressed)
        {
            LevelSelectButtonClick(null!);
            return;
        }

        if (InputController.F3.IsPressed)
        {
            ConfigButtonClick(null!);
        }
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

    protected override void OnDispose()
    {
        LogoImage = new MGTextureData();
        PlayButtonImage = new MGTextureData();
        LevelSelectButton = new MGTextureData();
        GroupButton = new MGTextureData();
        GroupUpButton = new MGTextureData();
        GroupDownButton = new MGTextureData();
        ConfigButton = new MGTextureData();
        QuitButton = new MGTextureData();

        var resources = Resources;
        resources.RemoveCommand(nameof(PlayButtonClick));
        resources.RemoveCommand(nameof(LevelSelectButtonClick));
        resources.RemoveCommand(nameof(GroupUpButtonClick));
        resources.RemoveCommand(nameof(GroupDownButtonClick));
        resources.RemoveCommand(nameof(ConfigButtonClick));
        resources.RemoveCommand(nameof(QuitButtonClick));
    }
}