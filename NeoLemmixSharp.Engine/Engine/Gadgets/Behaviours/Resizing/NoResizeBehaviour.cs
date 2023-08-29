namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.Resizing;

public sealed class NoResizeBehaviour : IResizeBehaviour
{
    public static NoResizeBehaviour Instance { get; } = new();

    private NoResizeBehaviour()
    {
    }

}