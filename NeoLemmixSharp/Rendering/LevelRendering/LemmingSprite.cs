using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;

namespace NeoLemmixSharp.Rendering.LevelRendering;

public sealed class LemmingSprite : ISprite
{
    private readonly Lemming _lemming;

    public LemmingSprite(Lemming lemming)
    {
        _lemming = lemming;
    }

    private ActionSprite ActionSprite => _lemming.FacingDirection.ChooseActionSprite(_lemming.CurrentAction.ActionSpriteBundle, _lemming.Orientation);

    public Rectangle GetLocationRectangle()
    {
        var actionSprite = ActionSprite;
        var p = _lemming.LevelPosition - actionSprite.AnchorPoint;
        var s = actionSprite.Size;

        return new Rectangle(p.X, p.Y, s.X, s.Y);
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, int x, int y, int scaleMultiplier)
    {
        RenderAtPosition(spriteBatch, ActionSprite.GetSourceRectangleForFrame(_lemming.AnimationFrame), x, y, scaleMultiplier);
    }

    public void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int x, int y, int scaleMultiplier)
    {
        var actionSprite = ActionSprite;

        var renderDestination = new Rectangle(
            x,
            y,
            actionSprite.SpriteWidth * scaleMultiplier,
            actionSprite.SpriteHeight * scaleMultiplier);

        spriteBatch.Draw(
            actionSprite.Texture,
            renderDestination,
            actionSprite.GetSourceRectangleForFrame(sourceRectangle, _lemming.AnimationFrame),
            Color.White,
            0.0f,
            new Vector2(),
            SpriteEffects.None,
            RenderingLayers.LemmingRenderLayer);

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