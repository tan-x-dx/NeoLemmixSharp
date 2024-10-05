using MLEM.Ui;
using MLEM.Ui.Elements;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Menu.Pages;

public abstract class PageBase : IInitialisable, IDisposable
{
    private readonly UiSystem _uiSystem;
    protected readonly MenuInputController InputController;

    protected Element UiRoot { get; }

    private bool _isInitialised;

    protected PageBase(
        MenuInputController inputController)
    {
        _uiSystem = IGameWindow.Instance.UiSystem;
        UiRoot = IGameWindow.Instance.UiRoot;
        InputController = inputController;
    }

    public void Initialise()
    {
        if (_isInitialised)
            return;

        OnInitialise();
        _isInitialised = true;
    }

    protected abstract void OnInitialise();

    public void SetWindowDimensions(int windowWidth, int windowHeight)
    {
        OnWindowDimensionsChanged(windowWidth, windowHeight);
    }

    protected abstract void OnWindowDimensionsChanged(int windowWidth, int windowHeight);

    public abstract void Tick();

    public void Dispose()
    {
        OnDispose();
    }

    protected abstract void OnDispose();
}