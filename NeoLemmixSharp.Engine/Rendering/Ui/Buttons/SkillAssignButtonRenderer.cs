using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

public sealed class SkillAssignButtonRenderer : ControlPanelButtonRenderer
{
    private readonly SkillAssignButton _skillAssignButton;

    private readonly Texture2D _skillPanels;
    private readonly Texture2D _skillSelected;
    private readonly Texture2D _skillIcons;

    private readonly SkillCountDigitFont _skillCountDigitFont;
    private readonly int _skillY;

    public SkillAssignButtonRenderer(
        ControlPanelSpriteBank spriteBank,
        SkillAssignButton skillAssignButton)
    {
        _skillAssignButton = skillAssignButton;

        _skillPanels = spriteBank.GetTexture(ControlPanelTexture.Panel);
        _skillSelected = spriteBank.GetTexture(ControlPanelTexture.PanelSkillSelected);
        _skillIcons = spriteBank.GetTexture(ControlPanelTexture.PanelSkills);

        _skillCountDigitFont = FontBank.SkillCountDigitFont;

        var skillTrackingData = _skillAssignButton.SkillTrackingData;
        _skillY = GetSkillY(skillTrackingData.Skill);
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
            PanelHelpers.GetRectangleForCoordinates(_skillAssignButton.SkillPanelFrame, 0),
            RenderingLayers.ControlPanelButtonLayer);

        spriteBatch.Draw(
            _skillPanels,
            destRectangle,
            PanelHelpers.GetRectangleForCoordinates(1, 2),
            RenderingLayers.ControlPanelSkillCountEraseLayer);

        var skillIconDestRectangle = new Rectangle(
            _skillAssignButton.ScreenX,
            _skillAssignButton.ScreenY,
            PanelHelpers.ControlPanelButtonPixelWidth * _skillAssignButton.ScaleMultiplier,
            PanelHelpers.ControlPanelButtonPixelHeight * _skillAssignButton.ScaleMultiplier);

        spriteBatch.Draw(
            _skillIcons,
            skillIconDestRectangle,
            new Rectangle(0, _skillY * PanelHelpers.ControlPanelButtonPixelHeight,
                PanelHelpers.ControlPanelButtonPixelWidth, PanelHelpers.ControlPanelButtonPixelHeight),
            RenderingLayers.ControlPanelSkillIconLayer);

        RenderSkillCounts(spriteBatch, destRectangle);

        if (_skillAssignButton.IsSelected)
        {
            spriteBatch.Draw(
                _skillSelected,
                destRectangle,
                new Rectangle(0, 0, _skillSelected.Width, _skillSelected.Height),
                RenderingLayers
                    .ControlPanelSkillCountEraseLayer); // Can reuse this layer since the sprites shouldn't overlap anyway
        }
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

    private static int GetSkillY(LemmingSkill skill) => skill.Id switch
    {
        LevelConstants.BasherSkillId => 16,
        LevelConstants.BlockerSkillId => 11,
        LevelConstants.BomberSkillId => 9,
        LevelConstants.BuilderSkillId => 13,
        LevelConstants.ClimberSkillId => 4,
        LevelConstants.ClonerSkillId => 20,
        LevelConstants.DiggerSkillId => 19,
        LevelConstants.DisarmerSkillId => 8,
        LevelConstants.FencerSkillId => 17,
        LevelConstants.FloaterSkillId => 6,
        LevelConstants.GliderSkillId => 7,
        LevelConstants.JumperSkillId => 1,
        LevelConstants.LasererSkillId => 15,
        LevelConstants.MinerSkillId => 18,
        LevelConstants.PlatformerSkillId => 12,
        LevelConstants.ShimmierSkillId => 2,
        LevelConstants.SliderSkillId => 3,
        LevelConstants.StackerSkillId => 14,
        LevelConstants.StonerSkillId => 10,
        LevelConstants.SwimmerSkillId => 5,
        LevelConstants.WalkerSkillId => 0,

        _ => throw new ArgumentOutOfRangeException(nameof(skill), skill, "Cannot get icon for skill")
    };
}