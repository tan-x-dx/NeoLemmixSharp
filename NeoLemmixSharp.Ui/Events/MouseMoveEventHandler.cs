using NeoLemmixSharp.Common;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Ui.Events;

public sealed class MouseMoveEventHandler : IDisposable
{
    public delegate void ComponentMouseMoveAction(Component c, Point position);

    private readonly List<ComponentMouseMoveAction> _actions = [];

    public void RegisterMouseMoveEvent(ComponentMouseMoveAction action)
    {
        _actions.Add(action);
    }

    public void Invoke(Component c, Point position)
    {
        for (int i = 0; i < _actions.Count; i++)
        {
            var action = _actions[i];
            action(c, position);
        }
    }

    public void Clear()
    {
        _actions.Clear();
    }

    public void Dispose()
    {
        Clear();
    }
}
