namespace NeoLemmixSharp.Menu.Pages;

public sealed class ExceptionViewer : PageBase
{
    private readonly Exception _exception;

    public ExceptionViewer(MenuInputController inputController, Exception exception) : base(inputController)
    {
        _exception = exception;
    }

    protected override void OnInitialise()
    {
        throw new NotImplementedException();
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
        throw new NotImplementedException();
    }

    public override void Tick()
    {
        throw new NotImplementedException();
    }

    protected override void OnDispose()
    {
        throw new NotImplementedException();
    }
}