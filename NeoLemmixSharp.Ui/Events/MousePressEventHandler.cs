using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components;
using System.Runtime.CompilerServices;
using ButtonActionList = System.Collections.Generic.List<NeoLemmixSharp.Ui.Events.MousePressEventHandler.ComponentMousePressAction>;

namespace NeoLemmixSharp.Ui.Events;

public sealed class MousePressEventHandler : IDisposable
{
    public delegate void ComponentMousePressAction(Component c, Point position);

    private ButtonActionList? _leftButtonActions;
    private ButtonActionList? _middleButtonActions;
    private ButtonActionList? _rightButtonActions;
    private ButtonActionList? _mouse4ButtonActions;
    private ButtonActionList? _mouse5ButtonActions;

    public void RegisterMousePressEvent(ComponentMousePressAction action, MouseButtonType mouseButtonType)
    {
        ref var buttonActions = ref GetButtonActionListRef(mouseButtonType);
        buttonActions ??= [];
        buttonActions.Add(action);
    }

    public void Invoke(Component c, Point position, MouseButtonType mouseButtonType)
    {
        var buttonActions = GetButtonActionListRef(mouseButtonType);
        if (buttonActions == null)
            return;

        // Do not convert to foreach loop!
        for (int i = 0; i < buttonActions.Count; i++)
        {
            var action = buttonActions[i];
            action(c, position);
        }
    }

    private ref ButtonActionList? GetButtonActionListRef(MouseButtonType mouseButtonType)
    {
        switch (mouseButtonType)
        {
            case MouseButtonType.Left:
                return ref _leftButtonActions;

            case MouseButtonType.Middle:
                return ref _middleButtonActions;

            case MouseButtonType.Right:
                return ref _rightButtonActions;

            case MouseButtonType.Mouse4:
                return ref _mouse4ButtonActions;

            case MouseButtonType.Mouse5:
                return ref _mouse5ButtonActions;

            default:
                Helpers.ThrowUnknownEnumValueException<MouseButtonType, ButtonActionList>(mouseButtonType);
                break;
        }

        return ref Unsafe.NullRef<ButtonActionList?>();
    }

    public void Clear()
    {
        _leftButtonActions?.Clear();
        _middleButtonActions?.Clear();
        _rightButtonActions?.Clear();
        _mouse4ButtonActions?.Clear();
        _mouse5ButtonActions?.Clear();
    }

    public void Dispose()
    {
        Clear();
    }
}
