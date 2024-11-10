using Gum.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum.Forms;
using MonoGameGum.GueDeriving;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Pages;
using NeoLemmixSharp.Menu.Rendering;
using RenderingLibrary;

namespace NeoLemmixSharp.Menu;

public sealed class MenuScreen : IBaseScreen
{
    public static MenuScreen Current { get; private set; } = null!;

    private readonly ContainerRuntime _root = new(true)
    {
        Width = 0,
        Height = 0,
        WidthUnits = DimensionUnitType.RelativeToContainer,
        HeightUnits = DimensionUnitType.RelativeToContainer
    };
    private readonly PageTransition _pageTransition = new();

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
        var menuCursorRenderer = new MenuCursorRenderer(InputController);
        MenuScreenRenderer = new MenuScreenRenderer(
            menuCursorRenderer,
            _pageTransition);

        MenuPageCreator = new MenuPageCreator(
            contentManager,
            graphicsDevice,
            InputController,
            _root);
        _currentPage = MenuPageCreator.CreateMainPage();
        Current = this;
    }

    public void Initialise()
    {
        _root.AddToManagers();

        MenuScreenRenderer.Initialise();

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
        // Update UI
        FormsUtilities.Update(null!, gameTime, _root);
        SystemManagers.Default.Activity(gameTime.TotalGameTime.TotalSeconds);

        if (_pageTransition.IsTransitioning)
        {
            HandlePageTransition();

            return;
        }

        InputController.Tick();
        _currentPage.Tick();

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

        _currentPage.Dispose();
        _root.Children.Clear();
        _root.RemoveFromManagers();

        MenuScreenRenderer.Dispose();

        Current = null!;
        IsDisposed = true;
    }
}