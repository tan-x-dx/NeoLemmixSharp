﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Menu.Pages;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuScreenRenderer : IScreenRenderer
{
    private readonly BackgroundRenderer _backgroundRenderer;
    private readonly MenuCursorRenderer _menuCursorRenderer;
    private readonly PageTransitionRenderer _pageTransitionRenderer;

    private UiHandler _uiHandler;

    private bool _initialized;

    public bool IsDisposed { get; private set; }

    public MenuScreenRenderer(
        MenuCursorRenderer menuCursorRenderer,
        PageTransition pageTransition)
    {
        _backgroundRenderer = new BackgroundRenderer(CommonSprites.Background);
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
        // background
        spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
        _backgroundRenderer.Render(spriteBatch);

        // draw ui
        _uiHandler.Render(spriteBatch);

        _menuCursorRenderer.RenderCursor(spriteBatch);

        // fade transition where necessary
        _pageTransitionRenderer.Render(spriteBatch);
        spriteBatch.End();
    }

    public void OnWindowSizeChanged()
    {
        if (!_initialized)
            return;

        var windowSize = IGameWindow.Instance.WindowSize;

        _backgroundRenderer.SetWindowDimensions(windowSize);
        _pageTransitionRenderer.SetWindowDimensions(windowSize);
    }

    public void Dispose()
    {
        if (IsDisposed)
            return;

        //UserInterface.Active.Dispose();

        _pageTransitionRenderer.Dispose();

        _uiHandler = null!;

        IsDisposed = true;
    }

    public void SetNextPage(PageBase currentPage)
    {
        _uiHandler = currentPage.UiHandler;
    }
}