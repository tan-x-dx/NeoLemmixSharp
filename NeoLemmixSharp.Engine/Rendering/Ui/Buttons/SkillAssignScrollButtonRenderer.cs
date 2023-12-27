using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

namespace NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

public sealed class SkillAssignScrollButtonRenderer : ControlPanelButtonRenderer
{
	private readonly SkillAssignScrollButton _skillAssignScrollButton;
	private readonly int _iconX;

	public SkillAssignScrollButtonRenderer(ControlPanelSpriteBank spriteBank, SkillAssignScrollButton skillAssignScrollButton)
		: base(spriteBank)
	{
		_skillAssignScrollButton = skillAssignScrollButton;
		_iconX = skillAssignScrollButton.Delta < 0
			? PanelHelpers.SkillAssignScrollLeftX
			: PanelHelpers.SkillAssignScrollRightX;
	}

	public override void Render(SpriteBatch spriteBatch)
	{
		var destRectangle = new Rectangle(
			_skillAssignScrollButton.ScreenX,
			_skillAssignScrollButton.ScreenY,
			_skillAssignScrollButton.ScreenWidth,
			_skillAssignScrollButton.ScreenHeight);

		spriteBatch.Draw(
			PanelTexture,
			destRectangle,
			PanelHelpers.GetRectangleForCoordinates(_skillAssignScrollButton.SkillPanelFrame, PanelHelpers.PanelBackgroundsY),
			RenderingLayers.ControlPanelButtonLayer);

		spriteBatch.Draw(
			PanelTexture,
			destRectangle,
			PanelHelpers.GetRectangleForCoordinates(_iconX, PanelHelpers.SkillAssignScrollY),
			RenderingLayers.ControlPanelSkillCountEraseLayer);
	}
}