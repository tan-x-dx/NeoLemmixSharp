namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class FastForwardButtonAction : IButtonAction
{
	public ButtonType ButtonType => ButtonType.FastForward;

	public void OnMouseDown()
	{
	}

	public void OnPress()
	{
		LevelScreen.UpdateScheduler.FastForwardButtonPress();
	}

	public void OnDoubleTap()
	{
	}

	public void OnRightClick()
	{
	}
}