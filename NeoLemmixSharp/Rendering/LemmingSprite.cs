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

    public override Rectangle GetBoundingBox() => new();

    public override LevelPosition GetAnchorPoint() => _lemming.LevelPosition;

    public override bool ShouldRender => LevelScreen.CurrentLevel!.Viewport.IsVisible(GetBoundingBox());
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
        var viewport = LevelScreen.CurrentLevel!.Viewport;

        var spriteAnchor = skillSprite.GetAnchorPoint();
        var x0 = (_lemming.X - spriteAnchor.X - viewport.SourceX) * viewport.ScaleMultiplier + viewport.TargetX;
        var y0 = (_lemming.Y - spriteAnchor.Y - viewport.SourceY) * viewport.ScaleMultiplier + viewport.TargetY;

        return new Rectangle(
            x0,
            y0,
            skillSprite.SpriteWidth * viewport.ScaleMultiplier,
            skillSprite.SpriteHeight * viewport.ScaleMultiplier);
    }

    private Rectangle GetAnchorPointSpriteDestinationRectangle()
    {
        var viewport = LevelScreen.CurrentLevel!.Viewport;

        var x0 = (_lemming.X - 1 - viewport.SourceX) * viewport.ScaleMultiplier + viewport.TargetX;
        var y0 = (_lemming.Y - 1 - viewport.SourceY) * viewport.ScaleMultiplier + viewport.TargetY;

        return new Rectangle(
            x0,
            y0,
            3 * viewport.ScaleMultiplier,
            3 * viewport.ScaleMultiplier);
    }
}