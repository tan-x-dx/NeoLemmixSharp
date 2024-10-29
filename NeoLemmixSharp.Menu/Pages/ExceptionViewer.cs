﻿namespace NeoLemmixSharp.Menu.Pages;

public sealed class ExceptionViewer : PageBase
{
    private readonly Exception _exception;

    public ExceptionViewer(MenuInputController inputController, Exception exception) : base(inputController)
    {
        _exception = exception;
    }

    protected override void OnInitialise()
    {
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
    }

    protected override void HandleUserInput()
    {
    }

    protected override void OnDispose()
    {
    }
}