using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class PaddingButton : ControlPanelButton
{
	public PaddingButton(int buttonId)
		: base(
			buttonId,
			PanelHelpers.PaddingButtonX,
			EmptyButtonAction.Instance,
			-1,
			-1)
	{
	}

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new PaddingButtonRenderer(spriteBank, this);
	}
}