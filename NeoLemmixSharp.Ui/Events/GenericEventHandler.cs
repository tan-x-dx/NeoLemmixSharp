using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Ui.Events;

public sealed class GenericEventHandler : IDisposable
{
    public delegate void ComponentAction(Component c);

    private readonly List<ComponentAction> _actions = [];

    public void RegisterEvent(ComponentAction action)
    {
        _actions.Add(action);
    }

    public void Invoke(Component c)
    {
        for (int i = 0; i < _actions.Count; i++)
        {
            var action = _actions[i];
            action(c);
        }
    }

    public void Clear() => _actions.Clear();

    public void Dispose()
    {
        Clear();
    }
}
