﻿namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelEndPage : PageBase
{
    public LevelEndPage(
        MenuInputController inputController)
        : base(inputController)
    {
    }

    protected override void OnInitialise()
    {
        throw new NotImplementedException();
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
        throw new NotImplementedException();
    }

    protected override void HandleUserInput()
    {
        if (InputController.Quit.IsPressed)
        {
            NavigateToMainMenuPage();
        }
    }

    protected override void OnDispose()
    {
        throw new NotImplementedException();
    }
}