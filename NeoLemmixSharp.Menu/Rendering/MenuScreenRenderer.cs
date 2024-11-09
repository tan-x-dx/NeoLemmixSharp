using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using RenderingLibrary;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuScreenRenderer : IScreenRenderer
{
    private readonly BackgroundRenderer _backgroundRenderer;
    private readonly MenuCursorRenderer _menuCursorRenderer;
    private readonly PageTransitionRenderer _pageTransitionRenderer;

    private bool _initialized;

    public bool IsDisposed { get; private set; }

    public MenuScreenRenderer(
        MenuCursorRenderer menuCursorRenderer,
        PageTransition pageTransition)
    {
        _backgroundRenderer = new BackgroundRenderer(MenuSpriteBank.Background);
        _menuCursorRenderer = menuCursorRenderer;
        _pageTransitionRenderer = new PageTransitionRenderer(pageTransition);
    }

    public void Initialise()
    {
        if (_initialized)
            return;

        //   UserInterface.Active.DebugDraw = true;

        _initialized = true;
        OnWindowSizeChanged();
    }

    public void RenderScreen(GameTime gameTime, SpriteBatch spriteBatch)
    {
        IGameWindow.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);
        // background
        spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp);
        _backgroundRenderer.Render(spriteBatch);
        spriteBatch.End();

        // draw ui
        SystemManagers.Default.Draw();

        spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp);
        _menuCursorRenderer.RenderCursor(spriteBatch);

        // fade transition where necessary
        _pageTransitionRenderer.Render(spriteBatch);
        spriteBatch.End();
    }

    public void OnWindowSizeChanged()
    {
        if (!_initialized)
            return;

        var windowWidth = IGameWindow.Instance.WindowWidth;
        var windowHeight = IGameWindow.Instance.WindowHeight;

        _backgroundRenderer.SetWindowDimensions(windowWidth, windowHeight);
        _pageTransitionRenderer.SetWindowDimensions(windowWidth, windowHeight);
    }

    public void Dispose()
    {
        if (IsDisposed)
            return;

        //UserInterface.Active.Dispose();

        _pageTransitionRenderer.Dispose();

        IsDisposed = true;
    }
}