﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Updates;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager : IPerfectHasher<Lemming>, IDisposable
{
    private readonly HatchGroup[] _hatchGroups;
    private readonly Lemming[] _lemmings;

    private readonly SpacialHashGrid<Lemming> _lemmingPositionHelper;
    private readonly SpacialHashGrid<Lemming> _zombieSpacialHashGrid;
    private readonly SimpleSet<Lemming> _lemmingsToZombify;
    private readonly SimpleSet<Lemming> _allBlockers;

    public int LemmingsToRelease { get; private set; }
    public int LemmingsOut { get; private set; }
    public int LemmingsRemoved { get; private set; }

    public int TotalNumberOfLemmings => _lemmings.Length;
    public ReadOnlySpan<Lemming> AllLemmings => new(_lemmings);
    public ReadOnlySpan<HatchGroup> AllHatchGroups => new(_hatchGroups);

    private int _nextLemmingId;

    public LemmingManager(
        HatchGroup[] hatchGroups,
        Lemming[] lemmings,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        if (hatchGroups.Length > 0)
        {
            IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<HatchGroup>(hatchGroups));
            Array.Sort(hatchGroups, IdEquatableItemHelperMethods.Compare);
        }
        _hatchGroups = hatchGroups;

        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<Lemming>(lemmings));
        Array.Sort(lemmings, IdEquatableItemHelperMethods.Compare);
        _lemmings = lemmings;

        _lemmingPositionHelper = new SpacialHashGrid<Lemming>(
            this,
            LevelConstants.LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
        _zombieSpacialHashGrid = new SpacialHashGrid<Lemming>(
            this,
            LevelConstants.LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        _lemmingsToZombify = new SimpleSet<Lemming>(this);
        _allBlockers = new SimpleSet<Lemming>(this);
    }

    public void Initialise()
    {
        foreach (var lemming in AllLemmings)
        {
            InitialiseLemming(lemming);
        }

        var controlPanelTextualData = LevelScreen.LevelControlPanel.TextualData;

        //controlPanelTextualData.SetHatchData(0);
        controlPanelTextualData.SetLemmingData(LemmingsOut - LemmingsRemoved);
        //controlPanelTextualData.SetGoalData(0);
    }

    private void InitialiseLemming(Lemming lemming)
    {
        if (!lemming.State.IsActive)
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

    public void Tick(UpdateState updateState, bool isMajorTick)
    {
        if (updateState == UpdateState.FastForward ||
            isMajorTick)
        {
            foreach (var hatchGroup in AllHatchGroups)
            {
                var hatchGadget = hatchGroup.Tick();

                if (hatchGadget is null)
                    continue;

                var lemming = _lemmings[_nextLemmingId++];

                lemming.LevelPosition = hatchGadget.SpawnPosition;
                hatchGadget.HatchSpawnData.InitialiseLemming(lemming);
                InitialiseLemming(lemming);
                hatchGroup.OnSpawnLemming();
            }
        }

        foreach (var lemming in AllLemmings)
        {
            var i = GetTickNumberForLemming(lemming, updateState, isMajorTick);

            while (i-- > 0)
            {
                lemming.Tick();
                UpdateLemmingPosition(lemming);
            }
        }

        if (_lemmingsToZombify.Count > 0)
        {
            foreach (var lemming in _lemmingsToZombify)
            {
                lemming.State.IsZombie = true;
            }

            _lemmingsToZombify.Clear();
        }
    }

    private static int GetTickNumberForLemming(Lemming lemming, UpdateState updateState, bool isMajorTick)
    {
        var lemmingIsFastForward = lemming.IsFastForward;
        var gameIsFastForward = updateState == UpdateState.FastForward;
        if (!lemming.State.IsActive ||
            (!lemmingIsFastForward &&
             !gameIsFastForward &&
             !isMajorTick))
            return 0;

        return lemmingIsFastForward && gameIsFastForward
            ? EngineConstants.FastForwardSpeedMultiplier
            : 1;
    }

    private void UpdateLemmingPosition(Lemming lemming)
    {
        if (!lemming.State.IsActive)
            return;

        _lemmingPositionHelper.UpdateItemPosition(lemming);

        if (lemming.CurrentAction == BlockerAction.Instance)
        {
            BlockerAction.SetBlockerField(lemming, true);
        }

        // If not relevant for this lemming, nothing will happen
        UpdateZombiePosition(lemming);
    }

    public void RemoveLemming(Lemming lemming, LemmingRemovalReason removalReason)
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
        lemming.OnRemoval(removalReason);
        LevelScreen.LevelControlPanel.TextualData.SetLemmingData(LemmingsOut - LemmingsRemoved);
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

    #region Zombie Handling

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RegisterZombie(Lemming lemming) => _zombieSpacialHashGrid.AddItem(lemming);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeregisterZombie(Lemming lemming) => _zombieSpacialHashGrid.RemoveItem(lemming);

    private void UpdateZombiePosition(Lemming lemming)
    {
        if (!_zombieSpacialHashGrid.IsTrackingItem(lemming))
            return;

        _zombieSpacialHashGrid.UpdateItemPosition(lemming);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AnyZombies() => !_zombieSpacialHashGrid.IsEmpty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DoZombieCheck(Lemming lemming)
    {
        Debug.Assert(!lemming.State.IsZombie);

        var checkRegion = new LevelPositionPair(lemming.TopLeftPixel, lemming.BottomRightPixel);
        var nearbyZombies = _zombieSpacialHashGrid.GetAllItemsNearRegion(checkRegion);

        if (nearbyZombies.Count == 0)
            return;

        foreach (var zombie in nearbyZombies)
        {
            Debug.Assert(zombie.State.IsZombie);

            var zombieRegion = new LevelPositionPair(zombie.TopLeftPixel, zombie.BottomRightPixel);

            if (checkRegion.Overlaps(zombieRegion))
            {
                RegisterLemmingForZombification(lemming);

                return;
            }
        }
    }

    private void RegisterLemmingForZombification(Lemming lemming)
    {
        _lemmingsToZombify.Add(lemming);
    }

    #endregion

    int IPerfectHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int IPerfectHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming IPerfectHasher<Lemming>.UnHash(int index) => _lemmings[index];

    public void Dispose()
    {
        Array.Clear(_lemmings);
        Array.Clear(_hatchGroups);
        _lemmingPositionHelper.Clear();
        _lemmingsToZombify.Clear();
        _allBlockers.Clear();
    }
}