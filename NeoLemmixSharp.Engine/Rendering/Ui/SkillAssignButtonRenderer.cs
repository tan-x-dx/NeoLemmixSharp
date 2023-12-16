using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Level.ControlPanel;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

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

    private readonly SkillCountDigitFont _skillCountDigitFont;

    public SkillAssignButtonRenderer(
        ControlPanelSpriteBank spriteBank,
        SkillAssignButton skillAssignButton)
    {
        _skillCountErase = spriteBank.GetTexture(ControlPanelTexture.PanelSkillCountErase);
        _skillPanels = spriteBank.GetTexture(ControlPanelTexture.PanelSkillPanels);
        _skillSelected = spriteBank.GetTexture(ControlPanelTexture.PanelSkillSelected);

        _skillCountDigitFont = FontBank.SkillCountDigitFont;

        _skillAssignButton = skillAssignButton;

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
        _skillIcon = spriteBank.GetTexture(ControlPanelTexture.WhitePixel);
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

        _skillCountDigitFont.RenderTextSpan(
            spriteBatch,
            _skillAssignButton.SkillCountChars,
            destRectangle.X + dx,
            destRectangle.Y + _skillAssignButton.ScaleMultiplier,
            _skillAssignButton.ScaleMultiplier,
            Color.White);
    }
}