using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

namespace NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

public sealed class SkillAssignButtonRenderer : ControlPanelButtonRenderer
{
    private readonly Texture2D _skillIcons;

    private readonly int _skillY;

    public SkillAssignButtonRenderer(
        ControlPanelSpriteBank spriteBank,
        SkillAssignButton skillAssignButton)
        : base(spriteBank, skillAssignButton, 0, 0)
    {
        _skillIcons = spriteBank.PanelSkills;

        var skillType = skillAssignButton.SkillType;
        _skillY = GetSkillY(skillType);
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        if (!ControlPanelButton.ShouldRender)
            return;

        var destRectangle = GetDestinationRectangle();

        spriteBatch.Draw(
            PanelTexture,
            destRectangle,
            PanelHelpers.GetRectangleForCoordinates(ControlPanelButton.SkillPanelFrame, PanelHelpers.PanelBackgroundY));

        spriteBatch.Draw(
            PanelTexture,
            destRectangle,
            PanelHelpers.GetRectangleForCoordinates(PanelHelpers.SkillIconDoubleMaskX, PanelHelpers.SkillIconMaskY));

        var skillIconDestRectangle = new Rectangle(
            ControlPanelButton.X,
            ControlPanelButton.Y,
            PanelHelpers.ControlPanelButtonPixelWidth,
            PanelHelpers.ControlPanelButtonPixelHeight);

        spriteBatch.Draw(
            _skillIcons,
            skillIconDestRectangle,
            new Rectangle(0, _skillY * PanelHelpers.ControlPanelButtonPixelHeight, PanelHelpers.ControlPanelButtonPixelWidth, PanelHelpers.ControlPanelButtonPixelHeight));

        RenderDigits(spriteBatch, destRectangle);

        RenderSelected(spriteBatch, destRectangle);
    }

    private static int GetSkillY(LemmingSkillType skillType) => skillType switch
    {
        LemmingSkillType.BasherSkill => 16,
        LemmingSkillType.BlockerSkill => 11,
        LemmingSkillType.BomberSkill => 9,
        LemmingSkillType.BuilderSkill => 13,
        LemmingSkillType.ClimberSkill => 4,
        LemmingSkillType.ClonerSkill => 20,
        LemmingSkillType.DiggerSkill => 19,
        LemmingSkillType.DisarmerSkill => 8,
        LemmingSkillType.FencerSkill => 17,
        LemmingSkillType.FloaterSkill => 6,
        LemmingSkillType.GliderSkill => 7,
        LemmingSkillType.JumperSkill => 1,
        LemmingSkillType.LasererSkill => 15,
        LemmingSkillType.MinerSkill => 18,
        LemmingSkillType.PlatformerSkill => 12,
        LemmingSkillType.ShimmierSkill => 2,
        LemmingSkillType.SliderSkill => 3,
        LemmingSkillType.StackerSkill => 14,
        LemmingSkillType.StonerSkill => 10,
        LemmingSkillType.SwimmerSkill => 5,
        LemmingSkillType.WalkerSkill => 0,

        _ => Helpers.ThrowUnknownEnumValueException<LemmingSkillType, int>(skillType)
    };
}