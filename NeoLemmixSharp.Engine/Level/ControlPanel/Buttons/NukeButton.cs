using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class NukeButton : ControlPanelButton
{
	public NukeButton(int skillPanelFrame) : base(skillPanelFrame)
	{
	}

	public override void OnDoubleTap()
	{
	}
	
	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new ControlPanelButtonRenderer(spriteBank, this, PanelHelpers.NukeButtonX, PanelHelpers.ButtonIconsY);
	}
}