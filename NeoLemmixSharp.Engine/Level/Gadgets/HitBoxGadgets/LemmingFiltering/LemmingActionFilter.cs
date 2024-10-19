using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingActionFilter : ILemmingFilter
{
    private readonly LemmingActionSet _allowedActions = ExtendedEnumTypeComparer<LemmingAction>.CreateSimpleSet();

    public void RegisterActions(LemmingActionSet actions)
    {
        _allowedActions.UnionWith(actions);
    }

    public bool MatchesLemming(Lemming lemming) => _allowedActions.Contains(lemming.CurrentAction);
}