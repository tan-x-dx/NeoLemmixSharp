using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingRenderer : IViewportObjectRenderer
{
    private const int NumberOfCountDownChars = 2;

    private CountDownCharBuffer _countDownCharBuffer;

    private Lemming _lemming;
    private LemmingActionSprite _actionSprite;
    private Rectangle _previousSpriteBounds;
    private Rectangle _spriteBounds;

    private bool _shouldRenderCountDown;

    public Span<int> CountDownCharsSpan => _countDownCharBuffer;

    public int RendererId { get; set; }
    public int ItemId => _lemming.Id;

    public LevelPosition TopLeftPixel => _spriteBounds.TopLeftLevelPosition();
    public LevelPosition BottomRightPixel => _spriteBounds.BottomRightLevelPosition();
    public LevelPosition PreviousTopLeftPixel => _previousSpriteBounds.TopLeftLevelPosition();
    public LevelPosition PreviousBottomRightPixel => _previousSpriteBounds.BottomRightLevelPosition();

    public LemmingRenderer(Lemming lemming)
    {
        _lemming = lemming;
    }

    public void UpdatePosition()
    {
        var p = _lemming.LevelPosition - _actionSprite.AnchorPoint;

        _previousSpriteBounds = _spriteBounds;
        _spriteBounds = new Rectangle(p.X, p.Y, _actionSprite.SpriteWidth, _actionSprite.SpriteHeight);

        LevelScreenRenderer.Instance.LevelRenderer.UpdateSpritePosition(this);
    }

    public void UpdateLemmingState(bool shouldRender)
    {
        if (!shouldRender)
        {
            LevelScreenRenderer.Instance.LevelRenderer.DeregisterSpriteForRendering(this);

            return;
        }

        LevelScreenRenderer.Instance.LevelRenderer.RegisterSpriteForRendering(this);

        var spriteBank = _lemming.State.TeamAffiliation.SpriteBank;

        _actionSprite = spriteBank.GetActionSprite(
            _lemming.CurrentAction,
            _lemming.Orientation,
            _lemming.FacingDirection);

        UpdatePosition();
    }

    public void SetDisplayTimer(bool displayTimer)
    {
        _shouldRenderCountDown = displayTimer;
    }

    public Rectangle GetSpriteBounds() => _spriteBounds;

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int projectionX, int projectionY)
    {
        var renderDestination = new Rectangle(
            projectionX,
            projectionY,
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
                projectionX + countDownPositionOffset.X,
                projectionY + countDownPositionOffset.Y,
                1,
                Color.White);
        }

        if (_lemming.ParticleTimer > 0)
        {
            RenderParticles(spriteBatch, projectionX, projectionY);
        }
        /*
        if (_lemming.Id < 7)
        {
            FontBank.MenuFont.RenderText(
                spriteBatch,
                _lemming.LevelPosition.ToString(),
                screenX - 10,
                screenY - MenuFont.GlyphHeight * 2,
                1,
                Color.White);
        }*/
    }

    private void RenderParticles(
        SpriteBatch spriteBatch,
        int screenX,
        int screenY)
    {
        var destRectangle = new Rectangle(0, 0, 1, 1);
        var explosionParticleColors = LevelConstants.GetExplosionParticleColors();
        var whitePixelTexture = CommonSprites.WhitePixelGradientSprite;

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
                color);
        }
    }

    public void Dispose()
    {
        _lemming = null!;
        _actionSprite = null!;
    }

    [InlineArray(NumberOfCountDownChars)]
    private struct CountDownCharBuffer
    {
        private int _firstElement;
    }
}