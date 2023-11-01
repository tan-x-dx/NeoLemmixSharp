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

    public MenuSpriteBank MenuSpriteBank { get; }
    public MenuInputController InputController { get; } = new();
    public MenuScreenRenderer MenuScreenRenderer { get; }

    public IGameWindow GameWindow { get; set; } = null!;
    IScreenRenderer IBaseScreen.ScreenRenderer => MenuScreenRenderer;
    public string ScreenTitle => "NeoLemmixSharp";
    public bool IsDisposed { get; private set; }

    public MenuScreen(
        MenuSpriteBank menuSpriteBank,
        FontBank fontBank)
    {
        MenuSpriteBank = menuSpriteBank;
        var menuCursorRenderer = new MenuCursorRenderer(menuSpriteBank, InputController);
        MenuScreenRenderer = new MenuScreenRenderer(
            menuSpriteBank,
            fontBank,
            menuCursorRenderer,
            _pageTransition);

        _currentPage = new MainPage(InputController);

        Current = this;
    }

    public void Initialise()
    {
        MenuScreenRenderer.Initialise(_currentPage);
    }

    public void SetNextPage(IPage page)
    {
        _nextPage = page;
        _pageTransition.BeginTransition();
        InputController.ClearAllKeys();
    }

    public void Tick()
    {
        if (_pageTransition.IsTransitioning)
        {
            HandlePageTransition();

            return;
        }

        InputController.Tick();

        _currentPage.Tick();

        if (InputController.ToggleFullScreen.IsPressed)
        {
            GameWindow.ToggleBorderless();
        }
    }

    private void HandlePageTransition()
    {
        _pageTransition.Tick();

        if (!_pageTransition.IsHalfWayDone)
            return;

        HelperMethods.DisposeOf(ref _currentPage);
        _currentPage = _nextPage!;
        MenuScreenRenderer.SetPage(_nextPage!);
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