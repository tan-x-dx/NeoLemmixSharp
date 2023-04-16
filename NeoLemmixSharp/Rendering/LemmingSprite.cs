using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;

namespace NeoLemmixSharp.Rendering;

public sealed class LemmingSprite : IRenderable
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

    public void RenderAtPosition(SpriteBatch spriteBatch, int x, int y)
    {
        var scaleMultiplier = LevelScreen.CurrentLevel.Viewport.ScaleMultiplier;
        var actionSprite = ActionSprite;

        var renderDestination = new Rectangle(
            x,
            y,
            actionSprite.SpriteWidth * scaleMultiplier,
            actionSprite.SpriteHeight * scaleMultiplier);

        spriteBatch.Draw(
            actionSprite.Texture,
            renderDestination,
            actionSprite.GetSourceRectangleForFrame(_lemming.AnimationFrame),
            Color.White);

        /*  var x0 = 

          renderDestination = new Rectangle(_lemming.LevelPosition - new Point(1, 1), new Point(3 * scaleMultiplier, 3 * scaleMultiplier));

          var spriteBank = LevelScreen.CurrentLevel.SpriteBank;
          spriteBatch.Draw(
              spriteBank.AnchorTexture,
              renderDestination,
              Color.White);*/
    }

    public void Dispose()
    {
    }
}