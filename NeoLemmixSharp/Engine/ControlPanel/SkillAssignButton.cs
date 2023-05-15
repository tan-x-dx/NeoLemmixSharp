using NeoLemmixSharp.Engine.LemmingSkills;

namespace NeoLemmixSharp.Engine.ControlPanel;

public sealed class SkillAssignButton : ControlPanelButton
{
    public LemmingSkill LemmingSkill { get; }

    public bool IsSelected { get; set; }

    public SkillAssignButton(
        LemmingSkill lemmingSkill,
        int skillPanelFrame)
        : base(skillPanelFrame)
    {
        LemmingSkill = lemmingSkill;
    }
}