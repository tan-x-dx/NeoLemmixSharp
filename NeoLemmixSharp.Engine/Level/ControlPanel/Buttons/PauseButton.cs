using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class PauseButton : ControlPanelButton
{
	public PauseButton(int skillPanelFrame) : base(skillPanelFrame)
	{
	}

	public override void OnPress()
	{
		LevelScreen.UpdateScheduler.PausePress();
	}

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new ControlPanelButtonRenderer(spriteBank, this, PanelHelpers.PauseButtonX, PanelHelpers.ButtonIconsY);
	}
}