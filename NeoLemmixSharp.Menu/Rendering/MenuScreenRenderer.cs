using MGUI.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuScreenRenderer : IScreenRenderer
{
    private readonly BackgroundRenderer _backgroundRenderer;
    private readonly MenuCursorRenderer _menuCursorRenderer;
    private readonly PageTransitionRenderer _pageTransitionRenderer;
    private readonly MGDesktop _desktop;

    private bool _initialized;

    public bool IsDisposed { get; private set; }

    public MenuScreenRenderer(
        MenuCursorRenderer menuCursorRenderer,
        PageTransition pageTransition,
        MGDesktop desktop)
    {
        _backgroundRenderer = new BackgroundRenderer(MenuSpriteBank.GetTexture(MenuResource.Background));
        _menuCursorRenderer = menuCursorRenderer;
        _pageTransitionRenderer = new PageTransitionRenderer(pageTransition);
        _desktop = desktop;
    }

    public void Initialise()
    {
        if (_initialized)
            return;

        //   UserInterface.Active.DebugDraw = true;

        _initialized = true;
        OnWindowSizeChanged();
    }

    public void RenderScreen(SpriteBatch spriteBatch)
    {
        // draw ui
        //UserInterface.Active.Draw(spriteBatch);

        spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp);

        _backgroundRenderer.Render(spriteBatch);

        spriteBatch.End();

        // finalize ui rendering
        //UserInterface.Active.DrawMainRenderTarget(spriteBatch);

        _desktop.Draw();

        /*   spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp);

           _pageTransitionRenderer.Render(spriteBatch);

           spriteBatch.End();*/


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

        _menuCursorRenderer.Dispose();
        _pageTransitionRenderer.Dispose();

        IsDisposed = true;
    }
}