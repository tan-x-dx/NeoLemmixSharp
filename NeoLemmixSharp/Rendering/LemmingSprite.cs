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

    private Rectangle GetBoundingBox() => new();

    private bool ShouldRender => LevelScreen.CurrentLevel!.Viewport.IsOnScreen(GetBoundingBox());
    public void Render(SpriteBatch spriteBatch)
    {
        var actionSprite = _lemming.Orientation.GetActionSprite(_lemming.CurrentAction.ActionSpriteBundle, _lemming.FacingDirection);

        var rect = new Rectangle(
            _lemming.X - actionSprite.AnchorPointX,
            _lemming.Y - actionSprite.AnchorPointY,
            actionSprite.SpriteWidth,
            actionSprite.SpriteHeight);

        var viewport = LevelScreen.CurrentLevel!.Viewport;

        if (!viewport.IsOnScreen(rect))
            return;

        spriteBatch.Draw(
            actionSprite.Texture,
            GetDestinationRectangle(viewport, rect.X, rect.Y, rect.Width, rect.Height),
            actionSprite.GetSourceRectangleForFrame(_lemming.AnimationFrame),
            Color.White);

        var spriteBank = LevelScreen.CurrentLevel.SpriteBank;

        spriteBatch.Draw(
            spriteBank.AnchorTexture,
            GetDestinationRectangle(viewport, _lemming.X - 1, _lemming.Y - 1, 3, 3),
            Color.White);
    }

    private static Rectangle GetDestinationRectangle(LevelViewPort viewport, int x, int y, int w, int h)
    {
        var x0 = (x - viewport.SourceX) * viewport.ScaleMultiplier + viewport.TargetX;
        var y0 = (y - viewport.SourceY) * viewport.ScaleMultiplier + viewport.TargetY;

        return new Rectangle(
            x0,
            y0,
            w * viewport.ScaleMultiplier,
            h * viewport.ScaleMultiplier);
    }

    public void Dispose()
    {
    }
}