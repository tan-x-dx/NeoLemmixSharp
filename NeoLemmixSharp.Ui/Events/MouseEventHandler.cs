using NeoLemmixSharp.Common;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Ui.Events;

public sealed class MouseEventHandler
{
    public delegate void ComponentMouseAction(Component c, Point position);

    private readonly List<ComponentMouseAction> _actions = new();

    public void RegisterMouseEvent(ComponentMouseAction action)
    {
        if (!_actions.Contains(action))
        {
            _actions.Add(action);
        }
    }

    public void Invoke(Component c, Point position)
    {
        foreach (ComponentMouseAction action in _actions)
        {
            action.Invoke(c, position);
        }
    }

    public void Clear() => _actions.Clear();
}
