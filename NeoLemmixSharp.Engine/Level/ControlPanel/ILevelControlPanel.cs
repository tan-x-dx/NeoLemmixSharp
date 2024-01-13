using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Timer;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public interface ILevelControlPanel
{
	int Width { get; }
	int Height { get; }

	LevelTimer LevelTimer { get; }
	SkillAssignButton? SelectedSkillAssignButton { get; }
	int SelectedSkillButtonId { get; }
	int ControlPanelScale { get; }

	void HandleMouseInput();
	void SetPanelScale(int scale);
	void SetWindowDimensions(int windowWidth, int windowHeight);
	void ChangeSkillAssignButtonScroll(int delta);
	void SetSelectedSkillAssignmentButton(SkillAssignButton? skillAssignButton);
	void UpdateSkillCount(SkillAssignButton? selectedSkillAssignButton, int skillCount);
	void OnSpawnIntervalChanged();
}