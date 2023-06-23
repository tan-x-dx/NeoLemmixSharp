using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering.Level.Viewport.Lemming;

public sealed class LemmingRenderer : ILevelObjectRenderer
{
    private readonly Engine.Lemming _lemming;

    private ActionSprite _actionSprite;

    public LemmingRenderer(Engine.Lemming lemming)
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

    public Rectangle GetLocationRectangle()
    {
        var p = _lemming.LevelPosition - _actionSprite.AnchorPoint;

        return new Rectangle(p.X, p.Y, _actionSprite.SpriteWidth, _actionSprite.SpriteHeight);
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, int x, int y, int scaleMultiplier)
    {
        RenderAtPosition(spriteBatch, _actionSprite.GetSourceRectangleForFrame(_lemming.AnimationFrame), x, y, scaleMultiplier);
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int x, int y, int scaleMultiplier)
    {
        var renderDestination = new Rectangle(
            x,
            y,
            _actionSprite.SpriteWidth * scaleMultiplier,
            _actionSprite.SpriteHeight * scaleMultiplier);

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

        // var p = new Point(x - scaleMultiplier, y - scaleMultiplier);
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
    }
}