namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.Movement;

public sealed class StationaryBehaviour : IMovementBehaviour
{
    public static StationaryBehaviour Instance { get; } = new();

    private StationaryBehaviour()
    {
    }

}