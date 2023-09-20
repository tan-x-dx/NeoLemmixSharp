using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LargeSimpleSet<Lemming> GetAllLemmingsNearRegion(
        LevelPosition topLeftLevelPosition,
        LevelPosition bottomRightLevelPosition)
    {
        return _lemmingPositionHelper.GetAllItemsNearRegion(topLeftLevelPosition, bottomRightLevelPosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RegisterBlocker(Lemming lemming)
    {
        _blockerPositionHelper.AddItem(lemming);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeregisterBlocker(Lemming lemming)
    {
        _blockerPositionHelper.RemoveItem(lemming);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool LemmingIsBlocking(Lemming lemming)
    {
        return _blockerPositionHelper.IsTrackingItem(lemming);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LargeSimpleSet<Lemming> GetAllBlockersNearLemming(
        LevelPosition topLeftLevelPosition,
        LevelPosition bottomRightLevelPosition)
    {
        return _blockerPositionHelper.GetAllItemsNearRegion(topLeftLevelPosition, bottomRightLevelPosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RegisterZombie(Lemming lemming)
    {
        _zombiePositionHelper.AddItem(lemming);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeregisterZombie(Lemming lemming)
    {
        _zombiePositionHelper.RemoveItem(lemming);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LargeSimpleSet<Lemming> GetAllZombiesNearLemming(
        LevelPosition topLeftLevelPosition,
        LevelPosition bottomRightLevelPosition)
    {
        return _zombiePositionHelper.GetAllItemsNearRegion(topLeftLevelPosition, bottomRightLevelPosition);
    }

    int ISimpleHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int ISimpleHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming ISimpleHasher<Lemming>.UnHash(int index) => _lemmings[index];
}