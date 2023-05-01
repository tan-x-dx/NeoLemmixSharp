using NeoLemmixSharp.Engine.LemmingSkills;

namespace NeoLemmixSharp.Engine.ControlPanel;

public sealed class SkillAssignButton : ControlPanelButton
{
    private readonly LemmingSkill _lemmingSkill;

    public int NumberOfSkillsAvailable { get; }
    public bool IsSelected { get; }

    public SkillAssignButton(
        LemmingSkill lemmingSkill,
        int numberOfSkillsAvailable)
    {
        _lemmingSkill = lemmingSkill;
        NumberOfSkillsAvailable = numberOfSkillsAvailable;
    }
}