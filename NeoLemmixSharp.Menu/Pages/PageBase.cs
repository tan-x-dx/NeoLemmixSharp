using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.Pages;

public abstract class PageBase : IInitialisable, IDisposable
{
    protected readonly MenuController InputController;

    protected bool IsInitialised { get; private set; }
    protected bool IsDisposed { get; private set; }

    public UiHandler UiHandler { get; }

    protected PageBase(MenuController menuInputController)
    {
        InputController = menuInputController;
        UiHandler = new UiHandler(menuInputController.InputHandler);
    }

    public void Initialise()
    {
        if (IsInitialised)
            return;

        UiHandler.Instance = UiHandler;

        OnInitialise();
        IsInitialised = true;
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
        if (!IsDisposed)
        {
            IsDisposed = true;

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
