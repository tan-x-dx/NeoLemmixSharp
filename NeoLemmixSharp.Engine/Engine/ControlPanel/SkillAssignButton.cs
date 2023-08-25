using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Engine.ControlPanel;

public sealed class SkillAssignButton : ControlPanelButton
{
    private readonly int[] _skillCountChars;

    public int SkillAssignButtonId { get; }
    public bool IsSelected { get; set; }

    public ReadOnlySpan<int> SkillCountChars => new(_skillCountChars);

    public SkillAssignButton(
        int skillAssignButtonId,
        int skillPanelFrame)
        : base(skillPanelFrame)
    {
        _skillCountChars = new int[2];

        SkillAssignButtonId = skillAssignButtonId;
    }

    public void UpdateSkillCount(int numberOfSkillsAvailable)
    {
        if (numberOfSkillsAvailable >= GameConstants.InfiniteSkillCount)
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
}