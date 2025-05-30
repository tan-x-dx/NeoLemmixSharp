using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DisarmerAction : LemmingAction
{
    public static readonly DisarmerAction Instance = new();

    private DisarmerAction()
        : base(
            LemmingActionConstants.DisarmerActionId,
            LemmingActionConstants.DisarmerActionName,
            LemmingActionConstants.DisarmerActionSpriteFileName,
            LemmingActionConstants.DisarmerAnimationFrames,
            LemmingActionConstants.MaxDisarmerPhysicsFrames,
            EngineConstants.PermanentSkillPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        lemming.DisarmingFrames--;
        if (lemming.DisarmingFrames <= 0)
        {
            if (lemming.NextAction == NoneAction.Instance)
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            }
            else
            {
                lemming.NextAction.TransitionLemmingToAction(lemming, false);
                lemming.SetNextAction(NoneAction.Instance);
            }
        }
        else if ((lemming.PhysicsFrame & 7) == 0)
        {
            // ?? CueSoundEffect(SFX_FIXING, L.Position); ??
        }

        return false;
    }

    protected override RectangularRegion ActionBounds() => LemmingActionBounds.DisarmerLemmingBounds;

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround) => DoMainTransitionActions(lemming, turnAround);
}