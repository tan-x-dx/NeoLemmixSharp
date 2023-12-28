using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class PaddingButton : ControlPanelButton
{
	public PaddingButton()
		: base(0, EmptyButtonAction.Instance, PanelHelpers.PaddingButtonX, PanelHelpers.PaddingButtonY)
	{
	}

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new PaddingButtonRenderer(spriteBank, this);
	}
}