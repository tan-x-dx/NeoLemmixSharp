using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class ExceptionViewer : PageBase
{
    private readonly Exception _exception;

    public ExceptionViewer(
        InputHandler inputHandler,
        Exception exception)
        : base(inputHandler)
    {
        _exception = exception;
    }

    protected override void OnInitialise()
    {
    }

    protected override void OnWindowDimensionsChanged(Size windowSize)
    {
    }

    protected override void HandleUserInput()
    {
    }

    protected override void OnDispose()
    {
    }
}