using GeonBit.UI;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Pages;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuScreenRenderer : IScreenRenderer
{
    private readonly MenuSpriteBank _menuSpriteBank;
    private readonly FontBank _fontBank;

    private readonly MenuCursorRenderer _menuCursorRenderer;
    private readonly PageTransitionRenderer _pageTransitionRenderer;

    private UserInterface _userInterface;
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
        if (_initialized)
            return;

        _userInterface = currentPage.UserInterface;

        _initialized = true;
        OnWindowSizeChanged();
    }

    public void SetPage(IPage page)
    {
        _userInterface = page.UserInterface;
    }

    public void RenderScreen(SpriteBatch spriteBatch)
    {
        try
        {
            _userInterface.Draw(spriteBatch);
        }
        catch (Exception ex)
        {
            ;
        }

        _menuCursorRenderer.RenderCursor(spriteBatch);

        _pageTransitionRenderer.Render(spriteBatch);
    }

    public void OnWindowSizeChanged()
    {
        if (!_initialized)
            return;

        var windowWidth = GameWindow.WindowWidth;
        var windowHeight = GameWindow.WindowHeight;

        _pageTransitionRenderer.SetWindowDimensions(windowWidth, windowHeight);
    }

    public void Dispose()
    {
        _userInterface.Dispose();

        HelperMethods.DisposeOf(ref _backGround);
        _menuCursorRenderer.Dispose();
        _pageTransitionRenderer.Dispose();
    }
}