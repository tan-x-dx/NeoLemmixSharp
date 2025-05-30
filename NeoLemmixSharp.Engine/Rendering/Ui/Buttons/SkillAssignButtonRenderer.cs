using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
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

        var skillId = skillAssignButton.SkillId;
        _skillY = GetSkillY(skillId);
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

    private static int GetSkillY(int skillId) => skillId switch
    {
        LemmingSkillConstants.BasherSkillId => 16,
        LemmingSkillConstants.BlockerSkillId => 11,
        LemmingSkillConstants.BomberSkillId => 9,
        LemmingSkillConstants.BuilderSkillId => 13,
        LemmingSkillConstants.ClimberSkillId => 4,
        LemmingSkillConstants.ClonerSkillId => 20,
        LemmingSkillConstants.DiggerSkillId => 19,
        LemmingSkillConstants.DisarmerSkillId => 8,
        LemmingSkillConstants.FencerSkillId => 17,
        LemmingSkillConstants.FloaterSkillId => 6,
        LemmingSkillConstants.GliderSkillId => 7,
        LemmingSkillConstants.JumperSkillId => 1,
        LemmingSkillConstants.LasererSkillId => 15,
        LemmingSkillConstants.MinerSkillId => 18,
        LemmingSkillConstants.PlatformerSkillId => 12,
        LemmingSkillConstants.ShimmierSkillId => 2,
        LemmingSkillConstants.SliderSkillId => 3,
        LemmingSkillConstants.StackerSkillId => 14,
        LemmingSkillConstants.StonerSkillId => 10,
        LemmingSkillConstants.SwimmerSkillId => 5,
        LemmingSkillConstants.WalkerSkillId => 0,

        _ => throw new ArgumentOutOfRangeException(nameof(skillId), skillId, "Unknown skill id")
    };
}