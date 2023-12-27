using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class SkillAssignScrollButton : ControlPanelButton
{
	private readonly int _delta;

	public SkillAssignScrollButton(int skillPanelFrame, int delta)
		: base(skillPanelFrame)
	{
		_delta = delta;
	}

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		var iconX = _delta < 0
				? PanelHelpers.SkillAssignScrollLeftX
				: PanelHelpers.SkillAssignScrollRightX;

		return new ControlPanelButtonRenderer(spriteBank, this, iconX, PanelHelpers.SkillAssignScrollY);
	}

	public override void OnMouseDown()
	{
		LevelScreen.LevelControlPanel.ChangeSkillAssignButtonScroll(_delta);
	}
}