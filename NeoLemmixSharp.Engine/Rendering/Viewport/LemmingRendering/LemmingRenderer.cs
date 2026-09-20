using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.Lemmings;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingRenderer : IViewportObjectRenderer
{
    private Lemming Lemming => LevelScreen.LemmingManager.GetLemming(ItemId);

    private RectangularRegion _spriteBounds;

    public int RendererId { get; set; }
    public int ItemId { get; }

    public RectangularRegion CurrentBounds => _spriteBounds;

    public LemmingRenderer(int lemmingId)
    {
        ItemId = lemmingId;
    }

    public void UpdatePosition()
    {
        var lemming = Lemming;
        var actionSprite = LevelScreen.LemmingSpriteBank.GetActionSprite(lemming);
        var dht = lemming.DihedralTransformation;
        var spriteOffset = dht.Transform(actionSprite.AnchorPoint, actionSprite.SpriteSize);
        var p = lemming.AnchorPosition - spriteOffset;

        _spriteBounds = new RectangularRegion(p, dht.Transform(actionSprite.SpriteSize));

        LevelScreenRenderer.Instance.LevelRenderer.UpdateSpritePosition(this);
    }

    public void UpdateLemmingState(bool shouldRender)
    {
        if (Lemming.IsSimulation)
            return;

        var levelScreenRenderer = LevelScreenRenderer.Instance.LevelRenderer;
        if (!shouldRender)
        {
            levelScreenRenderer.DeregisterSpriteForRendering(this);

            return;
        }

        UpdatePosition();

        levelScreenRenderer.RegisterSpriteForRendering(this);
    }

    public void ResetPosition()
    {
        var levelScreenRenderer = LevelScreenRenderer.Instance.LevelRenderer;
        levelScreenRenderer.DeregisterSpriteForRendering(this);

        if (!Lemming.IsActive)
            return;

        UpdatePosition();

        levelScreenRenderer.RegisterSpriteForRendering(this);
    }

    public Rectangle GetSpriteBounds() => _spriteBounds.ToRectangle();

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int projectionX, int projectionY)
    {
        var lemming = Lemming;
        var actionSprite = LevelScreen.LemmingSpriteBank.GetActionSprite(lemming);

        RenderLemmingSprite(spriteBatch, sourceRectangle, projectionX, projectionY, lemming, actionSprite);

        /* spriteBatch.Draw(
              actionSprite.Texture,
              renderDestination,
              actionSprite.GetSourceRectangleForFrame(sourceRectangle, lemming.AnimationFrame),
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

        if (lemming.CountDownTimer > 0)
        {
            RenderCountDown(spriteBatch, projectionX, projectionY, lemming);
        }

        if (lemming.ParticleTimer > 0)
        {
            RenderParticles(spriteBatch, projectionX, projectionY, lemming, actionSprite);
        }
        /*
        if (lemming.Id < 7)
        {
            FontBank.MenuFont.RenderText(
                spriteBatch,
                lemming.LevelPosition.ToString(),
                screenX - 10,
                screenY - MenuFont.GlyphHeight * 2,
                1,
                Color.White);
        }*/
    }

    private static void RenderLemmingSprite(
        SpriteBatch spriteBatch,
        Rectangle sourceRectangle,
        int projectionX,
        int projectionY,
        Lemming lemming,
        LemmingActionSprite actionSprite)
    {
        var renderDestination = new Rectangle(
            projectionX,
            projectionY,
            sourceRectangle.Width,
            sourceRectangle.Height);

        sourceRectangle.Y += lemming.AnimationFrame * actionSprite.SpriteSize.H;

        actionSprite.RenderLemming(spriteBatch, lemming, sourceRectangle, renderDestination);
    }

    private static void RenderCountDown(
        SpriteBatch spriteBatch,
        int projectionX,
        int projectionY,
        Lemming lemming)
    {
        var countDownPositionOffset = new Point();

        FontBank.CountDownFont.RenderTextSpan(
            spriteBatch,
            lemming.CountDownCharBuffer(),
            projectionX + countDownPositionOffset.X,
            projectionY + countDownPositionOffset.Y,
            1,
            Color.White);
    }

    private static void RenderParticles(
        SpriteBatch spriteBatch,
        int projectionX,
        int projectionY,
        Lemming lemming,
        LemmingActionSprite actionSprite)
    {
        var destRectangle = new Rectangle(0, 0, 1, 1);
        var explosionParticleColors = EngineConstants.GetExplosionParticleColors();

        var dht = lemming.DihedralTransformation;
        var anchorPoint = dht.Transform(actionSprite.AnchorPoint, actionSprite.SpriteSize);

        projectionX += anchorPoint.X;
        projectionY += anchorPoint.Y;

        for (var i = 0; i < EngineConstants.NumberOfParticles; i++)
        {
            var offset = ParticleHelper.GetParticleOffsets(lemming.ParticleTimer, i);

            if (offset.X == -128 || offset.Y == -128)
                continue;

            var color = explosionParticleColors.At(i & EngineConstants.NumberOfExplosionParticleColorsMask);

            destRectangle.X = projectionX + offset.X;
            destRectangle.Y = projectionY + offset.Y;

            spriteBatch.FillRect(
                destRectangle,
                color);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
