namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class SkillAssignScrollButtonAction : IButtonAction
{
	private readonly int _delta;

	public SkillAssignScrollButtonAction(int delta)
	{
		_delta = delta;
	}

	public void OnMouseDown()
	{
		LevelScreen.LevelControlPanel.ChangeSkillAssignButtonScroll(_delta);
	}

	public void OnPress()
	{
	}

	public void OnDoubleTap()
	{
	}

	public void OnRightClick()
	{
	}
}