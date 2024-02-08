using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class FastForwardButtonAction : IButtonAction
{
    private readonly InputAction _fastForwardAction;

    public ButtonType ButtonType => ButtonType.FastForward;

    public FastForwardButtonAction(InputAction fastForwardAction)
    {
        _fastForwardAction = fastForwardAction;
    }

    public void OnMouseDown()
    {
    }

    public void OnPress(bool isDoubleTap)
    {
        _fastForwardAction.DoPress();
    }

    public void OnRightClick()
    {
    }
}