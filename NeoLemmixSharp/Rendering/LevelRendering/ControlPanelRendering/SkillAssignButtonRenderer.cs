using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.ControlPanel;
using NeoLemmixSharp.Rendering.Text;

namespace NeoLemmixSharp.Rendering.LevelRendering.ControlPanelRendering;

public sealed class SkillAssignButtonRenderer : ControlPanelButtonRenderer
{
    private readonly SkillAssignButton _skillAssignButton;

    private readonly Texture2D _skillPanels;
    private readonly Texture2D _skillSelected;
    private readonly Texture2D _skillCountErase;
    private readonly Texture2D _skillIcon;
    private readonly Rectangle _skillIconSourceRectangle;
    private readonly int _skillIconWidth;
    private readonly int _skillIconHeight;

    private readonly INeoLemmixFont _skillCountDigitFont;

    public SkillAssignButtonRenderer(
        SpriteBank spriteBank,
        FontBank fontBank,
        SkillAssignButton skillAssignButton)
    {
        _skillCountErase = spriteBank.TextureLookup["panel/skill_count_erase"];
        _skillPanels = spriteBank.TextureLookup["panel/skill_panels"];
        _skillSelected = spriteBank.TextureLookup["panel/skill_selected"];

        _skillCountDigitFont = fontBank.SkillCountDigitFont;

        _skillAssignButton = skillAssignButton;

        if (spriteBank.LemmingActionSpriteBundleLookup.TryGetValue(_skillAssignButton.LemmingSkill.LemmingSkillName, out var lemmingActionSpriteBundle))
        {
            var sprite = lemmingActionSpriteBundle.DownRightSprite;
            _skillIcon = sprite.Texture;
            _skillIconSourceRectangle = sprite.GetSourceRectangleForFrame(0);
            _skillIconWidth = sprite.SpriteWidth;
            _skillIconHeight = sprite.SpriteHeight;
        }
        else
        {
            _skillIcon = spriteBank.WhitePixelTexture;
            _skillIconSourceRectangle = new Rectangle(0, 0, 1, 1);
            _skillIconWidth = 1;
            _skillIconHeight = 1;
        }
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        if (!_skillAssignButton.ShouldRender)
            return;

        var destRectangle = new Rectangle(
                  _skillAssignButton.ScreenX,
                  _skillAssignButton.ScreenY,
                  _skillAssignButton.ScreenWidth,
                  _skillAssignButton.ScreenHeight);

        spriteBatch.Draw(_skillPanels, destRectangle, GetPanelButtonBackgroundSourceRectangle(_skillAssignButton.SkillPanelFrame), Color.White);
        spriteBatch.Draw(_skillCountErase, destRectangle, Color.White);

        var skillIconDestRectangle = new Rectangle(
            _skillAssignButton.ScreenX,
            _skillAssignButton.ScreenY + 6 * _skillAssignButton.ScaleMultiplier,
            _skillIconWidth * _skillAssignButton.ScaleMultiplier,
            _skillIconHeight * _skillAssignButton.ScaleMultiplier);
        spriteBatch.Draw(_skillIcon, skillIconDestRectangle, _skillIconSourceRectangle, Color.White);

        var dx = 3 * _skillAssignButton.ScaleMultiplier;

        _skillCountDigitFont.RenderText(
            spriteBatch,
            _skillAssignButton.NumberOfSkillsAvailable.ToString(),
            destRectangle.X + dx,
            destRectangle.Y + _skillAssignButton.ScaleMultiplier,
            _skillAssignButton.ScaleMultiplier);

        if (_skillAssignButton.IsSelected)
        {
            spriteBatch.Draw(_skillSelected, destRectangle, Color.White);
        }
    }
}