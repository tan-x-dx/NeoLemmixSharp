﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelStartPage : PageBase
{
    private readonly LevelScreen _levelScreen;

    public LevelStartPage(
        MenuInputController inputController,
        LevelScreen levelScreen)
        : base(inputController)
    {
        _levelScreen = levelScreen;
    }

    protected override void OnInitialise()
    {
    }

    protected override void OnWindowDimensionsChanged(Size windowSize)
    {
    }

    protected override void HandleUserInput()
    {
        if (InputController.Quit.IsPressed)
        {
            NavigateToMainMenuPage();
        }

        if (InputController.Space.IsPressed)
        {
            StartLevel();
            return;
        }
    }

    private void StartLevel()
    {
        IGameWindow.Instance.SetScreen(_levelScreen);
    }

    protected override void OnDispose()
    {
    }
}