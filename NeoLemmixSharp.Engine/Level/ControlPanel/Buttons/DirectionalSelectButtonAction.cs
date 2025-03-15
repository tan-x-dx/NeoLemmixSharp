using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class DirectionalSelectButtonAction : IButtonAction
{
    private readonly FacingDirection _facingDirection;

    public DirectionalSelectButtonAction(FacingDirection facingDirection)
    {
        _facingDirection = new FacingDirection(facingDirection.Id);
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