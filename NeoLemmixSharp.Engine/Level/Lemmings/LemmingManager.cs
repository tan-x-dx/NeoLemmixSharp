using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings.ZombieHelpers;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager : IPerfectHasher<Lemming>
{
    public const int BlockerQuantityThreshold = 20;
    public const ChunkSizeType LemmingPositionChunkSize = ChunkSizeType.ChunkSize32;

    private readonly HatchGroup[] _hatchGroups;
    private readonly Lemming[] _lemmings;

    private readonly SpacialHashGrid<Lemming> _lemmingPositionHelper;
    private readonly SimpleSet<Lemming> _lemmingsToZombify;
    private readonly SimpleSet<Lemming> _allBlockers;
    private readonly IZombieHelper _zombieHelper;

    public int LemmingsToRelease { get; private set; }
    public int LemmingsOut { get; private set; }
    public int LemmingsRemoved { get; private set; }

    public int TotalNumberOfLemmings => _lemmings.Length;
    public ReadOnlySpan<Lemming> AllLemmings => new(_lemmings);

    private int _nextLemmingId;

    public LemmingManager(
        LevelData levelData,
        HatchGroup[] hatchGroups,
        Lemming[] lemmings,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _hatchGroups = hatchGroups;
        if (_hatchGroups.Length > 0)
        {
            IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<HatchGroup>(hatchGroups));
            Array.Sort(_hatchGroups, IdEquatableItemHelperMethods.Compare);
        }

        _lemmings = lemmings;
        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<Lemming>(lemmings));
        Array.Sort(_lemmings, IdEquatableItemHelperMethods.Compare);

        _lemmingPositionHelper = new SpacialHashGrid<Lemming>(
            this,
            LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        _lemmingsToZombify = new SimpleSet<Lemming>(this);
        _allBlockers = new SimpleSet<Lemming>(this);

        _zombieHelper = GetZombieHelper(levelData, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
    }

    private IZombieHelper GetZombieHelper(
        LevelData levelData,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        var levelHasZombies = levelData.LevelContainsAnyZombies();

        if (levelHasZombies)
            return new PositionTrackingZombieHelper(this, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);

        return new EmptyZombieHelper();
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

        if (lemming.State.IsZombie)
        {
            RegisterZombie(lemming);
        }
        else
        {
            LemmingsOut++;
        }
    }

    public void Tick()
    {
        for (var i = 0; i < _hatchGroups.Length; i++)
        {
            var hatchGadget = _hatchGroups[i].Tick();

            if (hatchGadget is null)
                continue;

            var lemming = _lemmings[_nextLemmingId++];

            lemming.LevelPosition = hatchGadget.SpawnPosition;
            hatchGadget.HatchSpawnData.InitialiseLemming(lemming);
            InitialiseLemming(lemming);
            _hatchGroups[i].OnSpawnLemming();
        }

        foreach (var lemming in _lemmingsToZombify)
        {
            lemming.State.IsZombie = true;
        }
        _lemmingsToZombify.Clear();
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

        if (lemming.CurrentAction == BlockerAction.Instance)
        {
            BlockerAction.SetBlockerField(lemming, true);
        }

        // If not relevant for this lemming, nothing will happen
        _zombieHelper.UpdateZombiePosition(lemming);
    }

    public static Lemming SimulateLemming(Lemming lemming, bool checkGadgets)
    {
        var simulationLemming = Lemming.SimulationLemming;
        simulationLemming.SetRawData(lemming);

        simulationLemming.Simulate(checkGadgets);

        return simulationLemming;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleSetEnumerable<Lemming> GetAllLemmingsNearRegion(LevelPositionPair levelRegion) => _lemmingPositionHelper.GetAllItemsNearRegion(levelRegion);

    public void RegisterBlocker(Lemming lemming)
    {
        BlockerAction.SetBlockerField(lemming, true);
        _allBlockers.Add(lemming);
    }

    public void DeregisterBlocker(Lemming lemming)
    {
        BlockerAction.SetBlockerField(lemming, false);
        _allBlockers.Remove(lemming);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool DoBlockerCheck(Lemming lemming)
    {
        BlockerAction.DoBlockerCheck(lemming);
        return true;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanAssignBlocker(Lemming lemming)
    {
        var firstBounds = BlockerAction.Instance.GetLemmingBounds(lemming);

        foreach (var blocker in _allBlockers)
        {
            var blockerTopLeft = blocker.TopLeftPixel;
            var blockerBottomRight = blocker.BottomRightPixel;

            var secondBounds = new LevelPositionPair(blockerTopLeft, blockerBottomRight);

            if (firstBounds.Overlaps(secondBounds))
                return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RegisterZombie(Lemming lemming) => _zombieHelper.RegisterZombie(lemming);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeregisterZombie(Lemming lemming) => _zombieHelper.DeregisterZombie(lemming);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AnyZombies() => _zombieHelper.AnyZombies();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DoZombieCheck(Lemming lemming) => _zombieHelper.CheckZombies(lemming);

    public void RegisterLemmingForZombification(Lemming lemming)
    {
        _lemmingsToZombify.Add(lemming);
    }

    int IPerfectHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int IPerfectHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming IPerfectHasher<Lemming>.UnHash(int index) => _lemmings[index];
}