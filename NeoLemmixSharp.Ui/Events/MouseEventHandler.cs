using NeoLemmixSharp.Common;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Ui.Events;

public sealed class MouseEventHandler : IDisposable
{
    public delegate void ComponentMouseAction(Component c, Point position);

    private readonly List<ComponentMouseAction> _actions = [];

    public void RegisterMouseEvent(ComponentMouseAction action)
    {
        _actions.Add(action);
    }

    public void Invoke(Component c, Point position)
    {
        foreach (ComponentMouseAction action in _actions)
        {
            action(c, position);
        }
    }

    public void Clear() => _actions.Clear();

    public void Dispose()
    {
        Clear();
    }
}
