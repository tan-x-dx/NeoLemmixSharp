using NeoLemmixSharp.Engine.LemmingSkills;

namespace NeoLemmixSharp.Engine.ControlPanel;

public sealed class SkillAssignButton : ControlPanelButton
{
    private readonly LemmingSkill _lemmingSkill;

    private int _numberOfSkillsAvailable;

    public SkillAssignButton(
        LemmingSkill lemmingSkill,
        int numberOfSkillsAvailable)
    {
        _lemmingSkill = lemmingSkill;
        _numberOfSkillsAvailable = numberOfSkillsAvailable;
    }
}