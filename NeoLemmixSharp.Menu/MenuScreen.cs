using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
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
    public MenuScreenRenderer MenuScreenRenderer { get; }

    public IGameWindow GameWindow { get; set; }
    IScreenRenderer IBaseScreen.ScreenRenderer => MenuScreenRenderer;
    public string ScreenTitle => "NeoLemmixSharp";
    public bool IsDisposed { get; private set; }

    public MenuScreen(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        FontBank fontBank)
    {
        var menuCursorRenderer = new MenuCursorRenderer(graphicsDevice, InputController);
        MenuScreenRenderer = new MenuScreenRenderer(
            contentManager,
            graphicsDevice,
            spriteBatch,
            fontBank,
            menuCursorRenderer,
            _pageTransition);

        _currentPage = new MainPage(contentManager, InputController, MenuScreenRenderer.BackgroundBrush);

        Current = this;
    }

    public void Initialise()
    {
        MenuScreenRenderer.Initialise();
        MenuScreenRenderer.SetPage(_currentPage);
    }

    public void SetNextPage(IPage page)
    {
        _nextPage = page;
        _pageTransition.BeginTransition();
        InputController.MiddleMouseButtonAction
    }

    public void Tick()
    {
        if (_pageTransition.IsTransitioning)
        {
            _pageTransition.Tick();

            if (_pageTransition.IsHalfWayDone)
            {
                HelperMethods.DisposeOf(ref _currentPage);
                _currentPage = _nextPage!;
                MenuScreenRenderer.SetPage(_nextPage!);
            }

            return;
        }

        InputController.Tick();

        _currentPage.Tick();
    }

    public void OnWindowSizeChanged()
    {
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