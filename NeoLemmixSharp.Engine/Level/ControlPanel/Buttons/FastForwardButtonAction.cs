namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class FastForwardButtonAction : IButtonAction
{
    public ButtonType ButtonType => ButtonType.FastForward;

    public void OnMouseDown()
    {
    }

    public void OnPress(bool isDoubleTap)
    {
        LevelScreen.UpdateScheduler.FastForwardButtonPress();
    }

    public void OnRightClick()
    {
    }
}