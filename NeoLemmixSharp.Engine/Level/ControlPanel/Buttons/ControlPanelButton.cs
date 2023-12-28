using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public class ControlPanelButton
{
	private const int NumberOfSkillPanels = 8;
	private const int SkillPanelFrameMask = NumberOfSkillPanels - 1;

	public int SkillPanelFrame { get; }
	public int ScreenX { get; set; }
	public int ScreenY { get; set; }
	public int ScreenWidth { get; set; }
	public int ScreenHeight { get; set; }

	public int IconX { get; }
	public int IconY { get; }

	public bool ShouldRender { get; set; } = true;
	public bool IsSelected { get; set; }

	public IButtonAction ButtonAction { get; protected init; }

	protected ControlPanelButton(
		int skillPanelFrame)
	{
		SkillPanelFrame = skillPanelFrame & SkillPanelFrameMask;
	}

	public ControlPanelButton(
		int skillPanelFrame,
		IButtonAction buttonAction,
		int iconX,
		int iconY)
	{
		ButtonAction = buttonAction;
		SkillPanelFrame = skillPanelFrame & SkillPanelFrameMask;

		IconX = iconX;
		IconY = iconY;
	}

	public virtual bool TryPress(int mouseX, int mouseY)
	{
		return MouseIsOverButton(mouseX, mouseY);
	}

	protected bool MouseIsOverButton(int mouseX, int mouseY)
	{
		return ShouldRender &&
			   mouseX >= ScreenX && mouseX <= ScreenX + ScreenWidth &&
			   mouseY >= ScreenY && mouseY <= ScreenY + ScreenHeight;
	}

	public virtual ReadOnlySpan<int> GetDigitsToRender() => ReadOnlySpan<int>.Empty;
	public virtual int GetNumberOfDigitsToRender() => 0;

	public virtual ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new ControlPanelButtonRenderer(spriteBank, this, IconX, IconY);
	}
}