using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components;

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
        var buttonActions = GetButtonActionList(mouseButtonType);
        buttonActions.Add(action);
    }

    public void Invoke(Component c, Point position, MouseButtonType mouseButtonType)
    {
        var buttonActions = GetButtonActionList(mouseButtonType);
        foreach (ComponentMousePressAction action in buttonActions)
        {
            action(c, position);
        }
    }

    private List<ComponentMousePressAction> GetButtonActionList(MouseButtonType mouseButtonType)
    {
        switch (mouseButtonType)
        {
            case MouseButtonType.Left:
                _leftButtonActions ??= [];
                return _leftButtonActions;

            case MouseButtonType.Middle:
                _middleButtonActions ??= [];
                return _middleButtonActions;

            case MouseButtonType.Right:
                _rightButtonActions ??= [];
                return _rightButtonActions;

            case MouseButtonType.Mouse4:
                _mouse4ButtonActions ??= [];
                return _mouse4ButtonActions;

            case MouseButtonType.Mouse5:
                _mouse5ButtonActions ??= [];
                return _mouse5ButtonActions;

            default:
                Helpers.ThrowUnknownEnumValueException<MouseButtonType, List<ComponentMousePressAction>>(mouseButtonType);
                break;
        }

        return [];
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
