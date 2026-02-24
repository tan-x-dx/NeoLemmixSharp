using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Ui.Events;

public sealed class GenericEventHandler : IDisposable
{
    public delegate void ComponentAction(Component c);

    private readonly List<ComponentAction> _actions = [];

    public void RegisterEvent(ComponentAction action)
    {
        if (!_actions.Contains(action))
        {
            _actions.Add(action);
        }
    }

    public void Invoke(Component c)
    {
        foreach (ComponentAction action in _actions)
        {
            action(c);
        }
    }

    public void Clear() => _actions.Clear();

    public void Dispose()
    {
        Clear();
    }
}
