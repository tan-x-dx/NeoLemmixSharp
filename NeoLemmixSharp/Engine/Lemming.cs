using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Engine.Directions.FacingDirections;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LemmingStates;

namespace NeoLemmixSharp.Engine;

public sealed class Lemming : ITickable
{
    public bool IsActive = true;
    public bool IsAlive = true;
    public bool HasExited;

    public int X
    {
        get => LevelPosition.X;
        set => LevelPosition.X = value;
    }

    public int Y
    {
        get => LevelPosition.Y;
        set => LevelPosition.Y = value;
    }

    public LevelPosition LevelPosition;

    public int AnimationFrame;

    public IFacingDirection FacingDirection = RightFacingDirection.Instance;
    public IOrientation Orientation = DownOrientation.Instance;

    public ILemmingState CurrentState = WalkerState.Instance;

    public Lemming()
    {
    }

    public bool ShouldTick => true;

    public void Tick(MouseState mouseState)
    {
        CurrentState.UpdateLemming(this);
    }
}