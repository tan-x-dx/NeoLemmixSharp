using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Events;

namespace NeoLemmixSharp.Ui.Components.Buttons;

public sealed class ToggleButton : Button
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

    public ToggleButton(int x, int y, string? label) : base(x, y, label)
    {
        _alternateLabel = label;

        MouseDown.RegisterMouseEvent(OnMouseDown);
    }

    public ToggleButton(int x, int y, int width, int height, string? label) : base(x, y, width, height, label)
    {
        _alternateLabel = label;

        MouseDown.RegisterMouseEvent(OnMouseDown);
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

    private void OnMouseDown(Component _, LevelPosition mousePosition)
    {
        IsActive = !IsActive;
    }
}
