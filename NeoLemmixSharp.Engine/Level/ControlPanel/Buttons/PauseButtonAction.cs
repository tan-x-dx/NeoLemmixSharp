namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class PauseButtonAction : IButtonAction
{
	public ButtonType ButtonType => ButtonType.Pause;

	public void OnMouseDown()
	{
	}

	public void OnPress(bool isDoubleTap)
	{
		LevelScreen.UpdateScheduler.PausePress();
	}

	public void OnRightClick()
	{
	}
}