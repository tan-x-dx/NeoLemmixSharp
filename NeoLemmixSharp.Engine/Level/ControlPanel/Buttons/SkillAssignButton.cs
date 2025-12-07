using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class SkillAssignButton : ControlPanelButton, IButtonAction
{
    private const int NumberOfSkillChars = 2;

    private SkillCharBuffer _skillCountChars;

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
        SkillId = skillId;
        SkillTrackingDataId = skillTrackingDataId;
        SkillAssignButtonId = skillAssignButtonId;

        ButtonAction = this;
    }

    public void UpdateSkillCount()
    {
        var skillTrackingData = LevelScreen.SkillSetManager.GetSkillTrackingData(SkillTrackingDataId);
        if (skillTrackingData is null)
        {
            ClearSkillCount();
            return;
        }

        if (skillTrackingData.IsInfinite)
        {
            WriteInfiniteSkillCount();
            return;
        }

        var numberOfSkillsAvailable = skillTrackingData.EffectiveQuantity;

        if (numberOfSkillsAvailable == 0)
        {
            ClearSkillCount();
            return;
        }

        Debug.Assert(numberOfSkillsAvailable > 0);
        WriteSkillCount((uint)numberOfSkillsAvailable);
    }

    private void ClearSkillCount()
    {
        _skillCountChars[0] = ' ';
        _skillCountChars[1] = ' ';
    }

    private void WriteInfiniteSkillCount()
    {
        _skillCountChars[0] = SkillCountDigitFont.InfinityGlyph;
        _skillCountChars[1] = ' ';
    }

    private void WriteSkillCount(uint skillCount)
    {
        NumberFormattingHelpers.WriteDigits(_skillCountChars, skillCount);
    }

    public override ReadOnlySpan<char> GetDigitsToRender() => _skillCountChars;

    public override int GetNumberOfDigitsToRender() => NumberOfSkillChars;

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

    [InlineArray(NumberOfSkillChars)]
    private struct SkillCharBuffer
    {
        private char _0;
    }
}
