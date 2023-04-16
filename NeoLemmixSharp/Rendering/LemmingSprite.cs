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

    public Texture2D RenderTexture => ActionSprite.Texture;

    public Rectangle GetLocationRectangle()
    {
        var actionSprite = ActionSprite;
        return new Rectangle(_lemming.LevelPosition - actionSprite.AnchorPoint, actionSprite.Size);
    }

    public Rectangle GetTextureSourceRectangle()
    {
        return ActionSprite.GetSourceRectangleForFrame(_lemming.AnimationFrame);
    }

    /*public void Render(SpriteBatch spriteBatch)
    {
        var actionSprite = _lemming.FacingDirection.ChooseActionSprite(_lemming.CurrentAction.ActionSpriteBundle, _lemming.Orientation);

        var rect = new Rectangle(
            _lemming.LevelPosition - actionSprite.AnchorPoint,
            actionSprite.Size);

        var viewport = LevelScreen.CurrentLevel.Viewport;

        if (!viewport.GetRenderDestinationRectangle(rect, out var renderDestination))
            return;

        spriteBatch.Draw(
            actionSprite.Texture,
            renderDestination,
            actionSprite.GetSourceRectangleForFrame(_lemming.AnimationFrame),
            Color.White);

        viewport.GetRenderDestinationRectangle(new Rectangle(_lemming.LevelPosition - new Point(1, 1), new Point(3, 3)), out renderDestination);

        var spriteBank = LevelScreen.CurrentLevel.SpriteBank;
        spriteBatch.Draw(
            spriteBank.AnchorTexture,
            renderDestination,
            Color.White);
    }*/

    public void Dispose()
    {
    }
}