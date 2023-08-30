namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.Resizing;

/// <summary>
/// A class that represents the "Do Nothing" behaviour of something that should not be allowed to change size
/// </summary>
public sealed class NoResizeBehaviour : IResizeBehaviour
{
    public static NoResizeBehaviour Instance { get; } = new();

    private NoResizeBehaviour()
    {
    }

    public void Resize(int dw, int dh)
    {
    }

    public void SetSize(int w, int h)
    {
    }
}