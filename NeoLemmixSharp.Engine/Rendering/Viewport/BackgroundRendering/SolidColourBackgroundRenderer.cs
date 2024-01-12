using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;

public sealed class SolidColorBackgroundRenderer : IBackgroundRenderer
{
	private readonly Texture2D _pixelTexture;
	private readonly Level.Viewport _viewport;
	private readonly Color _backgroundColor;

	public SolidColorBackgroundRenderer(Level.Viewport viewport, Color backgroundColor)
	{
		_pixelTexture = CommonSpriteBank.Instance.GetTexture(CommonTexture.WhitePixel);
		_backgroundColor = backgroundColor;
		_viewport = viewport;
	}

	public void RenderBackground(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(
			_pixelTexture,
			new Rectangle(
				0,
				0,
				_viewport.ViewPortWidth,
				_viewport.ViewPortHeight),
			new Rectangle(0, 0, 1, 1),
			_backgroundColor,
			RenderingLayers.BackgroundLayer);
	}

	public void Dispose()
	{
	}
}