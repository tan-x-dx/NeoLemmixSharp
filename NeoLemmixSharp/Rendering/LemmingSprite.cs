using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;

namespace NeoLemmixSharp.Rendering;

public sealed class LemmingSprite : NeoLemmixSprite
{
    private readonly Lemming _lemming;
    // private Texture2D _lemmingTexture;

    public LemmingSprite(Lemming lemming)
    {
        _lemming = lemming;
    }

    public override Texture2D GetTexture()
    {
        throw new System.NotImplementedException();
    }

    public override Rectangle GetBoundingBox()
    {
        throw new System.NotImplementedException();
    }

    public override LevelPosition GetAnchorPoint() => _lemming.LevelPosition;

    public override bool ShouldRender => IsOnScreen();
    public override void Render(SpriteBatch spriteBatch)
    {
        var spriteBank = LevelScreen.CurrentLevel!.SpriteBank;

        var skillSprite = spriteBank.GetSkillSprite(_lemming);

        spriteBatch.Draw(
            skillSprite.GetTexture(),
            GetSpriteDestinationRectangle(skillSprite),
            skillSprite.GetSourceRectangleForFrame(_lemming.AnimationFrame),
            Color.White);

        spriteBatch.Draw(
            spriteBank.GetAnchorTexture(),
            GetAnchorPointSpriteDestinationRectangle(),
            Color.White);
    }

    private Rectangle GetSpriteDestinationRectangle(SkillSprite skillSprite)
    {
        var zoom = LevelScreen.CurrentLevel!.Viewport.Zoom;

        var spriteAnchor = skillSprite.GetAnchorPoint();
        var x0 = _lemming.X - spriteAnchor.X;
        var y0 = _lemming.Y - spriteAnchor.Y;

        return new Rectangle(x0 * zoom.ScaleMultiplier, y0 * zoom.ScaleMultiplier, skillSprite.SpriteWidth * zoom.ScaleMultiplier, skillSprite.SpriteHeight * zoom.ScaleMultiplier);
    }

    private Rectangle GetAnchorPointSpriteDestinationRectangle()
    {
        var zoom = LevelScreen.CurrentLevel!.Viewport.Zoom;

        return new Rectangle((_lemming.X - 1) * zoom.ScaleMultiplier, (_lemming.Y - 1) * zoom.ScaleMultiplier, 3 * zoom.ScaleMultiplier, 3 * zoom.ScaleMultiplier);
    }
}