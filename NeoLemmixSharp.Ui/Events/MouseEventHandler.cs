using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Ui.Events;

public sealed class MouseEventHandler
{
    public delegate void ComponentMouseAction(Component c, LevelPosition position);

    private readonly List<ComponentMouseAction> _actions = new();

    public void RegisterMouseEvent(ComponentMouseAction action)
    {
        if (!_actions.Contains(action))
        {
            _actions.Add(action);
        }
    }

    public void Invoke(Component c, LevelPosition position)
    {
        foreach (ComponentMouseAction action in _actions)
        {
            action.Invoke(c, position);
        }
    }

    public void Clear() => _actions.Clear();
}
