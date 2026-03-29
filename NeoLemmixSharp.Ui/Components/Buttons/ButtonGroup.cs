using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Ui.Components.Buttons;

public sealed class ButtonGroup : IDisposable
{
    private readonly Button[] _buttons;
    private Button? _currentButton = null;

    public ReadOnlySpan<Button> Buttons => new(_buttons);

    public ButtonGroup(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

        _buttons = new Button[capacity];

        for (int i = 0; i < _buttons.Length; i++)
        {
            var newButton = new Button(10 * i, 0, 10, 10);
            newButton.MouseEnter.Clear();
            newButton.MousePressed.Clear();
            newButton.MouseReleased.Clear();
            newButton.MouseExit.Clear();
            newButton.MousePressed.RegisterMousePressEvent(SetActive, MouseButtonType.Left);

            _buttons[i] = newButton;
        }

        SetActive(_buttons[0]);
    }

    private void SetActive(Component c, Point position)
    {
        SetActive(c as Button);
    }

    private void SetActive(Button? button)
    {
        _currentButton?.State = ComponentState.Normal;
        _currentButton = button;
        button?.State = ComponentState.Active;
    }

    public void SetActive(int buttonIndex)
    {
        Button? chosenButton = null;

        if ((uint)buttonIndex < (uint)_buttons.Length)
        {
            chosenButton = _buttons.At(buttonIndex);
        }

        SetActive(chosenButton);
    }

    public void Dispose()
    {
        new Span<Button>(_buttons).Clear();
    }
}
