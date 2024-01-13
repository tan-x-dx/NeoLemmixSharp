using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel;
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

	private readonly LevelRenderer _levelRenderer;
	private readonly ControlPanelRenderer _controlPanelRenderer;

	public bool IsDisposed { get; private set; }

	public LevelScreenRendererAaa(
		GraphicsDevice graphicsDevice,
		LevelData levelData,
		ILevelControlPanel controlPanel,
		Level.Viewport viewport,
		IViewportObjectRenderer[] levelSprites,
		TerrainRenderer terrainRenderer,
		LevelCursorSprite levelCursorSprite)
	{
		_graphicsDevice = graphicsDevice;
		_depthStencilState = new DepthStencilState { DepthBufferEnable = true };
		_graphicsDevice.DepthStencilState = _depthStencilState;

		var backgroundRenderer = GetBackgroundRenderer(levelData, viewport);

		_levelRenderer = new LevelRenderer(
			graphicsDevice,
			levelData,
			controlPanel,
			viewport,
			levelSprites,
			backgroundRenderer,
			terrainRenderer);
		_controlPanelRenderer = new ControlPanelRenderer(
			_graphicsDevice,
			controlPanel,
			levelData,
			levelCursorSprite);
	}

	private static IBackgroundRenderer GetBackgroundRenderer(
		LevelData levelData,
		Level.Viewport viewport)
	{
		return new SolidColorBackgroundRenderer(viewport, new Color(24, 24, 60));
	}

	public void RenderScreen(SpriteBatch spriteBatch)
	{
		_levelRenderer.RenderLevel(spriteBatch);

		_controlPanelRenderer.RenderControlPanel(spriteBatch);

		_graphicsDevice.SetRenderTarget(null);
	}

	public void OnWindowSizeChanged()
	{
		_levelRenderer.OnWindowSizeChanged();
		_controlPanelRenderer.OnWindowSizeChanged();
	}

	public void Dispose()
	{
		if (IsDisposed)
			return;

		DisposableHelperMethods.DisposeOf(ref _depthStencilState);
		_levelRenderer.Dispose();
		_controlPanelRenderer.Dispose();

		IsDisposed = true;
	}
}