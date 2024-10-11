using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Ui;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuScreenRenderer : IScreenRenderer
{
    private readonly BackgroundRenderer _backgroundRenderer;
    private readonly MenuCursorRenderer _menuCursorRenderer;
    private readonly PageTransitionRenderer _pageTransitionRenderer;
    private readonly UiSystem _uiSystem;

    private bool _initialized;

    public bool IsDisposed { get; private set; }

    public MenuScreenRenderer(
        MenuCursorRenderer menuCursorRenderer,
        PageTransition pageTransition,
        UiSystem uiSystem)
    {
        _backgroundRenderer = new BackgroundRenderer(MenuSpriteBank.Background);
        _menuCursorRenderer = menuCursorRenderer;
        _pageTransitionRenderer = new PageTransitionRenderer(pageTransition);
        _uiSystem = uiSystem;
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
        // background
        spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp);
        _backgroundRenderer.Render(spriteBatch);
        spriteBatch.End();

        // draw ui
        _uiSystem.Draw(gameTime, spriteBatch);

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