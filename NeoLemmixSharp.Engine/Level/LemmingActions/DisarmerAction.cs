using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DisarmerAction : LemmingAction
{
    public static readonly DisarmerAction Instance = new();

    private DisarmerAction()
        : base(
            LemmingActionType.DisarmerAction,
            LemmingActionConstants.DisarmerActionName,
            LemmingActionConstants.DisarmerActionSpriteFileName,
            LemmingActionConstants.DisarmerAnimationFrames,
            LemmingActionConstants.MaxDisarmerPhysicsFrames,
            CursorSelectionPriority.PermanentSkillPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        lemming.DisarmingFrames--;
        if (lemming.DisarmingFrames <= 0)
        {
            if (lemming.NextActionType == LemmingActionType.NoneAction)
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            }
            else
            {
                lemming.NextAction.TransitionLemmingToAction(lemming, false);
                lemming.NextAction = NoneAction.Instance;
            }
        }
        else if ((lemming.PhysicsFrame & 7) == 0)
        {
            // ?? CueSoundEffect(SFX_FIXING, L.Position); ??
        }

        return false;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround) => DoMainTransitionActions(lemming, turnAround);
}
