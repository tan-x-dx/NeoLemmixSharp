using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.LemmingActions;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager : ISimpleHasher<Lemming>, IComparer<Lemming>
{
    private readonly ChunkManager<Lemming> _lemmingChunkManager;
    private readonly LargeSimpleSet<Lemming> _activeLemmings;
    private readonly Lemming[] _lemmings;

    public int LemmingsToRelease { get; private set; }
    public int LemmingsOut { get; private set; }
    public int LemmingsRemoved { get; private set; }

    public int TotalNumberOfLemmings => _lemmings.Length;
    public ReadOnlySpan<Lemming> AllLemmings => new(_lemmings);

    public LargeSimpleSet<Lemming>.Enumerator ActiveLemmingsEnumerator => _activeLemmings.GetEnumerator();

    public LemmingManager(
        Lemming[] lemmings,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _lemmings = lemmings;
        Array.Sort(_lemmings, this);
        _lemmings.ValidateUniqueIds();

        _lemmingChunkManager = new ChunkManager<Lemming>(lemmings, this, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
        _activeLemmings = new LargeSimpleSet<Lemming>(this);

        for (var i = 0; i < _lemmings.Length; i++)
        {
            var lemming = _lemmings[i];
            if (lemming.CurrentAction != NoneAction.Instance)
            {
                _activeLemmings.Add(lemming);
                _lemmingChunkManager.UpdateItemPosition(lemming, true);
            }
        }
    }

    public bool LemmingIsActive(Lemming lemming) => _activeLemmings.Contains(lemming);

    public void RemoveLemming(Lemming lemming)
    {


        _activeLemmings.Remove(lemming);
    }

    public void UpdateLemmingPosition(Lemming lemming)
    {
        _lemmingChunkManager.UpdateItemPosition(lemming, false);
    }

    public void PopulateSetWithLemmingsFromRegion(
        LargeSimpleSet<Lemming> set,
        LevelPosition topLeftLevelPosition,
        LevelPosition bottomRightLevelPosition)
    {
        _lemmingChunkManager.PopulateSetWithItemsFromRegion(set, topLeftLevelPosition, bottomRightLevelPosition);
    }

    int IComparer<Lemming>.Compare(Lemming? x, Lemming? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (y is null) return 1;
        if (x is null) return -1;
        return x.Id.CompareTo(y.Id);
    }

    int ISimpleHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int ISimpleHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming ISimpleHasher<Lemming>.Unhash(int index) => _lemmings[index];
}