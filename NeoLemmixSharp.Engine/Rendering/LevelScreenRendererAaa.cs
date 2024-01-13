using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class LevelScreenRendererAaa : IScreenRenderer
{
	private readonly GraphicsDevice _graphicsDevice;
	private DepthStencilState _depthStencilState;
	private RenderTarget2D _levelRenderTarget;
	private RenderTarget2D _controlPanelRenderTarget;

	private IBackgroundRenderer _backgroundRenderer;

	public bool IsDisposed { get; private set; }

	public LevelScreenRendererAaa(
		GraphicsDevice graphicsDevice,
		LevelData levelData,
		Level.Viewport viewport,
		IViewportObjectRenderer[] levelSprites,
		LevelCursorSprite levelCursorSprite,
		LemmingSpriteBank lemmingSpriteBank,
		GadgetSpriteBank gadgetSpriteBank)
	{
		_graphicsDevice = graphicsDevice;
		_depthStencilState = new DepthStencilState { DepthBufferEnable = true };
		_graphicsDevice.DepthStencilState = _depthStencilState;

		_levelRenderTarget = GetLevelRenderTarget2D();
		_controlPanelRenderTarget = GetControlPanelRenderTarget2D();

		_backgroundRenderer = GetBackgroundRenderer(levelData, viewport);
	}

	private IBackgroundRenderer GetBackgroundRenderer(
		LevelData levelData,
		Level.Viewport viewport)
	{
		return new SolidColorBackgroundRenderer(viewport, new Color(24, 24, 60));
	}

	public void RenderScreen(SpriteBatch spriteBatch)
	{
		// Set the render target
		_graphicsDevice.SetRenderTarget(_levelRenderTarget);
		RenderLevel(spriteBatch);

		_graphicsDevice.SetRenderTarget(_controlPanelRenderTarget);
		RenderControlPanel(spriteBatch);

		// Drop the render target
		_graphicsDevice.SetRenderTarget(null);
	}

	private void RenderLevel(SpriteBatch spriteBatch)
	{
		_graphicsDevice.Clear(Color.Black);

		_backgroundRenderer.RenderBackground(spriteBatch);
	}

	private void RenderControlPanel(SpriteBatch spriteBatch)
	{

	}

	public void OnWindowSizeChanged()
	{
		DisposableHelperMethods.DisposeOf(ref _levelRenderTarget);
		DisposableHelperMethods.DisposeOf(ref _controlPanelRenderTarget);
		_levelRenderTarget = GetLevelRenderTarget2D();
		_controlPanelRenderTarget = GetControlPanelRenderTarget2D();
	}

	private RenderTarget2D GetLevelRenderTarget2D()
	{
		return new RenderTarget2D(
			_graphicsDevice,
			_graphicsDevice.PresentationParameters.BackBufferWidth,
			_graphicsDevice.PresentationParameters.BackBufferHeight,
			false,
			_graphicsDevice.PresentationParameters.BackBufferFormat,
			DepthFormat.Depth24);
	}

	private RenderTarget2D GetControlPanelRenderTarget2D()
	{
		return new RenderTarget2D(
			_graphicsDevice,
			_graphicsDevice.PresentationParameters.BackBufferWidth,
			_graphicsDevice.PresentationParameters.BackBufferHeight,
			false,
			_graphicsDevice.PresentationParameters.BackBufferFormat,
			DepthFormat.Depth24);
	}

	public void Dispose()
	{
		if (IsDisposed)
			return;

		DisposableHelperMethods.DisposeOf(ref _levelRenderTarget);
		DisposableHelperMethods.DisposeOf(ref _controlPanelRenderTarget);
		DisposableHelperMethods.DisposeOf(ref _depthStencilState);

		DisposableHelperMethods.DisposeOf(ref _backgroundRenderer);

		IsDisposed = true;
	}
}