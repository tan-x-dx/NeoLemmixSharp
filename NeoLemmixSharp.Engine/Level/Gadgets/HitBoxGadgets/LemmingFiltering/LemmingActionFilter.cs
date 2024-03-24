using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingActionFilter : ILemmingFilter
{
    private readonly SimpleSet<LemmingAction> _allowedActions = ExtendedEnumTypeComparer<LemmingAction>.CreateSimpleSet();

    public void RegisterActions(SimpleSet<LemmingAction> actions)
    {
        _allowedActions.UnionWith(actions);
    }

    public bool MatchesLemming(Lemming lemming) => _allowedActions.Contains(lemming.CurrentAction);
}