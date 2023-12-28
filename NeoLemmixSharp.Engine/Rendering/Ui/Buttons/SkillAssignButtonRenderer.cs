using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level;
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
		_skillIcons = spriteBank.GetTexture(ControlPanelTexture.PanelSkills);

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
			PanelHelpers.GetRectangleForCoordinates(ControlPanelButton.SkillPanelFrame, PanelHelpers.PanelBackgroundsY),
			RenderingLayers.ControlPanelButtonLayer);

		spriteBatch.Draw(
			PanelTexture,
			destRectangle,
			PanelHelpers.GetRectangleForCoordinates(PanelHelpers.SkillIconMaskX, PanelHelpers.SkillIconMaskY),
			RenderingLayers.ControlPanelSkillCountEraseLayer);

		var scaleMultiplier = LevelScreen.LevelControlPanel.ControlPanelScale;

		var skillIconDestRectangle = new Rectangle(
			ControlPanelButton.ScreenX,
			ControlPanelButton.ScreenY,
			PanelHelpers.ControlPanelButtonPixelWidth * scaleMultiplier,
			PanelHelpers.ControlPanelButtonPixelHeight * scaleMultiplier);

		spriteBatch.Draw(
			_skillIcons,
			skillIconDestRectangle,
			new Rectangle(0, _skillY * PanelHelpers.ControlPanelButtonPixelHeight,
				PanelHelpers.ControlPanelButtonPixelWidth, PanelHelpers.ControlPanelButtonPixelHeight),
			RenderingLayers.ControlPanelSkillIconLayer);

		RenderDigits(spriteBatch, destRectangle);

		RenderSelected(spriteBatch, destRectangle);
	}

	private static int GetSkillY(int skillId) => skillId switch
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

		_ => throw new ArgumentOutOfRangeException(nameof(skillId), skillId, "Unknown skill id")
	};
}