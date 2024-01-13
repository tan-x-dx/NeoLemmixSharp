namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class FrameNudgeButtonAction : IButtonAction
{
    private readonly int _delta;

    public FrameNudgeButtonAction(int delta)
    {
        _delta = delta;
    }

    public ButtonType ButtonType => _delta < 0
        ? ButtonType.NudgeFrameBack
        : ButtonType.NudgeFrameForward;

    public void OnMouseDown()
    {
    }

    public void OnPress(bool isDoubleTap)
    {
    }

    public void OnRightClick()
    {
    }
}