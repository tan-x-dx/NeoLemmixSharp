namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public interface IButtonAction
{
	void OnMouseDown();
	void OnPress();
	void OnDoubleTap();

	void OnRightClick();
}

public sealed class EmptyButtonAction : IButtonAction
{
	public static readonly EmptyButtonAction Instance = new();

	private EmptyButtonAction()
	{
	}

	public void OnMouseDown()
	{
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