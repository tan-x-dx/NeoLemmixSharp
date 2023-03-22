using NeoLemmixSharp.Rendering;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeoLemmixSharp.Engine.LemmingActions;

public interface ILemmingAction
{
    public static ReadOnlyDictionary<string, ILemmingAction> LemmingActions { get; } = RegisterAllLemmingActions();

    private static ReadOnlyDictionary<string, ILemmingAction> RegisterAllLemmingActions()
    {
        var result = new Dictionary<string, ILemmingAction>();

        RegisterLemmingAction(result, WalkerAction.Instance);
        RegisterLemmingAction(result, FallerAction.Instance);
        RegisterLemmingAction(result, AscenderAction.Instance);

        return new ReadOnlyDictionary<string, ILemmingAction>(result);
    }

    private static void RegisterLemmingAction(IDictionary<string, ILemmingAction> dict, ILemmingAction lemmingAction)
    {
        dict.Add(lemmingAction.LemmingActionName, lemmingAction);
    }

    public static ICollection<ILemmingAction> AllLemmingActions => LemmingActions.Values;

    LemmingActionSpriteBundle ActionSpriteBundle { get; set; }

    string LemmingActionName { get; }
    int LemmingActionId { get; }
    int NumberOfAnimationFrames { get; set; }

    void UpdateLemming(Lemming lemming);
}