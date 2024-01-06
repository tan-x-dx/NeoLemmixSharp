using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public class ControlPanelButton
{
	private const int NumberOfSkillPanels = 8;
	public const int SkillPanelFrameMask = NumberOfSkillPanels - 1;

	private readonly int _iconX;
	private readonly int _iconY;

	public int SkillPanelFrame { get; }
	public int ScreenX { get; set; }
	public int ScreenY { get; set; }
	public int ScreenWidth { get; set; }
	public int ScreenHeight { get; set; }

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
		_iconX = iconX;
		_iconY = iconY;

		ButtonAction = buttonAction;
		SkillPanelFrame = skillPanelFrame & SkillPanelFrameMask;
	}

	public bool MouseIsOverButton(int mouseX, int mouseY)
	{
		return ShouldRender &&
			   mouseX >= ScreenX && mouseX < ScreenX + ScreenWidth &&
			   mouseY >= ScreenY && mouseY < ScreenY + ScreenHeight;
	}

	public virtual ReadOnlySpan<int> GetDigitsToRender() => ReadOnlySpan<int>.Empty;
	public virtual int GetNumberOfDigitsToRender() => 0;

	public virtual ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new ControlPanelButtonRenderer(spriteBank, this, _iconX, _iconY);
	}
}