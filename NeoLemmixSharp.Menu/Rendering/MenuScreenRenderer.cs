using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Pages;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuScreenRenderer : IScreenRenderer
{
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly FontBank _fontBank;

    private readonly Desktop _desktop = new();

    private readonly MenuCursorRenderer _menuCursorRenderer;

    private Texture2D _backGround;
    private bool _initialized;

    public bool IsDisposed { get; private set; }
    public IGameWindow GameWindow { get; set; }

    public MenuScreenRenderer(ContentManager contentManager,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        FontBank fontBank,
        MenuCursorRenderer menuCursorRenderer)
    {
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        _fontBank = fontBank;
        _menuCursorRenderer = menuCursorRenderer;
    }

    public void Initialise()
    {
        _backGround = _contentManager.Load<Texture2D>("background");

        _desktop.Background = new TextureRegion(_backGround);

        _initialized = true;
    }

    public void SetPage(IPage page)
    {
        _desktop.Root = page.RootWidget;
    }

    public void RenderScreen(SpriteBatch spriteBatch)
    {
        _desktop.Render();

        _menuCursorRenderer.RenderCursor(spriteBatch);
    }

    public void OnWindowSizeChanged()
    {
        if (!_initialized)
            return;

    }

    public void Dispose()
    {
        _desktop.Dispose();

        HelperMethods.DisposeOf(ref _backGround);
        _menuCursorRenderer.Dispose();
    }
}