using NeoLemmixSharp.Common;
using NeoLemmixSharp.Ui.Events;

namespace NeoLemmixSharp.Ui.Components.Buttons;

public sealed class ToggleButton : Component
{
    private string? _alternateLabel;
    private bool _isActive = false;

    public MouseEventHandler OnDeactivated { get; } = new();

    public override ComponentState State
    {
        get => base.State;
        set
        {
            if (IsActive)
            {
                base.State = value;
            }
        }
    }

    public ToggleButton(int x, int y, string? label)
        : base(x, y, label)
    {
        _alternateLabel = label;

        MousePressed.RegisterMouseEvent(OnMouseDown);
        MouseEnter.RegisterMouseEvent(SetMouseOver);
        MouseReleased.RegisterMouseEvent(SetMouseOver);
        MouseExit.RegisterMouseEvent(SetMouseNormal);
    }

    public ToggleButton(int x, int y, int width, int height, string? label)
        : base(x, y, width, height, label)
    {
        _alternateLabel = label;

        MousePressed.RegisterMouseEvent(OnMouseDown);
        MouseEnter.RegisterMouseEvent(SetMouseOver);
        MouseReleased.RegisterMouseEvent(SetMouseOver);
        MouseExit.RegisterMouseEvent(SetMouseNormal);
    }

    public void SetAlternateLabel(string message) => _alternateLabel = message;

    public override string? Label
    {
        get => IsActive ? _alternateLabel : base.Label;
        set => base.Label = value;
    }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            State = _isActive ? ComponentState.Active : ComponentState.Normal;
        }
    }

    private void OnMouseDown(Component c, Point mousePosition)
    {
        IsActive = !IsActive;
        SetMousePress(c, mousePosition);
    }
}
