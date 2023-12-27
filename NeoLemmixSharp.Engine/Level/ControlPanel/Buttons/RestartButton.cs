using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class RestartButton : ControlPanelButton
{
	public RestartButton(int skillPanelFrame) : base(skillPanelFrame)
	{
	}

	public override void OnPress()
	{
	}

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new ControlPanelButtonRenderer(spriteBank, this, PanelHelpers.ReplayButtonX, PanelHelpers.ButtonIconsY);
	}
}