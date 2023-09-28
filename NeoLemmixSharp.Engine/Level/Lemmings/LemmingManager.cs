using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager : ISimpleHasher<Lemming>, ISimpleHasher<HatchGroup>
{
    private const ChunkSizeType LemmingPositionChunkSize = ChunkSizeType.ChunkSize32;

    private readonly HatchGroup[] _hatchGroups;
    private readonly Lemming[] _lemmings;

    private readonly SpacialHashGrid<Lemming> _lemmingPositionHelper;
    private readonly SpacialHashGrid<Lemming> _blockerPositionHelper;
    private readonly SpacialHashGrid<Lemming> _zombiePositionHelper;

    public int LemmingsToRelease { get; private set; }
    public int LemmingsOut { get; private set; }
    public int LemmingsRemoved { get; private set; }

    public int TotalNumberOfLemmings => _lemmings.Length;
    public ReadOnlySpan<Lemming> AllLemmings => new(_lemmings);

    public LemmingManager(
        Lemming[] lemmings,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _lemmings = lemmings;
        Array.Sort(_lemmings, IdEquatableItemHelperMethods.Compare);
        _lemmings.ValidateUniqueIds();

        _lemmingPositionHelper = new SpacialHashGrid<Lemming>(
            this,
            LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        _blockerPositionHelper = new SpacialHashGrid<Lemming>(
            this,
            LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        _zombiePositionHelper = new SpacialHashGrid<Lemming>(
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

    public void RemoveLemming(Lemming lemming)
    {
        lemming.State.IsActive = false;
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
        if (!lemming.State.IsActive)
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
    public SimpleSet<Lemming> GetAllLemmingsNearRegion(LevelPositionPair levelRegion)
    {
        return _lemmingPositionHelper.GetAllItemsNearRegion(levelRegion);
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
    public SimpleSet<Lemming> GetAllBlockersNearLemming(LevelPositionPair levelRegion)
    {
        return _blockerPositionHelper.GetAllItemsNearRegion(levelRegion);
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
    public SimpleSet<Lemming> GetAllZombiesNearLemming(LevelPositionPair levelRegion)
    {
        return _zombiePositionHelper.GetAllItemsNearRegion(levelRegion);
    }

    int ISimpleHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int ISimpleHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming ISimpleHasher<Lemming>.UnHash(int index) => _lemmings[index];

    int ISimpleHasher<HatchGroup>.NumberOfItems => _hatchGroups.Length;
    int ISimpleHasher<HatchGroup>.Hash(HatchGroup item) => item.Id;
    HatchGroup ISimpleHasher<HatchGroup>.UnHash(int index) => _hatchGroups[index];
}