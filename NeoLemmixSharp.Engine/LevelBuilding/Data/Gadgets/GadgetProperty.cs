namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public enum GadgetProperty
{
    Behaviour,

    TriggerX,
    TriggerY,
    TriggerWidth,
    TriggerHeight,
    Width,
    Height,
    Team,
    RawLemmingState,
    LemmingCount
}

public sealed class GadgetPropertyEqualityComparer : IEqualityComparer<GadgetProperty>
{
    private static GadgetPropertyEqualityComparer? _instance = null;

    public static GadgetPropertyEqualityComparer Instance => _instance ??= new GadgetPropertyEqualityComparer();

    private GadgetPropertyEqualityComparer()
    {
    }

    public bool Equals(GadgetProperty x, GadgetProperty y)
    {
        return x == y;
    }

    public int GetHashCode(GadgetProperty obj)
    {
        return HashCode.Combine((int)obj);
    }
}