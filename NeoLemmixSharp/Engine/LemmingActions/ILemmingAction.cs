using System.Collections.Generic;

namespace NeoLemmixSharp.Engine.LemmingActions;

public interface ILemmingAction
{
    private static Dictionary<int, ILemmingAction> LemmingActions { get; } = RegisterAllLemmingActions();

    private static Dictionary<int, ILemmingAction> RegisterAllLemmingActions()
    {
        var result = new Dictionary<int, ILemmingAction>();

        RegisterLemmingAction(result, WalkerAction.Instance);
        RegisterLemmingAction(result, FallerAction.Instance);
        RegisterLemmingAction(result, AscenderAction.Instance);

        return result;
    }

    private static void RegisterLemmingAction(IDictionary<int, ILemmingAction> dict, ILemmingAction lemmingAction)
    {
        dict.Add(lemmingAction.LemmingActionId, lemmingAction);
    }

    public static ICollection<ILemmingAction> AllLemmingActions => LemmingActions.Values;

    string LemmingActionName { get; }
    int LemmingActionId { get; }

    void UpdateLemming(Lemming lemming);
}