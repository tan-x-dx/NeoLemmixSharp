using NeoLemmixSharp.Engine.LemmingSkills;

namespace NeoLemmixSharp.Engine.ControlPanel;

public sealed class SkillAssignButton : ControlPanelButton
{
    private readonly LemmingSkill _lemmingSkill;

    public int NumberOfSkillsAvailable { get; }
    public bool IsSelected { get; set; }
    public bool ShouldRender { get; set; }

    public SkillAssignButton(
        LemmingSkill lemmingSkill,
        int numberOfSkillsAvailable,
        int skillPanelFrame)
        : base(skillPanelFrame)
    {
        _lemmingSkill = lemmingSkill;
        NumberOfSkillsAvailable = numberOfSkillsAvailable;
    }
}