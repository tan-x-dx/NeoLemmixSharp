using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Ui.Components.Buttons;

public sealed class GroupedButton : Button
{
    public static ButtonGroup GroupButtons(params GroupedButton[] buttons) => new ButtonGroup(buttons);

    private ButtonGroup? _group = null;
    private int _index = -1;
    private bool _isActive = false;

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

    public GroupedButton(int x, int y, string label) : base(x, y, label)
    {
    }

    public GroupedButton(int x, int y, int width, int height, string label)
        : base(x, y, width, height, label)
    {
    }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;

            if (value && _group != null)
            {
                _group.SetActive(this, false);
            }
        }
    }

    public sealed class ButtonGroup
    {
        private readonly GroupedButton[] _buttons;

        private int _activeIndex = -1;

        public ButtonGroup(params GroupedButton[] buttons)
        {
            if (buttons.Length == 0)
                throw new InvalidOperationException();

            _buttons = buttons;

            for (int i = _buttons.Length - 1; i >= 0; i--)
            {
                _buttons[i]._group = this;
                _buttons[i]._index = i;
            }
        }

        public void SetActive(GroupedButton? button, bool performClick)
        {
            if (button == null)
            {
                if (_activeIndex != -1)
                {
                    _buttons[_activeIndex].IsActive = false;
                    _buttons[_activeIndex].State = ComponentState.Normal;
                }

                _activeIndex = -1;

                return;
            }

            if (this == button._group)
            {
                if (_activeIndex != -1)
                {
                    _buttons[_activeIndex].IsActive = false;
                    _buttons[_activeIndex].State = ComponentState.Normal;
                }

                button.IsActive = true;
                _activeIndex = button._index;
                if (performClick)
                {
                    button.MouseDown.Invoke(button, new LevelPosition());
                }
            }
        }

        public void SetActive(int buttonIndex, bool performClick)
        {
            if (buttonIndex < 0 || buttonIndex >= _buttons.Length)
            {
                SetActive(null, false);
            }
            else
            {
                SetActive(_buttons[buttonIndex], performClick);
            }
        }
    }
}
