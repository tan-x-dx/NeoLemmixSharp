namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public sealed class AnimationLayerArchetypeData
{
    public required int Layer { get; init; }
    public required AnimationLayerParameters AnimationLayerParameters { get; init; }
    public required int InitialFrame { get; init; }
    public required int NextGadgetState { get; init; }

    internal AnimationLayerArchetypeData()
    {
    }
}
