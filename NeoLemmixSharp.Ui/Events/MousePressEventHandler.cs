using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Ui.Events;

public sealed class MousePressEventHandler : IDisposable
{
    public delegate void ComponentMousePressAction(Component c, Point position);

    private List<ComponentMousePressAction>? _leftButtonActions;
    private List<ComponentMousePressAction>? _middleButtonActions;
    private List<ComponentMousePressAction>? _rightButtonActions;
    private List<ComponentMousePressAction>? _mouse4ButtonActions;
    private List<ComponentMousePressAction>? _mouse5ButtonActions;

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

        for (int i = 0; i < buttonActions.Count; i++)
        {
            var action = buttonActions[i];
            action(c, position);
        }
    }

    private ref List<ComponentMousePressAction>? GetButtonActionListRef(MouseButtonType mouseButtonType)
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
                Helpers.ThrowUnknownEnumValueException<MouseButtonType, List<ComponentMousePressAction>>(mouseButtonType);
                break;
        }

        return ref Unsafe.NullRef<List<ComponentMousePressAction>?>();
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
