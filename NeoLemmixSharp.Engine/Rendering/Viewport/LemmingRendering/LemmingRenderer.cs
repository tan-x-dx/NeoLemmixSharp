using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingRenderer : IViewportObjectRenderer
{
    private const int NumberOfCountDownChars = 2;

    private CountDownCharBuffer _countDownCharBuffer;

    private Lemming _lemming;
    private LemmingActionSprite _actionSprite;

    private RectangularRegion _spriteBounds;

    private bool _shouldRenderCountDown;

    public Span<char> CountDownCharsSpan => _countDownCharBuffer;

    public int RendererId { get; set; }
    public int ItemId => _lemming.Id;

    public RectangularRegion CurrentBounds => _spriteBounds;

    public LemmingRenderer(Lemming lemming)
    {
        _lemming = lemming;
    }

    public void UpdatePosition()
    {
        var dht = new DihedralTransformation(_lemming.Orientation, _lemming.FacingDirection);
        var spriteOffset = dht.Transform(_actionSprite.AnchorPoint, _actionSprite.SpriteSize);
        var p = _lemming.AnchorPosition - spriteOffset;

        _spriteBounds = new RectangularRegion(p, dht.Transform(_actionSprite.SpriteSize));

        LevelScreenRenderer.Instance.LevelRenderer.UpdateSpritePosition(this);
    }

    public void UpdateLemmingState(bool shouldRender)
    {
        var levelScreenRenderer = LevelScreenRenderer.Instance.LevelRenderer;
        if (!shouldRender)
        {
            levelScreenRenderer.DeregisterSpriteForRendering(this);

            return;
        }

        _actionSprite = LevelScreen.LemmingSpriteBank.GetActionSprite(_lemming);

        UpdatePosition();

        levelScreenRenderer.RegisterSpriteForRendering(this);
    }

    public void ResetPosition()
    {
        var levelScreenRenderer = LevelScreenRenderer.Instance.LevelRenderer;
        levelScreenRenderer.DeregisterSpriteForRendering(this);

        if (!_lemming.State.IsActive)
            return;

        _actionSprite = LevelScreen.LemmingSpriteBank.GetActionSprite(_lemming);

        UpdatePosition();

        levelScreenRenderer.RegisterSpriteForRendering(this);
    }

    public void SetDisplayTimer(bool displayTimer)
    {
        _shouldRenderCountDown = displayTimer;
    }

    public Rectangle GetSpriteBounds() => _spriteBounds.ToRectangle();

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int projectionX, int projectionY)
    {
        var renderDestination = new Rectangle(
            projectionX,
            projectionY,
            sourceRectangle.Width,
            sourceRectangle.Height);

        sourceRectangle.Y += _lemming.AnimationFrame * _actionSprite.SpriteSize.H;

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
            var countDownPositionOffset = new Point();

            FontBank.CountDownFont.RenderTextSpan(
                spriteBatch,
                _countDownCharBuffer,
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

        var dht = new DihedralTransformation(_lemming.Orientation, _lemming.FacingDirection);
        var anchorPoint = dht.Transform(_actionSprite.AnchorPoint, _actionSprite.SpriteSize);

        screenX += anchorPoint.X;
        screenY += anchorPoint.Y;

        for (var i = 0; i < EngineConstants.NumberOfParticles; i++)
        {
            var offset = ParticleHelper.GetParticleOffsets(_lemming.ParticleTimer, i);

            if (offset.X == -128 || offset.Y == -128)
                continue;

            var color = explosionParticleColors.At(i & EngineConstants.NumberOfExplosionParticleColorsMask);

            destRectangle.X = screenX + offset.X;
            destRectangle.Y = screenY + offset.Y;

            spriteBatch.FillRect(
                destRectangle,
                color);
        }
    }

    public void Dispose()
    {
        _lemming = null!;
        _actionSprite = null!;

        GC.SuppressFinalize(this);
    }

    [InlineArray(NumberOfCountDownChars)]
    private struct CountDownCharBuffer
    {
        private char _0;
    }
}
