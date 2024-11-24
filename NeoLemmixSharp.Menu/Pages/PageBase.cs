using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.Pages;

public abstract class PageBase : IInitialisable, IDisposable
{
    protected readonly MenuInputController InputController;

    private bool _isInitialised;
    private bool _isDisposed;

    public UiHandler UiHandler { get; }

    protected PageBase(
        MenuInputController inputController)
    {
        InputController = inputController;
        UiHandler = new UiHandler(inputController.InputController);
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
            UiHandler.Dispose();

            OnDispose();
            _isDisposed = true;
        }
        GC.SuppressFinalize(this);
    }

    protected abstract void OnDispose();

    protected static void NavigateToMainMenuPage()
    {
        var levelStartPage = MenuScreen.Current.MenuPageCreator.CreateMainPage();

        if (levelStartPage is null)
            return;

        MenuScreen.Current.SetNextPage(levelStartPage);
    }

    protected static Vector2 GetWindowSize() => IGameWindow.Instance.GetWindowSize();
}