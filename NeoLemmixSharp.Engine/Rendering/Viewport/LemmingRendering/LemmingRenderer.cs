﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingRenderer : IViewportObjectRenderer
{
    private const int NumberOfCountDownChars = 2;

    private CountDownCharBuffer _countDownCharBuffer;

    private Lemming _lemming;
    private LemmingActionSprite _actionSprite;
    private Rectangle _spriteBounds;
    private Rectangle _previousSpriteBounds;

    private bool _shouldRenderCountDown;

    public Span<char> CountDownCharsSpan => _countDownCharBuffer;

    public int RendererId { get; set; }
    public int ItemId => _lemming.Id;

    public LevelRegion CurrentBounds => new(_spriteBounds);
    public LevelRegion PreviousBounds => new(_previousSpriteBounds);

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

        var spriteBank = _lemming.State.TeamAffiliation.SpriteBank;

        _actionSprite = spriteBank.GetActionSprite(
            _lemming.CurrentAction,
            _lemming.Orientation,
            _lemming.FacingDirection);

        UpdatePosition();

        LevelScreenRenderer.Instance.LevelRenderer.RegisterSpriteForRendering(this);
    }

    public void ResetPosition()
    {
        LevelScreenRenderer.Instance.LevelRenderer.DeregisterSpriteForRendering(this);

        if (!_lemming.State.IsActive)
            return;

        var spriteBank = _lemming.State.TeamAffiliation.SpriteBank;

        _actionSprite = spriteBank.GetActionSprite(
            _lemming.CurrentAction,
            _lemming.Orientation,
            _lemming.FacingDirection);

        UpdatePosition();

        LevelScreenRenderer.Instance.LevelRenderer.RegisterSpriteForRendering(this);
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
        var explosionParticleColors = EngineConstants.GetExplosionParticleColors();
        var whitePixelTexture = CommonSprites.WhitePixelGradientSprite;

        var sourceRectangle = CommonSprites.RectangleForWhitePixelAlpha(0xff);

        for (var i = 0; i < EngineConstants.NumberOfParticles; i++)
        {
            var offset = ParticleHelper.GetParticleOffsets(_lemming.ParticleTimer, i);

            if (offset.X == -128 || offset.Y == -128)
                continue;

            var color = explosionParticleColors[i & EngineConstants.NumberOfExplosionParticleColorsMask];

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
        private char _0;
    }
}