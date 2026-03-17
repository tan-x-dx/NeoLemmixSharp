using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.GameInput;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.Pages;

public abstract class PageBase : IInitialisable, IDisposable
{
    public UiHandler UiHandler { get; }

    private bool _isInitialised;
    private bool _isDisposed;

    protected PageBase(InputHandler inputHandler)
    {
        UiHandler = new UiHandler(inputHandler);
    }

    public void Initialise()
    {
        if (_isInitialised)
            return;

        UiHandler.Instance = UiHandler;

        OnInitialise();
        _isInitialised = true;
    }

    protected abstract void OnInitialise();

    public void SetWindowDimensions(Size windowSize)
    {
        OnWindowDimensionsChanged(windowSize);
    }

    protected abstract void OnWindowDimensionsChanged(Size windowSize);

    public void Tick()
    {
        UiHandler.Tick();
        HandleUserInput();
        OnTick();
    }

    protected abstract void HandleUserInput();

    protected virtual void OnTick() { }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;

            UiHandler.Dispose();
            OnDispose();
        }
        GC.SuppressFinalize(this);
    }

    protected abstract void OnDispose();

    protected static void NavigateToMainMenuPage()
    {
        var levelStartPage = MenuScreen.Instance.MenuPageCreator.CreateMainPage();

        if (levelStartPage is null)
            return;

        MenuScreen.Instance.SetNextPage(levelStartPage);
    }
}
