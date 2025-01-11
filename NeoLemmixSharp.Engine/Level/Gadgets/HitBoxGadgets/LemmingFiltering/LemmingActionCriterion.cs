using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingActionCriterion : ILemmingCriterion
{
    private readonly LemmingActionSet _allowedActions = LemmingAction.CreateEmptySimpleSet();

    public void RegisterActions(LemmingActionSet actions)
    {
        _allowedActions.UnionWith(actions);
    }

    public bool LemmingMatchesCriteria(Lemming lemming) => _allowedActions.Contains(lemming.CurrentAction);
}