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
        EngineConstants.BasherSkillId => 16,
        EngineConstants.BlockerSkillId => 11,
        EngineConstants.BomberSkillId => 9,
        EngineConstants.BuilderSkillId => 13,
        EngineConstants.ClimberSkillId => 4,
        EngineConstants.ClonerSkillId => 20,
        EngineConstants.DiggerSkillId => 19,
        EngineConstants.DisarmerSkillId => 8,
        EngineConstants.FencerSkillId => 17,
        EngineConstants.FloaterSkillId => 6,
        EngineConstants.GliderSkillId => 7,
        EngineConstants.JumperSkillId => 1,
        EngineConstants.LasererSkillId => 15,
        EngineConstants.MinerSkillId => 18,
        EngineConstants.PlatformerSkillId => 12,
        EngineConstants.ShimmierSkillId => 2,
        EngineConstants.SliderSkillId => 3,
        EngineConstants.StackerSkillId => 14,
        EngineConstants.StonerSkillId => 10,
        EngineConstants.SwimmerSkillId => 5,
        EngineConstants.WalkerSkillId => 0,

        _ => throw new ArgumentOutOfRangeException(nameof(skillId), skillId, "Unknown skill id")
    };
}