﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Pages;
using NeoLemmixSharp.Menu.Rendering;

namespace NeoLemmixSharp.Menu;

public sealed class MenuScreen : IBaseScreen
{
    public static MenuScreen Current { get; private set; } = null!;

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
            InputController);
        _currentPage = MenuPageCreator.CreateMainPage();
        Current = this;
    }

    public void Initialise()
    {
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

        MenuScreenRenderer.Dispose();

        Current = null!;
        IsDisposed = true;
    }
}