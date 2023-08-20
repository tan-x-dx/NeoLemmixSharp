using NeoLemmixSharp.Engine.Engine.Skills;

namespace NeoLemmixSharp.Engine.Engine.ControlPanel;

public sealed class SkillAssignButton : ControlPanelButton
{
    public int SkillAssignButtonId { get; }
    public bool IsSelected { get; set; }

    public SkillAssignButton(
        int skillAssignButtonId,
        int skillPanelFrame)
        : base(skillPanelFrame)
    {
        SkillAssignButtonId = skillAssignButtonId;
    }
}