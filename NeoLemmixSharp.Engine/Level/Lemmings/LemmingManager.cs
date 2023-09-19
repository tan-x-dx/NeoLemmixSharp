using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager : ISimpleHasher<Lemming>
{
    private const ChunkSizeType LemmingPositionChunkSize = ChunkSizeType.ChunkSize32;

    private readonly Lemming[] _lemmings;

    private readonly PositionHelper<Lemming> _lemmingPositionHelper;
    private readonly PositionHelper<Lemming> _blockerPositionHelper;
    private readonly PositionHelper<Lemming> _zombiePositionHelper;

    public int LemmingsToRelease { get; private set; }
    public int LemmingsOut { get; private set; }
    public int LemmingsRemoved { get; private set; }

    public int TotalNumberOfLemmings => _lemmings.Length;
    public ReadOnlySpan<Lemming> AllLemmings => new(_lemmings);

    [Pure]
    public LargeSimpleSet<Lemming> ActiveLemmings() => _lemmingPositionHelper.GetAllTrackedItemsEnumerator();

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

        _lemmingPositionHelper.AddItem(lemming);

        if (!lemming.State.IsZombie)
        {
            LemmingsOut++;
        }

        lemming.OnInitialization();
    }

    public bool LemmingIsActive(Lemming lemming) => _lemmingPositionHelper.IsTrackingItem(lemming);

    public void RemoveLemming(Lemming lemming)
    {
        _lemmingPositionHelper.RemoveItem(lemming);

        if (lemming.State.IsZombie)
        {
            DeregisterZombie(lemming);
        }

        if (lemming.CurrentAction == BlockerAction.Instance)
        {
            DeregisterBlocker(lemming);
        }
        LemmingsRemoved++;
        lemming.OnRemoval();
    }

    public void UpdateLemmingPosition(Lemming lemming)
    {
        if (!LemmingIsActive(lemming))
            return;

        _lemmingPositionHelper.UpdateItemPosition(lemming);

        if (lemming.State.IsZombie)
        {
            _zombiePositionHelper.UpdateItemPosition(lemming);
        }

        if (lemming.CurrentAction == BlockerAction.Instance)
        {
            _blockerPositionHelper.UpdateItemPosition(lemming);
        }
    }

    public LargeSimpleSet<Lemming> GetAllLemmingsNearRegion(
        LevelPosition topLeftLevelPosition,
        LevelPosition bottomRightLevelPosition)
    {
        return _lemmingPositionHelper.GetAllItemsNearRegion(topLeftLevelPosition, bottomRightLevelPosition);
    }

    public void RegisterBlocker(Lemming lemming)
    {
        _blockerPositionHelper.AddItem(lemming);
    }

    public void DeregisterBlocker(Lemming lemming)
    {
        _blockerPositionHelper.RemoveItem(lemming);
    }

    public LargeSimpleSet<Lemming> BlockersNearLemming(Lemming lemming)
    {
        return _blockerPositionHelper.GetAllItemsNearRegion(lemming.TopLeftPixel, lemming.BottomRightPixel);
    }

    public void RegisterZombie(Lemming lemming)
    {
        _zombiePositionHelper.AddItem(lemming);
    }

    public void DeregisterZombie(Lemming lemming)
    {
        _zombiePositionHelper.RemoveItem(lemming);
    }

    public LargeSimpleSet<Lemming> ZombiesNearLemming(Lemming lemming)
    {
        return _zombiePositionHelper.GetAllItemsNearRegion(lemming.TopLeftPixel, lemming.BottomRightPixel);
    }

    int ISimpleHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int ISimpleHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming ISimpleHasher<Lemming>.UnHash(int index) => _lemmings[index];
}