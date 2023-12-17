using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Timer;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public interface ILevelControlPanel
{
    LevelTimer LevelTimer { get; }
    SkillAssignButton? SelectedSkillAssignButton { get; }
    int SelectedSkillButtonId { get; }

    void HandleMouseInput();
    void SetWindowDimensions(int windowWidth, int windowHeight);
}