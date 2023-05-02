using NeoLemmixSharp.Engine.LemmingSkills;

namespace NeoLemmixSharp.Engine.ControlPanel;

public sealed class SkillAssignButton : ControlPanelButton
{
    public LemmingSkill LemmingSkill { get; }

    public int NumberOfSkillsAvailable { get; }
    public bool IsSelected { get; set; }

    public SkillAssignButton(
        LemmingSkill lemmingSkill,
        int numberOfSkillsAvailable,
        int skillPanelFrame)
        : base(skillPanelFrame)
    {
        LemmingSkill = lemmingSkill;
        NumberOfSkillsAvailable = numberOfSkillsAvailable;
    }
}