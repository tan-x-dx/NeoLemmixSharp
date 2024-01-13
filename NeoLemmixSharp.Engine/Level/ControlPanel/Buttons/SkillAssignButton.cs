using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class SkillAssignButton : ControlPanelButton, IButtonAction
{
    private const int NumberOfChars = 2;

    private readonly int[] _skillCountChars;

    public int SkillId { get; }
    public int SkillTrackingDataId { get; }
    public int SkillAssignButtonId { get; }

    public SkillAssignButton(
        int buttonId,
        int skillAssignButtonId,
        int skillPanelFrame,
        int skillId,
        int skillTrackingDataId)
        : base(buttonId, skillPanelFrame)
    {
        _skillCountChars = new int[NumberOfChars];

        SkillId = skillId;
        SkillTrackingDataId = skillTrackingDataId;
        SkillAssignButtonId = skillAssignButtonId;

        ButtonAction = this;
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

    public ButtonType ButtonType => ButtonType.SkillAssign;

    public void OnMouseDown()
    {
        LevelScreen.LevelControlPanel.SetSelectedSkillAssignmentButton(this);
    }

    public void OnPress(bool isDoubleTap)
    {
    }

    public void OnRightClick()
    {
    }
}