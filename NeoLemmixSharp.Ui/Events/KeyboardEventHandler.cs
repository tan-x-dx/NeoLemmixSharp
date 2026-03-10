using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Ui.Events;

public sealed class KeyboardEventHandler : IDisposable
{
    public delegate void ComponentKeyboardAction(Component c, in KeysEnumerable keys);

    private readonly List<ComponentKeyboardAction> _actions = [];

    public void RegisterKeyEvent(ComponentKeyboardAction action)
    {
        _actions.Add(action);
    }

    public void Invoke(Component c, in KeysEnumerable keys)
    {
        foreach (ComponentKeyboardAction action in _actions)
        {
            action(c, in keys);
        }
    }

    public void Clear() => _actions.Clear();

    public void Dispose()
    {
        Clear();
    }
}
