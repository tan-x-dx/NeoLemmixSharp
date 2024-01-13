using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class LevelRenderer : IDisposable
{
	private readonly GraphicsDevice _graphicsDevice;
	private readonly ILevelControlPanel _levelControlPanel;
	private readonly Level.Viewport _viewport;
	private readonly IViewportObjectRenderer[] _levelSprites;

	private IBackgroundRenderer _backgroundRenderer;
	private TerrainRenderer _terrainRenderer;

	private RenderTarget2D _levelRenderTarget;
	private bool _disposed;

	public LevelRenderer(
		GraphicsDevice graphicsDevice,
		LevelData levelData,
		ILevelControlPanel levelControlPanel,
		Level.Viewport viewport,
		IViewportObjectRenderer[] levelSprites,
		IBackgroundRenderer backgroundRenderer,
		TerrainRenderer terrainRenderer)
	{
		_graphicsDevice = graphicsDevice;
		_levelControlPanel = levelControlPanel;
		_viewport = viewport;
		_levelSprites = levelSprites;

		_backgroundRenderer = backgroundRenderer;
		_terrainRenderer = terrainRenderer;
		_levelRenderTarget = GetLevelRenderTarget2D();
	}

	public void RenderLevel(SpriteBatch spriteBatch)
	{
		_graphicsDevice.SetRenderTarget(_levelRenderTarget);
		_graphicsDevice.Clear(Color.DarkGray);
		_backgroundRenderer.RenderBackground(spriteBatch);
		_terrainRenderer.RenderTerrain(spriteBatch);

		RenderSprites(spriteBatch);
	}

	private void RenderSprites(SpriteBatch spriteBatch)
	{

	}

	private void RenderSprites2(SpriteBatch spriteBatch)
	{
		var _levelWidth = 1;
		var _levelHeight = 1;
		var w = _levelWidth * _viewport.ScaleMultiplier;
		var h = _levelHeight * _viewport.ScaleMultiplier;
		var maxX = _viewport.NumberOfHorizontalRenderIntervals;
		var maxY = _viewport.NumberOfVerticalRenderIntervals;
		var dx = _viewport.ScreenX - _viewport.ViewPortX * _viewport.ScaleMultiplier;
		var dy = _viewport.ScreenY - _viewport.ViewPortY * _viewport.ScaleMultiplier;
		var w0 = dx;
		var h0 = dy;
		var levelSpritesSpan = new ReadOnlySpan<IViewportObjectRenderer>(_levelSprites);

		for (var i = 0; i < maxX; i++)
		{
			var hInterval = _viewport.GetHorizontalRenderInterval(i);
			for (var j = 0; j < maxY; j++)
			{
				var vInterval = _viewport.GetVerticalRenderInterval(j);
				var viewportClip = new Rectangle(hInterval.PixelStart, vInterval.PixelStart, hInterval.PixelLength, vInterval.PixelLength);

				foreach (var sprite in levelSpritesSpan)
				{
					var spriteClip = sprite.GetSpriteBounds();

					Rectangle.Intersect(ref viewportClip, ref spriteClip, out var clipIntersection);

					if (!clipIntersection.IsEmpty)
					{
						clipIntersection.X -= spriteClip.X;
						clipIntersection.Y -= spriteClip.Y;

						var screenX = (spriteClip.X + clipIntersection.X) * _viewport.ScaleMultiplier + w0;
						var screenY = (spriteClip.Y + clipIntersection.Y) * _viewport.ScaleMultiplier + h0;

						sprite.RenderAtPosition(spriteBatch, clipIntersection, screenX, screenY, _viewport.ScaleMultiplier);
					}
				}

				h0 += h;
			}

			h0 = dy;
			w0 += w;
		}
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

	public void OnWindowSizeChanged()
	{
		DisposableHelperMethods.DisposeOf(ref _levelRenderTarget);
		_levelRenderTarget = GetLevelRenderTarget2D();
	}

	public void Dispose()
	{
		if (_disposed)
			return;

		DisposableHelperMethods.DisposeOf(ref _backgroundRenderer);
		DisposableHelperMethods.DisposeOf(ref _terrainRenderer);
		DisposableHelperMethods.DisposeOf(ref _levelRenderTarget);

		_disposed = true;
	}
}