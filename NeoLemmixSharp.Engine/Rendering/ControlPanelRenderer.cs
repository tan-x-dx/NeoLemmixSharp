using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering.Viewport;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class ControlPanelRenderer
{
	private readonly GraphicsDevice _graphicsDevice;
	private readonly ILevelControlPanel _controlPanel;

	private RenderTarget2D _controlPanelRenderTarget;
	private bool _disposed;

	public ControlPanelRenderer(
		GraphicsDevice graphicsDevice,
		ILevelControlPanel controlPanel,
		LevelData levelData,
		LevelCursorSprite levelCursorSprite)
	{
		_graphicsDevice = graphicsDevice;
		_controlPanel = controlPanel;

		_controlPanelRenderTarget = GetControlPanelRenderTarget2D();
	}

	public void RenderControlPanel(SpriteBatch spriteBatch)
	{
		//_graphicsDevice.SetRenderTarget(_controlPanelRenderTarget);
	}

	public void DrawToScreen(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(_controlPanelRenderTarget, Vector2.Zero, Color.White);
	}

	private RenderTarget2D GetControlPanelRenderTarget2D()
	{
		return new RenderTarget2D(
			_graphicsDevice,
			_controlPanel.Width,
			_controlPanel.Height,
			false,
			_graphicsDevice.PresentationParameters.BackBufferFormat,
			DepthFormat.Depth24);
	}

	public void OnWindowSizeChanged()
	{
		DisposableHelperMethods.DisposeOf(ref _controlPanelRenderTarget);
		_controlPanelRenderTarget = GetControlPanelRenderTarget2D();
	}

	public void Dispose()
	{
		if (_disposed)
			return;

		DisposableHelperMethods.DisposeOf(ref _controlPanelRenderTarget);

		_disposed = true;
	}
}