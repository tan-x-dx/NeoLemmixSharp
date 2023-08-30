using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

public sealed class LemmingRenderer : IViewportObjectRenderer
{
    private Level.Lemmings.Lemming _lemming;
    private ActionSprite _actionSprite;

    public LemmingRenderer(Level.Lemmings.Lemming lemming)
    {
        _lemming = lemming;
    }

    public void UpdateLemmingState()
    {
        _actionSprite = LevelRenderer.Current.LemmingSpriteBank.GetActionSprite(
            _lemming.CurrentAction,
            _lemming.Orientation,
            _lemming.FacingDirection);
    }

    public Rectangle GetSpriteBounds()
    {
        var p = _lemming.LevelPosition - _actionSprite.AnchorPoint;

        return new Rectangle(p.X, p.Y, _actionSprite.SpriteWidth, _actionSprite.SpriteHeight);
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int screenX, int screenY, int scaleMultiplier)
    {
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
    }

    public void Dispose()
    {
        _lemming = null;
        _actionSprite = null;
    }
}