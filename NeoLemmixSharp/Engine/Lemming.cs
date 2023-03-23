using NeoLemmixSharp.Engine.Directions.FacingDirections;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LemmingActions;

namespace NeoLemmixSharp.Engine;

public sealed class Lemming : ITickable
{
    public bool IsActive = true;
    public bool IsAlive = true;
    public bool HasExited;

    public int X => LevelPosition.X;
    public int Y => LevelPosition.Y;

    public LevelPosition LevelPosition;

    public bool Debug;

    public int AnimationFrame;
    public int AscenderProgress;
    public int NumberOfBricksLeft;
    public bool ConstructivePositionFreeze;

    public IFacingDirection FacingDirection = RightFacingDirection.Instance;
    public IOrientation Orientation = DownOrientation.Instance;

    public ILemmingAction CurrentAction = WalkerAction.Instance;
    public LemmingState CurrentState = new();

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