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

    public int AnimationFrame;
    public int AscenderProgress;
    public int NumberOfBricksLeft;
    public int DisarmingFrames;
    public bool ConstructivePositionFreeze;
    public bool IsStartingAction;
    public bool PlacedBrick;
    public bool StackLow;
    public bool InitialFall;
    public int DistanceFallen;
    public int TrueDistanceFallen;
    public LevelPosition DehoistPin;

    public int X => LevelPosition.X;
    public int Y => LevelPosition.Y;

    public LevelPosition LevelPosition;

    public bool Debug;

    public IFacingDirection FacingDirection = RightFacingDirection.Instance;
    public IOrientation Orientation = DownOrientation.Instance;

    public ILemmingAction CurrentAction = WalkerAction.Instance;

    public bool ShouldTick => true;

    public void Tick()
    {
        if (Debug)
        {
            ;
        }

        AnimationFrame++;
        if (AnimationFrame == CurrentAction.NumberOfAnimationFrames)
        {
            AnimationFrame = 0;
        }

        CurrentAction.UpdateLemming(this);

    }
}