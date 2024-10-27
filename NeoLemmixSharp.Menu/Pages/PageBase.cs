using Microsoft.Xna.Framework;
using MLEM.Ui;
using MLEM.Ui.Elements;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Menu.Pages;

public abstract class PageBase : IInitialisable, IDisposable
{
    protected readonly MenuInputController InputController;
    protected readonly Element UiRoot;

    private bool _isInitialised;

    protected PageBase(
        MenuInputController inputController)
    {
        UiRoot = new Panel(Anchor.Center, Vector2.One)
        {
            Texture = null
        };
        InputController = inputController;
    }

    public void Initialise()
    {
        if (_isInitialised)
            return;

        IGameWindow.Instance.UiRoot.AddChild(UiRoot);

        OnInitialise();
        _isInitialised = true;
    }

    protected abstract void OnInitialise();

    public void SetWindowDimensions(int windowWidth, int windowHeight)
    {
        UiRoot.Size = new Vector2(windowWidth, windowHeight);

        OnWindowDimensionsChanged(windowWidth, windowHeight);
    }

    protected abstract void OnWindowDimensionsChanged(int windowWidth, int windowHeight);

    public void Tick()
    {
        HandleUserInput();
        OnTick();
    }

    protected abstract void HandleUserInput();

    protected virtual void OnTick() { }

    public void Dispose()
    {
        IGameWindow.Instance.UiRoot.RemoveChildren();

        OnDispose();
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