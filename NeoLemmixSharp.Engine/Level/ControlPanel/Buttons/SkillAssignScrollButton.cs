using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class SkillAssignScrollButton : ControlPanelButton
{
	public int Delta { get; }

	public SkillAssignScrollButton(int skillPanelFrame, int delta)
		: base(skillPanelFrame)
	{
		Delta = delta;
	}

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new SkillAssignScrollButtonRenderer(spriteBank, this);
	}


	public override void OnMouseDown()
	{
		LevelScreen.LevelControlPanel.ChangeSkillAssignButtonScroll(Delta);
	}
}