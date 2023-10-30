using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Rendering;

namespace NeoLemmixSharp.Menu;

public sealed class MenuScreen : IBaseScreen
{
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
        MenuScreenRenderer = new MenuScreenRenderer(content, graphicsDevice, spriteBatch, fontBank, null);
    }

    public void Initialise()
    {
        MenuScreenRenderer.Initialise();
    }

    public void Tick()
    {
        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleKeyboardInput()
    {
    }

    private void HandleMouseInput()
    {
    }

    public void OnWindowSizeChanged()
    {
    }

    public void Dispose()
    {
    }
}