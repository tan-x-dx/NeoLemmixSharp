namespace NeoLemmixSharp.Engine.Engine.ControlPanel;

public interface ILevelControlPanel
{
    int SelectedSkillButtonId { get; }

    void HandleMouseInput();
    void SetWindowDimensions(int windowWidth, int windowHeight);
}