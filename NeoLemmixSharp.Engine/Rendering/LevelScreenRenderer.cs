using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

namespace NeoLemmixSharp.Engine.Rendering;

public sealed class LevelScreenRenderer : IScreenRenderer
{
	public static ControlPanelSpriteBank ControlPanelSpriteBank { get; private set; } = null!;

	public static void SetControlPanelSpriteBank(ControlPanelSpriteBank controlPanelSpriteBank)
	{
		ControlPanelSpriteBank = controlPanelSpriteBank;
	}

	private readonly int _levelWidth;
	private readonly int _levelHeight;
	private readonly Level.Viewport _viewport;

	private readonly LemmingSpriteBank _lemmingSpriteBank;
	private readonly GadgetSpriteBank _gadgetSpriteBank;

	private readonly IBackgroundRenderer _backgroundRenderer;
	private readonly TerrainRenderer _terrainRenderer;
	private readonly IViewportObjectRenderer[] _levelSprites;
	private readonly LevelCursorSprite _cursorSprite;

	private readonly IControlPanelRenderer _controlPanelRenderer;

	public bool IsDisposed { get; set; }

	public LevelScreenRenderer(
		int levelWidth,
		int levelHeight,
		Level.Viewport viewport,
		IBackgroundRenderer backgroundRenderer,
		TerrainRenderer terrainRenderer,
		IViewportObjectRenderer[] levelSprites,
		LevelCursorSprite levelCursorSprite,
		IControlPanelRenderer controlPanelRenderer,
		LemmingSpriteBank lemmingSpriteBank,
		GadgetSpriteBank gadgetSpriteBank)
	{
		_levelWidth = levelWidth;
		_levelHeight = levelHeight;
		_viewport = viewport;

		_backgroundRenderer = backgroundRenderer;
		_terrainRenderer = terrainRenderer;
		_levelSprites = levelSprites;
		_cursorSprite = levelCursorSprite;
		_controlPanelRenderer = controlPanelRenderer;

		_lemmingSpriteBank = lemmingSpriteBank;
		_gadgetSpriteBank = gadgetSpriteBank;
	}

	public void RenderScreen(SpriteBatch spriteBatch)
	{
		//_graphicsDevice.Clear(Color.Black);

		spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp);

		RenderLevel(spriteBatch);
		RenderControlPanel(spriteBatch);

		spriteBatch.End();
	}

	private void RenderLevel(SpriteBatch spriteBatch)
	{
		_backgroundRenderer.RenderBackground(spriteBatch);
		_terrainRenderer.RenderTerrain(spriteBatch);

		RenderSprites(spriteBatch);
	}

	private void RenderSprites(SpriteBatch spriteBatch)
	{
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

						sprite.RenderAtPosition(spriteBatch, clipIntersection, screenX, screenY);
					}
				}

				h0 += h;
			}

			h0 = dy;
			w0 += w;
		}
	}

	private void RenderControlPanel(SpriteBatch spriteBatch)
	{
		_controlPanelRenderer.RenderControlPanel(spriteBatch);
		_cursorSprite.RenderAtPosition(spriteBatch, _viewport.ScreenMouseX, _viewport.ScreenMouseY, _viewport.ScaleMultiplier);
	}

	public void OnWindowSizeChanged()
	{
	}

	public void Dispose()
	{
		if (IsDisposed)
			return;

#pragma warning disable CS8625
		SetControlPanelSpriteBank(null);
#pragma warning restore CS8625

		_lemmingSpriteBank.Dispose();
		_gadgetSpriteBank.Dispose();

		DisposableHelperMethods.DisposeOfAll(new ReadOnlySpan<IViewportObjectRenderer>(_levelSprites));
		IsDisposed = true;
	}
}