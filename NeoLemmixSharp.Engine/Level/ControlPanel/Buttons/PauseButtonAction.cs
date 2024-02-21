using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class PauseButtonAction : IButtonAction
{
    private readonly InputAction _pauseAction;

    public ButtonType ButtonType => ButtonType.Pause;

    public PauseButtonAction(InputAction pauseAction)
    {
        _pauseAction = pauseAction;
    }

    public void OnMouseDown()
    {
    }

    public void OnPress(bool isDoubleTap)
    {
        _pauseAction.DoPress();
    }

    public void OnRightClick()
    {
    }
}