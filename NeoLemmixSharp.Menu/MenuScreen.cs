using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Pages;
using NeoLemmixSharp.Menu.Rendering;

namespace NeoLemmixSharp.Menu;

public sealed class MenuScreen : IBaseScreen
{
    private readonly IPage[] _allPages;

    private IPage _currentPage;

    public MenuInputController InputController { get; } = new();

    public MenuScreenRenderer MenuScreenRenderer { get; }

    public IGameWindow GameWindow { get; set; }
    IScreenRenderer IBaseScreen.ScreenRenderer => MenuScreenRenderer;
    public string ScreenTitle => "NeoLemmixSharp";
    public bool IsDisposed { get; private set; }

    public MenuScreen(
        ContentManager content,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        FontBank fontBank)
    {
        var menuCursorRenderer = new MenuCursorRenderer(graphicsDevice, InputController);

        _allPages = GeneratePages();
        _currentPage = _allPages[0];

        MenuScreenRenderer = new MenuScreenRenderer(
            content,
            graphicsDevice,
            spriteBatch,
            fontBank,
            menuCursorRenderer);
    }

    public void Initialise()
    {
        MenuScreenRenderer.Initialise();
        MenuScreenRenderer.SetPage(_currentPage);
    }

    private IPage[] GeneratePages()
    {
        var result = new IPage[]
        {
            new MainPage(InputController)
        };

        return result;
    }

    public void Tick()
    {
        InputController.Tick();

        HandleKeyboardInput();
        HandleMouseInput();

        if (InputController.Quit.IsPressed)
        {
            GameWindow.Escape();
        }

        if (InputController.ToggleFullScreen.IsPressed)
        {
            GameWindow.ToggleBorderless();
        }
    }

    private void HandleKeyboardInput()
    {
    }

    private void HandleMouseInput()
    {
    }

    public void OnWindowSizeChanged()
    {
        MenuScreenRenderer.OnWindowSizeChanged();
    }

    public void Dispose()
    {
    }
}