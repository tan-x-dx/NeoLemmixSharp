using NeoLemmixSharp.Engine.Engine.Skills;

namespace NeoLemmixSharp.Engine.Engine.ControlPanel;

public interface ILevelControlPanel
{
    SkillAssignButton? SelectedSkillAssignButton { get; }
    LemmingSkill SelectedSkill { get; }

    void HandleMouseInput();
    void SetWindowDimensions(int windowWidth, int windowHeight);
}