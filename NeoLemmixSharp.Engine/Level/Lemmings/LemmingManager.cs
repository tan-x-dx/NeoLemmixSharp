namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager
{
    private readonly Lemming[] _lemmings;

    public ReadOnlySpan<Lemming> AllLemmings => new(_lemmings);

    public LemmingManager(Lemming[] lemmings)
    {
        _lemmings = lemmings;
    }
}