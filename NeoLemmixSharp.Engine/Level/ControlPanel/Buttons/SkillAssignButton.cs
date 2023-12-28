using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class SkillAssignButton : ControlPanelButton
{
	private const int NumberOfChars = 2;

	private readonly int[] _skillCountChars;

	public int SkillId { get; }
	public int SkillTrackingDataId { get; }
	public int SkillAssignButtonId { get; }

	public SkillAssignButton(
		int skillAssignButtonId,
		int skillPanelFrame,
		int skillId,
		int skillTrackingDataId)
		: base(skillPanelFrame)
	{
		_skillCountChars = new int[NumberOfChars];

		SkillId = skillId;
		SkillTrackingDataId = skillTrackingDataId;
		SkillAssignButtonId = skillAssignButtonId;

		ButtonAction = new SkillAssignButtonAction(this);
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

	public override ReadOnlySpan<int> GetDigitsToRender() => new(_skillCountChars);
	public override int GetNumberOfDigitsToRender() => NumberOfChars;

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new SkillAssignButtonRenderer(spriteBank, this);
	}
}

public sealed class SkillAssignButtonAction : IButtonAction
{
	private readonly SkillAssignButton _skillAssignButton;

	public SkillAssignButtonAction(SkillAssignButton skillAssignButton)
	{
		_skillAssignButton = skillAssignButton;
	}

	public void OnMouseDown()
	{
		LevelScreen.LevelControlPanel.SetSelectedSkillAssignmentButton(_skillAssignButton);
	}

	public void OnPress()
	{
	}

	public void OnDoubleTap()
	{
	}

	public void OnRightClick()
	{
	}
}