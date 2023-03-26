using NeoLemmixSharp.Engine.Directions.FacingDirections;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LemmingActions;

namespace NeoLemmixSharp.Engine;

public sealed class Lemming : ITickable
{
    public bool IsNeutral;
    public bool IsZombie;
    public bool HasPermanentSkill;

    public bool IsActive = true;
    public bool IsAlive = true;
    public bool HasExited;

    public bool IsClimber;
    public bool IsFloater;
    public bool IsGlider;
    public bool IsSlider;
    public bool IsSwimmer;

    public int AnimationFrame;
    public int AscenderProgress;
    public int NumberOfBricksLeft;
    public int DisarmingFrames;
    public bool ConstructivePositionFreeze;
    public bool IsStartingAction;
    public bool PlacedBrick;
    public bool StackLow;
    public bool InitialFall;
    public bool EndOfAnimation;
    public int DistanceFallen;
    public int TrueDistanceFallen;
    public LevelPosition DehoistPin;
    public LevelPosition LaserHitPoint;
    public bool LaserHit;
    public int LaserRemainTime;

    public int X => LevelPosition.X;
    public int Y => LevelPosition.Y;

    public LevelPosition LevelPosition;

    public bool Debug;

    public IFacingDirection FacingDirection = RightFacingDirection.Instance;
    public IOrientation Orientation = DownOrientation.Instance;

    public ILemmingAction CurrentAction = WalkerAction.Instance;
    public ILemmingAction? NextAction = null;

    public bool ShouldTick => true;

    public void Tick()
    {
        if (Debug)
        {
            ;
        }

        var continueWithLemming = true;
        var oldLevelPosition = LevelPosition;
        var oldFacingDirection = FacingDirection;
        var oldAction = CurrentAction;
        NextAction = null;

        if (!continueWithLemming)
            return;
        continueWithLemming = HandleLemmingAction();
        if (!continueWithLemming)
            return;
        continueWithLemming = CheckLevelBoundaries();
        if (!continueWithLemming)
            return;
        CheckTriggerArea(false);
    }

    private bool HandleLemmingAction()
    {
        AnimationFrame++;
        if (AnimationFrame == CurrentAction.NumberOfAnimationFrames)
        {
            if (CurrentAction == FloaterAction.Instance ||
                CurrentAction == GliderAction.Instance)
            {
                AnimationFrame = 9;
            }
            else
            {
                AnimationFrame = 0;
            }

            if (CurrentAction.IsOneTimeAction)
            {
                EndOfAnimation = true;
            }
        }

        var result = CurrentAction.UpdateLemming(this);

        return result;
    }

    private bool CheckLevelBoundaries()
    {
        return true;
    }

    private bool CheckTriggerArea(bool isPostTeleportCheck)
    {

        return true;
    }
}