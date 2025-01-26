using NeoLemmixSharp.Engine.Level.FacingDirections;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class DirectionalSelectButtonAction : IButtonAction
{
    private readonly FacingDirection _facingDirection;

    public DirectionalSelectButtonAction(FacingDirection facingDirection)
    {
        _facingDirection = facingDirection;
    }

    public ButtonType ButtonType => _facingDirection == FacingDirection.Right
        ? ButtonType.DirectionalSelectRight
        : ButtonType.DirectionalSelectLeft;

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