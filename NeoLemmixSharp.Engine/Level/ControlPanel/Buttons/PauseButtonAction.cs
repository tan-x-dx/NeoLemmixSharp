namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class PauseButtonAction : IButtonAction
{
	public ButtonType ButtonType => ButtonType.Pause;

	public void OnMouseDown()
	{
	}

	public void OnPress()
	{
		LevelScreen.UpdateScheduler.PausePress();
	}

	public void OnDoubleTap()
	{
	}

	public void OnRightClick()
	{
	}
}