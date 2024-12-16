using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DisarmerAction : LemmingAction
{
    public static readonly DisarmerAction Instance = new();

    private DisarmerAction()
        : base(
            EngineConstants.DisarmerActionId,
            EngineConstants.DisarmerActionName,
            EngineConstants.DisarmerActionSpriteFileName,
            EngineConstants.DisarmerAnimationFrames,
            EngineConstants.MaxDisarmerPhysicsFrames,
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

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 8;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 5;
}