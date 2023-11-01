using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Pages;
using NeoLemmixSharp.Menu.Rendering.Pages;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuScreenRenderer : IScreenRenderer
{
    private readonly MenuSpriteBank _menuSpriteBank;
    private readonly FontBank _fontBank;

    private readonly Desktop _desktop = new();

    private readonly MenuCursorRenderer _menuCursorRenderer;
    private readonly PageTransitionRenderer _pageTransitionRenderer;

    private IPageRenderer _currentPageRenderer;
    private Texture2D _backGround;
    private bool _initialized;

    public bool IsDisposed { get; private set; }
    public IGameWindow GameWindow { get; set; }

    public MenuScreenRenderer(
        MenuSpriteBank menuSpriteBank,
        FontBank fontBank,
        MenuCursorRenderer menuCursorRenderer,
        PageTransition pageTransition)
    {
        _menuSpriteBank = menuSpriteBank;
        _fontBank = fontBank;
        _menuCursorRenderer = menuCursorRenderer;
        _pageTransitionRenderer = new PageTransitionRenderer(menuSpriteBank, pageTransition);
    }

    public void Initialise(IPage currentPage)
    {
        var backGroundTexture = _menuSpriteBank.GetTexture(MenuResource.Background);
        var backgroundBrush = new BackgroundBrush(backGroundTexture);
        _desktop.Background = backgroundBrush;

        _currentPageRenderer = currentPage.GetPageRenderer(_menuSpriteBank, _desktop);

        _initialized = true;
        OnWindowSizeChanged();
    }

    public void SetPage(IPage page)
    {
        HelperMethods.DisposeOf(ref _currentPageRenderer);
        _currentPageRenderer = page.GetPageRenderer(_menuSpriteBank, _desktop);
        _currentPageRenderer.SetRootWidget(_desktop);
    }

    public void RenderScreen(SpriteBatch spriteBatch)
    {
        _currentPageRenderer.RenderPage(spriteBatch);

        _menuCursorRenderer.RenderCursor(spriteBatch);

        _pageTransitionRenderer.Render(spriteBatch);
    }

    public void OnWindowSizeChanged()
    {
        if (!_initialized)
            return;

        var windowWidth = GameWindow.WindowWidth;
        var windowHeight = GameWindow.WindowHeight;

        _currentPageRenderer.SetWindowDimensions(windowWidth, windowHeight);
        _pageTransitionRenderer.SetWindowDimensions(windowWidth, windowHeight);
    }

    public void Dispose()
    {
        _desktop.Dispose();

        HelperMethods.DisposeOf(ref _backGround);
        _menuCursorRenderer.Dispose();
        _pageTransitionRenderer.Dispose();
    }
}