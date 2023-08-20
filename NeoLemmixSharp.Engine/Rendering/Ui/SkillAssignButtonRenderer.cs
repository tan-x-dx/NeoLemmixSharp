using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Engine.ControlPanel;
using NeoLemmixSharp.Engine.Engine.Skills;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public sealed class SkillAssignButtonRenderer : ControlPanelButtonRenderer
{
    private readonly SkillAssignButton _skillAssignButton;
    private readonly SkillSetManager _skillSetManager;

    private readonly Texture2D _skillPanels;
    private readonly Texture2D _skillSelected;
    private readonly Texture2D _skillCountErase;
    private readonly Texture2D _skillIcon;
    private readonly Rectangle _skillIconSourceRectangle;
    private readonly int _skillIconWidth;
    private readonly int _skillIconHeight;

    private readonly INeoLemmixFont _skillCountDigitFont;

    private readonly int[] _skillCountChars;

    public SkillAssignButtonRenderer(
        ControlPanelSpriteBank spriteBank,
        FontBank fontBank,
        SkillAssignButton skillAssignButton,
        SkillSetManager skillSetManager)
    {
        _skillCountErase = spriteBank.GetTexture("panel/skill_count_erase");
        _skillPanels = spriteBank.GetTexture("panel/skill_panels");
        _skillSelected = spriteBank.GetTexture("panel/skill_selected");

        _skillCountDigitFont = fontBank.SkillCountDigitFont;

        _skillAssignButton = skillAssignButton;
        _skillSetManager = skillSetManager;

        _skillCountChars = new int[2];

        // HOLY SHIT THIS IS TERRIBLE CODE
        // TODO REFACTOR THE FUCK OUT OF THIS WHEN PROPER SPRITES ARE CREATED FOR SKILL ASSIGN BUTTONS
        /*  try
          {
              var lemmingActionSpriteBundle = spriteBank.GetLemmingActionSpriteBundle(_skillAssignButton.LemmingSkill.LemmingSkillName);
              var sprite = lemmingActionSpriteBundle.DownRightSprite;
              _skillIcon = sprite.Texture;
              _skillIconSourceRectangle = sprite.GetSourceRectangleForFrame(0);
              _skillIconWidth = sprite.SpriteWidth;
              _skillIconHeight = sprite.SpriteHeight;
          }
          catch (KeyNotFoundException) // goddamn
          {*/
        _skillIcon = spriteBank.GetTexture("WhitePixel");
        _skillIconSourceRectangle = new Rectangle(0, 0, 1, 1);
        _skillIconWidth = 1;
        _skillIconHeight = 1;
        //}
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

        spriteBatch.Draw(
            _skillPanels,
            destRectangle,
            GetPanelButtonBackgroundSourceRectangle(_skillAssignButton.SkillPanelFrame),
            RenderingLayers.ControlPanelButtonLayer);

        spriteBatch.Draw(
            _skillCountErase,
            destRectangle,
            new Rectangle(0, 0, _skillCountErase.Width, _skillCountErase.Height),
            RenderingLayers.ControlPanelSkillCountEraseLayer);

        if (_skillAssignButton.IsSelected)
        {
            spriteBatch.Draw(
                _skillSelected,
                destRectangle,
                new Rectangle(0, 0, _skillSelected.Width, _skillSelected.Height),
                RenderingLayers.ControlPanelSkillCountEraseLayer); // Can reuse this layer since the sprites shouldn't overlap anyway
        }

        var skillIconDestRectangle = new Rectangle(
            _skillAssignButton.ScreenX,
            _skillAssignButton.ScreenY + 6 * _skillAssignButton.ScaleMultiplier,
            _skillIconWidth * _skillAssignButton.ScaleMultiplier,
            _skillIconHeight * _skillAssignButton.ScaleMultiplier);

        spriteBatch.Draw(
            _skillIcon,
            skillIconDestRectangle,
            _skillIconSourceRectangle,
            RenderingLayers.ControlPanelSkillIconLayer);

        RenderSkillCounts(spriteBatch, destRectangle);
    }

    private void RenderSkillCounts(
        SpriteBatch spriteBatch,
        Rectangle destRectangle)
    {
        var dx = 3 * _skillAssignButton.ScaleMultiplier;
        var skillTrackingData = _skillSetManager.GetSkillTrackingData(_skillAssignButton.SkillAssignButtonId)!;
        var numberOfSkillsAvailable = skillTrackingData.SkillCount;

        if (numberOfSkillsAvailable >= 100)
        {
            _skillCountChars[0] = SkillCountDigitFont.InfinityGlyph;
            _skillCountChars[1] = ' ';
        }
        else
        {
            var unit = numberOfSkillsAvailable % 10;
            _skillCountChars[1] = unit + '0';

            var tens = numberOfSkillsAvailable / 10;
            if (tens > 0)
            {
                _skillCountChars[0] = tens + '0';
            }
            else
            {
                _skillCountChars[0] = ' ';
            }
        }

        _skillCountDigitFont.RenderTextSpan(
            spriteBatch,
            new ReadOnlySpan<int>(_skillCountChars),
            destRectangle.X + dx,
            destRectangle.Y + _skillAssignButton.ScaleMultiplier,
            _skillAssignButton.ScaleMultiplier,
            Color.White);
    }
}