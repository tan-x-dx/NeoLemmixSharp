using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class SkillAssignButton : ControlPanelButton
{
    private readonly int[] _skillCountChars;

    public SkillTrackingData SkillTrackingData { get; }

    public int SkillAssignButtonId { get; }
    public bool IsSelected { get; set; }

    public ReadOnlySpan<int> SkillCountChars => new(_skillCountChars);

    public SkillAssignButton(
        int skillAssignButtonId,
        int skillPanelFrame,
        SkillTrackingData skillTrackingData)
        : base(skillPanelFrame)
    {
        _skillCountChars = new int[2];

        SkillAssignButtonId = skillAssignButtonId;
        SkillTrackingData = skillTrackingData;
    }

    public void UpdateSkillCount(int numberOfSkillsAvailable)
    {
        if (numberOfSkillsAvailable >= LevelConstants.InfiniteSkillCount)
        {
            _skillCountChars[0] = SkillCountDigitFont.InfinityGlyph;
            _skillCountChars[1] = ' ';
        }
        else
        {
            var span = new Span<int>(_skillCountChars);
            TextRenderingHelpers.WriteDigits(span, numberOfSkillsAvailable);
        }
    }

    public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
    {
        return new SkillAssignButtonRenderer(spriteBank, this);
    }
}