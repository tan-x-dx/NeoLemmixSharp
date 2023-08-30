namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.Movement;

/// <summary>
/// A class that represents the "Do Nothing" behaviour of something that should not be allowed to move
/// </summary>
public sealed class StationaryBehaviour : IMovementBehaviour
{
    public static StationaryBehaviour Instance { get; } = new();

    private StationaryBehaviour()
    {
    }

    public void Move(int dx, int dy)
    {
    }

    public void SetPosition(int x, int y)
    {
    }
}