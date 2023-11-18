using GeonBit.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Pages;
using NeoLemmixSharp.Menu.Rendering;

namespace NeoLemmixSharp.Menu;

public sealed class MenuScreen : IBaseScreen
{
    public static MenuScreen Current { get; private set; } = null!;

    private readonly PageTransition _pageTransition = new(EngineConstants.PageTransitionDurationInFrames);

    private IPage _currentPage;
    private IPage? _nextPage;

    public MenuInputController InputController { get; } = new();
    public MenuPageCreator MenuPageCreator { get; }
    public MenuScreenRenderer MenuScreenRenderer { get; }

    IScreenRenderer IBaseScreen.ScreenRenderer => MenuScreenRenderer;
    public string ScreenTitle => "NeoLemmixSharp";
    public bool IsDisposed { get; private set; }

    public MenuScreen(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch)
    {
        var menuCursorRenderer = new MenuCursorRenderer(InputController);
        MenuScreenRenderer = new MenuScreenRenderer(
            menuCursorRenderer,
            _pageTransition);

        MenuPageCreator = new MenuPageCreator(
            contentManager,
            graphicsDevice,
            spriteBatch,
            InputController);
        _currentPage = MenuPageCreator.CreateMainPage();
        Current = this;
    }

    public void Initialise()
    {
        MenuScreenRenderer.Initialise();
        var userInterface = UserInterface.Active;
        userInterface.Clear();
        _currentPage.Initialise(userInterface.Root);
    }

    public void SetNextPage(IPage page)
    {
        _nextPage = page;
        _pageTransition.BeginTransition();
        InputController.ClearAllKeys();
    }

    public void Tick(GameTime gameTime)
    {
        if (_pageTransition.IsTransitioning)
        {
            HandlePageTransition();

            return;
        }

        UserInterface.Active.Update(gameTime);
        InputController.Tick();

        _currentPage.Tick();

        if (InputController.ToggleFullScreen.IsPressed)
        {
            IGameWindow.Instance.ToggleBorderless();
        }
    }

    private void HandlePageTransition()
    {
        _pageTransition.Tick();

        if (!_pageTransition.IsHalfWayDone)
            return;

        HelperMethods.DisposeOf(ref _currentPage);
        _currentPage = _nextPage!;

        var userInterface = UserInterface.Active;
        userInterface.Clear();
        _currentPage.Initialise(userInterface.Root);

        var windowWidth = IGameWindow.Instance.WindowWidth;
        var windowHeight = IGameWindow.Instance.WindowHeight;

        _currentPage.SetWindowDimensions(windowWidth, windowHeight);
    }

    public void OnWindowSizeChanged()
    {
        var windowWidth = IGameWindow.Instance.WindowWidth;
        var windowHeight = IGameWindow.Instance.WindowHeight;

        _currentPage.SetWindowDimensions(windowWidth, windowHeight);
        MenuScreenRenderer.OnWindowSizeChanged();
    }

    public void Dispose()
    {
        if (IsDisposed)
            return;

        MenuScreenRenderer.Dispose();

        Current = null!;
        IsDisposed = true;
    }
}