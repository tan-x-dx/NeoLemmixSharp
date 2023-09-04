using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager : ISimpleHasher<Lemming>, IComparer<Lemming>
{
    private readonly LargeSimpleSet<Lemming> _activeLemmings;
    private readonly Lemming[] _lemmings;

    public int LemmingsToRelease { get; private set; }
    public int LemmingsOut { get; private set; }
    public int LemmingsRemoved { get; private set; }

    public int TotalNumberOfLemmings => _lemmings.Length;
    public ReadOnlySpan<Lemming> AllLemmings => new(_lemmings);

    public LargeSimpleSet<Lemming>.Enumerator ActiveLemmingsEnumerator => _activeLemmings.GetEnumerator();

    public LemmingManager(Lemming[] lemmings)
    {
        _lemmings = lemmings;
        Array.Sort(_lemmings, this);
        _lemmings.ValidateUniqueIds();

        _activeLemmings = new LargeSimpleSet<Lemming>(this, true);
    }

    public bool LemmingIsActive(Lemming lemming) => _activeLemmings.Contains(lemming);

    public void RemoveLemming(Lemming lemming)
    {


        _activeLemmings.Remove(lemming);
    }

    int IComparer<Lemming>.Compare(Lemming? x, Lemming? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (y is null) return 1;
        if (x is null) return -1;
        return x.Id.CompareTo(y.Id);
    }

    bool IEquatable<ISimpleHasher<Lemming>>.Equals(ISimpleHasher<Lemming>? other) => ReferenceEquals(this, other);
    int ISimpleHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int ISimpleHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming ISimpleHasher<Lemming>.Unhash(int index) => _lemmings[index];
}