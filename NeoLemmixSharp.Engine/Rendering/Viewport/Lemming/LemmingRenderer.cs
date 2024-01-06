using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

public sealed class LemmingRenderer : IViewportObjectRenderer
{
	private const int NumberOfChars = 2;

	private readonly int[] _countDownCharsToRender = new int[NumberOfChars];

	private Level.Lemmings.Lemming _lemming;
	private ActionSprite _actionSprite;

	private bool _shouldRender;
	private bool _shouldRenderCountDown;

	public Span<int> CountDownCharsSpan => new(_countDownCharsToRender);

	public LemmingRenderer(Level.Lemmings.Lemming lemming)
	{
		_lemming = lemming;
	}

	public void UpdateLemmingState(bool shouldRender)
	{
		_shouldRender = shouldRender;

		if (!shouldRender)
			return;

		var spriteBank = _lemming.State.TeamAffiliation.SpriteBank;

		_actionSprite = spriteBank.GetActionSprite(
			_lemming.CurrentAction,
			_lemming.Orientation,
			_lemming.FacingDirection);
	}

	public void SetDisplayTimer(bool displayTimer)
	{
		_shouldRenderCountDown = displayTimer;
	}

	public Rectangle GetSpriteBounds()
	{
		if (!_shouldRender)
			return Rectangle.Empty;

		var p = _lemming.LevelPosition - _actionSprite.AnchorPoint;

		return new Rectangle(p.X, p.Y, _actionSprite.SpriteWidth, _actionSprite.SpriteHeight);
	}

	public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
	{
		if (!_shouldRender)
			return;

		var renderDestination = new Rectangle(
			screenX,
			screenY,
			sourceRectangle.Width * scaleMultiplier,
			sourceRectangle.Height * scaleMultiplier);

		sourceRectangle.Y += _lemming.AnimationFrame * _actionSprite.SpriteHeight;

		_actionSprite.RenderLemming(spriteBatch, _lemming, sourceRectangle, renderDestination);

		/* spriteBatch.Draw(
              actionSprite.Texture,
              renderDestination,
              actionSprite.GetSourceRectangleForFrame(sourceRectangle, _lemming.AnimationFrame),
              Color.White,
              0.0f,
              new Vector2(),
              SpriteEffects.None,
              RenderingLayers.LemmingRenderLayer);*/

		// var p = new Point(screenX - scaleMultiplier, screenY - scaleMultiplier);
		// renderDestination = new Rectangle(p, new Point(3 * scaleMultiplier, 3 * scaleMultiplier));

		/* var spriteBank = LevelScreen.CurrentLevel.SpriteBank;
         spriteBatch.Draw(
             spriteBank.AnchorTexture,
             renderDestination,
             Color.White,
             0.0f,
             new Vector2(),
             SpriteEffects.None,
             RenderingLayers.LemmingRenderLayer);*/

		if (_shouldRenderCountDown)
		{
			var countDownPositionOffset = new LevelPosition();

			FontBank.CountDownFont.RenderTextSpan(
				spriteBatch,
				CountDownCharsSpan,
				screenX + countDownPositionOffset.X,
				screenY + countDownPositionOffset.Y,
				scaleMultiplier,
				Color.White);
		}
	}

	public void Dispose()
	{
		_lemming = null!;
		_actionSprite = null!;
	}
}