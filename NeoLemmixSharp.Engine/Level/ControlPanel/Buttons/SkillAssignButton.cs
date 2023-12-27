using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class SkillAssignButton : ControlPanelButton
{
	private readonly int[] _skillCountChars;

	public int SkillId { get; }
	public int SkillTrackingDataId { get; }
	public int SkillAssignButtonId { get; }

	public bool IsSelected { get; set; }

	public ReadOnlySpan<int> SkillCountChars => new(_skillCountChars);

	public SkillAssignButton(
		int skillAssignButtonId,
		int skillPanelFrame,
		int skillId,
		int skillTrackingDataId)
		: base(skillPanelFrame)
	{
		_skillCountChars = new int[2];

		SkillId = skillId;
		SkillTrackingDataId = skillTrackingDataId;
		SkillAssignButtonId = skillAssignButtonId;
	}

	public void UpdateSkillCount(int numberOfSkillsAvailable)
	{
		if (numberOfSkillsAvailable >= LevelConstants.InfiniteSkillCount)
		{
			_skillCountChars[0] = SkillCountDigitFont.InfinityGlyph;
			_skillCountChars[1] = ' ';
			return;
		}

		var span = new Span<int>(_skillCountChars);
		TextRenderingHelpers.WriteDigits(span, numberOfSkillsAvailable);
	}

	public override void OnPress()
	{
		LevelScreen.LevelControlPanel.SetSelectedSkillAssignmentButton(this);
	}

	public override void OnDoubleTap()
	{
		LevelScreen.LevelControlPanel.SetSelectedSkillAssignmentButton(this);
	}

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new SkillAssignButtonRenderer(spriteBank, this);
	}
}