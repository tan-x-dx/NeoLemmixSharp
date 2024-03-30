using MGUI.Core.UI;
using MGUI.Shared.Helpers;
using System.ComponentModel;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class ExceptionViewer : PageBase
{
    public const string ExceptionWindowProperty = nameof(ExceptionWindowProperty);

    private readonly Exception _exception;

    private string _message = string.Empty;
    private string _stackTrace = string.Empty;

    public string Message
    {
        get => _message;
        set => this.RaisePropertyChanged(ref _message, value);
    }

    public string StackTrace
    {
        get => _stackTrace;
        set => this.RaisePropertyChanged(ref _stackTrace, value);
    }

    public ExceptionViewer(
        MGDesktop desktop,
        MenuInputController inputController,
        Exception exception)
        : base(desktop, inputController)
    {
        _exception = exception;

        Window.IsCloseButtonVisible = true;
        Window.WindowStyle = WindowStyle.Default;
        Window.CanCloseWindow = true;
        Window.Metadata.Add(ExceptionWindowProperty, new object());
        Window.WindowClosing += WindowOnWindowClosing;
    }

    protected override void OnInitialise()
    {
        Message = _exception.Message;
        StackTrace = _exception.StackTrace ?? string.Empty;
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
        Window.WindowWidth = 500;
        Window.WindowHeight = 500;
    }

    public override void Tick()
    {
    }

    private void WindowOnWindowClosing(object? sender, CancelEventArgs e)
    {
        Window.WindowClosing -= WindowOnWindowClosing;
        Dispose();
    }

    protected override void OnDispose()
    {
    }
}