using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingRenderer : IViewportObjectRenderer
{
    private const int NumberOfChars = 2;

    private readonly int[] _countDownCharsToRender = new int[NumberOfChars];

    private Lemming _lemming;
    private LemmingActionSprite _actionSprite;

    private bool _shouldRender;
    private bool _shouldRenderCountDown;

    public Span<int> CountDownCharsSpan => new(_countDownCharsToRender);

    public LemmingRenderer(Lemming lemming)
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

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int screenX, int screenY)
    {
        if (!_shouldRender)
            return;

        var renderDestination = new Rectangle(
            screenX,
            screenY,
            sourceRectangle.Width,
            sourceRectangle.Height);

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

        // var p = new Point(screenX - 1, screenY - 1);
        // renderDestination = new Rectangle(p, new Point(3 , 3 ));

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
                1,
                Color.White);
        }

        if (_lemming.ParticleTimer > 0)
        {
            RenderParticles(spriteBatch, screenX, screenY);
        }
    }

    private void RenderParticles(
        SpriteBatch spriteBatch,
        int screenX,
        int screenY)
    {
        var destRectangle = new Rectangle(0, 0, 1, 1);
        var explosionParticleColors = LevelConstants.GetExplosionParticleColors();
        var whitePixelTexture = CommonSpriteBank.Instance.GetTexture(CommonTexture.WhitePixel);

        var sourceRectangle = new Rectangle(0, 255, 1, 1);

        for (var i = 0; i < LevelConstants.NumberOfParticles; i++)
        {
            var offset = ParticleHelper.GetParticleOffsets(_lemming.ParticleTimer, i);

            if (offset.X == -128 || offset.Y == -128)
                continue;

            var color = explosionParticleColors[i & LevelConstants.NumberOfExplosionParticleColorsMask];

            offset += _actionSprite.AnchorPoint;

            destRectangle.X = screenX + offset.X;
            destRectangle.Y = screenY + offset.Y;

            spriteBatch.Draw(
                whitePixelTexture,
                destRectangle,
                sourceRectangle,
                color,
                RenderingLayers.LemmingRenderLayer);
        }
    }

    public void Dispose()
    {
        _lemming = null!;
        _actionSprite = null!;
    }
}