using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.LemmingActions;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager : ISimpleHasher<Lemming>
{
    private const ChunkSizeType LemmingPositionChunkSize = ChunkSizeType.ChunkSize32;

    private readonly PositionHelper<Lemming> _lemmingPositionHelper;
    private readonly PositionHelper<Lemming> _blockerPositionHelper;
    private readonly PositionHelper<Lemming> _zombiePositionHelper;
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
        Array.Sort(_lemmings, IdEquatableItemHelperMethods.Compare);
        _lemmings.ValidateUniqueIds();

        _lemmingPositionHelper = new PositionHelper<Lemming>(
            this,
            LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        _blockerPositionHelper = new PositionHelper<Lemming>(
            this,
            LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        _zombiePositionHelper = new PositionHelper<Lemming>(
            this,
            LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        _activeLemmings = new LargeSimpleSet<Lemming>(this);
    }

    public void Initialise()
    {
        foreach (var lemming in AllLemmings)
        {
            InitialiseLemming(lemming);
        }
    }

    private void InitialiseLemming(Lemming lemming)
    {
        if (lemming.CurrentAction == NoneAction.Instance)
            return;

        lemming.Initialise();
        _activeLemmings.Add(lemming);

        if (!lemming.State.IsZombie)
        {
            LemmingsOut++;
        }

        _lemmingPositionHelper.AddItem(lemming);
    }

    public bool LemmingIsActive(Lemming lemming) => _activeLemmings.Contains(lemming);

    public void RemoveLemming(Lemming lemming)
    {
        _activeLemmings.Remove(lemming);
        _lemmingPositionHelper.RemoveItem(lemming);
        LemmingsRemoved++;
    }

    public void UpdateLemmingPosition(Lemming lemming)
    {
        _lemmingPositionHelper.UpdateItemPosition(lemming);

        if (lemming.State.IsZombie)
        {
            _zombiePositionHelper.UpdateItemPosition(lemming);
        }
    }

    public LargeSimpleSet<Lemming>.Enumerator GetAllLemmingsNearRegion(
        LevelPosition topLeftLevelPosition,
        LevelPosition bottomRightLevelPosition)
    {
        return _lemmingPositionHelper.GetItemsNearRegionEnumerator(topLeftLevelPosition, bottomRightLevelPosition);
    }

    public void RegisterBlocker(Lemming lemming)
    {
        _blockerPositionHelper.AddItem(lemming);
    }

    public void DeregisterBlocker(Lemming lemming)
    {
        _blockerPositionHelper.RemoveItem(lemming);
    }

    public LargeSimpleSet<Lemming>.Enumerator BlockersNearLemmingEnumerator(Lemming lemming)
    {
        return _blockerPositionHelper.GetItemsNearRegionEnumerator(lemming.TopLeftPixel, lemming.BottomRightPixel);
    }

    public void RegisterZombie(Lemming lemming)
    {
        _zombiePositionHelper.AddItem(lemming);
    }

    public void DeregisterZombie(Lemming lemming)
    {
        _zombiePositionHelper.RemoveItem(lemming);
    }

    public LargeSimpleSet<Lemming>.Enumerator ZombiesNearLemmingEnumerator(Lemming lemming)
    {
        return _zombiePositionHelper.GetItemsNearRegionEnumerator(lemming.TopLeftPixel, lemming.BottomRightPixel);
    }

    int ISimpleHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int ISimpleHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming ISimpleHasher<Lemming>.UnHash(int index) => _lemmings[index];
}