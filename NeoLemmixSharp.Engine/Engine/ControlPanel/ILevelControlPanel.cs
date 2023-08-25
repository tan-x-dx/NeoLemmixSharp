namespace NeoLemmixSharp.Engine.Engine.ControlPanel;

public interface ILevelControlPanel
{
    SkillAssignButton? SelectedSkillAssignButton { get; }
    int SelectedSkillButtonId { get; }

    void HandleMouseInput();
    void SetWindowDimensions(int windowWidth, int windowHeight);
}