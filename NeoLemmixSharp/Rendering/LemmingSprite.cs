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

    public void Render(SpriteBatch spriteBatch)
    {
        var actionSprite = _lemming.Orientation.GetActionSprite(_lemming.CurrentAction.ActionSpriteBundle, _lemming.FacingDirection);

        var rect = new Rectangle(
            _lemming.X - actionSprite.AnchorPointX,
            _lemming.Y - actionSprite.AnchorPointY,
            actionSprite.SpriteWidth,
            actionSprite.SpriteHeight);

        var viewport = LevelScreen.CurrentLevel!.Viewport;

        if (!viewport.GetRenderDestinationRectangle(rect, out var renderDestination))
            return;

        spriteBatch.Draw(
            actionSprite.Texture,
            renderDestination,
            actionSprite.GetSourceRectangleForFrame(_lemming.AnimationFrame),
            Color.White);

        var spriteBank = LevelScreen.CurrentLevel.SpriteBank;

        viewport.GetRenderDestinationRectangle(new Rectangle(_lemming.X - 1, _lemming.Y - 1, 3, 3), out renderDestination);

        spriteBatch.Draw(
            spriteBank.AnchorTexture,
            renderDestination,
            Color.White);
    }

    public void Dispose()
    {
    }
}