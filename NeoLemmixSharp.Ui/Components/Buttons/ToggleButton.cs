using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Ui.Components.Buttons;

public sealed class ToggleButton : Button
{
    private Action? _deactivateClick = null;
    private string _alternateLabel;

    public ToggleButton(int x, int y, string label) : base(x, y, label) { _alternateLabel = label; }

    public ToggleButton(int x, int y, int width, int height, string label) : base(x, y, width, height, label) { _alternateLabel = label; }

    public void SetDeactivationAction(Action action) => _deactivateClick = action;

    public void SetAlternateLabel(string message) => _alternateLabel = message;

    public override string? Label
    {
        get => Active ? _alternateLabel : base.Label;
        set => base.Label = value;
    }

    public override void InvokeMouseEnter(LevelPosition mousePosition)
    {
        if (!Active)
        {
            base.InvokeMouseEnter(mousePosition);
        }
    }

    public override void InvokeMouseDown(LevelPosition mousePosition)
    {
        if (Active)
        {
            _deactivateClick?.Invoke();
        }
        else
        {
            State = ComponentState.Active;
            Click();
        }

        Active = !Active;
    }

    public override void InvokeMouseUp(LevelPosition mousePosition)
    {
        if (!Active)
        {
            base.InvokeMouseUp(mousePosition);
        }
    }

    public override void InvokeMouseExit(LevelPosition mousePosition)
    {
        if (!Active)
        {
            base.InvokeMouseExit(mousePosition);
        }
    }
}
