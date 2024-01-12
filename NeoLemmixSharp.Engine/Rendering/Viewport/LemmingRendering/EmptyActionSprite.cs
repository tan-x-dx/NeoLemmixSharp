using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class EmptyActionSprite : ActionSprite
{
	public static EmptyActionSprite Instance { get; private set; } = null!;

	public static void Initialise(GraphicsDevice graphicsDevice)
	{
		var texture = new Texture2D(graphicsDevice, 1, 1);
		var data = new[] { 0U };
		texture.SetData(data);
		Instance = new EmptyActionSprite(texture);
	}

	private EmptyActionSprite(Texture2D texture)
		: base(texture, 0, 0, new LevelPosition())
	{
	}

	public override void RenderLemming(SpriteBatch spriteBatch, Level.Lemmings.Lemming lemming, Rectangle sourceRectangle, Rectangle destinationRectangle)
	{
	}
}