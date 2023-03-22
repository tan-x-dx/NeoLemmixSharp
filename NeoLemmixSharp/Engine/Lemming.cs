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

    public int AnimationFrame;
    public int AscenderProgress;

    public IFacingDirection FacingDirection = RightFacingDirection.Instance;
    public IOrientation Orientation = DownOrientation.Instance;

    public ILemmingAction CurrentAction = WalkerAction.Instance;
    public LemmingState CurrentState = new();

    public Lemming()
    {
    }

    public bool ShouldTick => true;

    public void Tick()
    {
        CurrentAction.UpdateLemming(this);

        AnimationFrame++;
    }
}