using MGUI.Core.UI;
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
    private readonly MGDesktop _desktop;

    private PageBase _currentPage;
    private PageBase? _nextPage;

    public MenuInputController InputController { get; } = new();
    public MenuPageCreator MenuPageCreator { get; }
    public MenuScreenRenderer MenuScreenRenderer { get; }

    IScreenRenderer IBaseScreen.ScreenRenderer => MenuScreenRenderer;
    public string ScreenTitle => "NeoLemmixSharp";
    public bool IsDisposed { get; private set; }

    public MenuScreen(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        _desktop = new MGDesktop(IGameWindow.Instance.MguiRenderer);

        var menuCursorRenderer = new MenuCursorRenderer(InputController);
        MenuScreenRenderer = new MenuScreenRenderer(
            menuCursorRenderer,
            _pageTransition,
            _desktop);

        MenuPageCreator = new MenuPageCreator(
            contentManager,
            graphicsDevice,
            InputController,
            _desktop);
        _currentPage = MenuPageCreator.CreateMainPage();
        Current = this;
    }

    public void Initialise()
    {
        MenuScreenRenderer.Initialise();

        _desktop.Windows.Clear();
        _currentPage.Initialise();
    }

    public void SetNextPage(PageBase page)
    {
        _nextPage = page;
        _pageTransition.BeginTransition();
        InputController.ClearAllInputActions();
    }

    public void Tick(GameTime gameTime)
    {
        if (_pageTransition.IsTransitioning)
        {
            HandlePageTransition();

            return;
        }

        InputController.Tick();
        _currentPage.Tick();
        _desktop.Update();

        if (InputController.ToggleFullScreen.IsPressed)
        {
            IGameWindow.Instance.ToggleFullscreen();
        }
    }

    private void HandlePageTransition()
    {
        _pageTransition.Tick();

        if (!_pageTransition.IsHalfWayDone)
            return;

        _currentPage.Dispose();
        _currentPage = _nextPage!;

        CloseExceptionViewers();

        _currentPage.Initialise();

        var windowWidth = IGameWindow.Instance.WindowWidth;
        var windowHeight = IGameWindow.Instance.WindowHeight;

        _currentPage.SetWindowDimensions(windowWidth, windowHeight);
    }

    private void CloseExceptionViewers()
    {
        for (var index = _desktop.Windows.Count - 1; index >= 0; index--)
        {
            var window = _desktop.Windows[index];
            if (window.Metadata.ContainsKey(ExceptionViewer.ExceptionWindowProperty))
            {
                window.TryCloseWindow();
            }
        }
    }

    public void OnWindowSizeChanged()
    {
        var windowWidth = IGameWindow.Instance.WindowWidth;
        var windowHeight = IGameWindow.Instance.WindowHeight;

        _currentPage.SetWindowDimensions(windowWidth, windowHeight);
        MenuScreenRenderer.OnWindowSizeChanged();
    }

    public void OnActivated()
    {
    }

    public void OnSetScreen()
    {
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