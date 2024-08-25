using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Updates;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

    private readonly int _totalNumberOfHatchLemmings;
    private readonly int _numberOfPreplacedLemmings;
    private readonly int _maxNumberOfClonedLemmings;

    private int _numberOfLemmingsReleasedFromHatch;
    private int _numberOfClonedLemmings;

    public int LemmingsToRelease { get; private set; }
    public int LemmingsOut { get; private set; }
    public int LemmingsRemoved { get; private set; }
    public int LemmingsSaved { get; private set; }

    public int TotalNumberOfLemmings => _lemmings.Length;
    public ReadOnlySpan<HatchGroup> AllHatchGroups => new(_hatchGroups);
    public SimpleSetEnumerable<Lemming> AllBlockers => _allBlockers.AsSimpleEnumerable();

    public LemmingManager(
        HatchGroup[] hatchGroups,
        List<Lemming> lemmings,
        int totalNumberOfHatchLemmings,
        int numberOfPreplacedLemmings,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        if (hatchGroups.Length > 0)
        {
            IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<HatchGroup>(hatchGroups));
            Array.Sort(hatchGroups, IdEquatableItemHelperMethods.Compare);
        }
        _hatchGroups = hatchGroups;

        _lemmings = lemmings.ToArray();
        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<Lemming>(_lemmings));
        Array.Sort(_lemmings, IdEquatableItemHelperMethods.Compare);

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

        _lemmingsToZombify = new SimpleSet<Lemming>(this, false);
        _allBlockers = new SimpleSet<Lemming>(this, false);

        _totalNumberOfHatchLemmings = totalNumberOfHatchLemmings;
        _numberOfPreplacedLemmings = numberOfPreplacedLemmings;
        _maxNumberOfClonedLemmings = _lemmings.Length - totalNumberOfHatchLemmings - numberOfPreplacedLemmings;
    }

    public void Initialise()
    {
        foreach (var lemming in _lemmings)
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
            UpdateControlPanel();
        }
    }

    public void Tick(UpdateState updateState, bool isMajorTick)
    {
        UpdateHatchGroups(updateState, isMajorTick);
        UpdateLemmings(updateState, isMajorTick);

        ZombifyLemmings();
    }

    private void UpdateHatchGroups(UpdateState updateState, bool isMajorTick)
    {
        if (updateState != UpdateState.FastForward &&
            !isMajorTick)
            return;

        var hatchLemmingSpan = new ReadOnlySpan<Lemming>(_lemmings, 0, _totalNumberOfHatchLemmings);
        foreach (var hatchGroup in AllHatchGroups)
        {
            var hatchGadget = hatchGroup.Tick();

            if (hatchGadget is null)
                continue;

            hatchGroup.OnSpawnLemming();
            var lemming = hatchLemmingSpan[_numberOfLemmingsReleasedFromHatch++];

            lemming.LevelPosition = hatchGadget.SpawnPosition;
            hatchGadget.HatchSpawnData.InitialiseLemming(lemming);
            InitialiseLemming(lemming);
        }
    }

    private void UpdateLemmings(UpdateState updateState, bool isMajorTick)
    {
        var lemmingSpan = new ReadOnlySpan<Lemming>(_lemmings);
        foreach (var lemming in lemmingSpan)
        {
            var i = GetTickNumberForLemming(lemming, updateState, isMajorTick);

            switch (i)
            {
                case 3: lemming.Tick(); UpdateLemmingPosition(lemming); goto case 2;
                case 2: lemming.Tick(); UpdateLemmingPosition(lemming); goto case 1;
                case 1: lemming.Tick(); UpdateLemmingPosition(lemming); break;
                case 0: break;
            }
        }
    }

    private static int GetTickNumberForLemming(Lemming lemming, UpdateState updateState, bool isMajorTick)
    {
        var lemmingIsFastForward = lemming.IsFastForward;
        var gameIsFastForward = updateState == UpdateState.FastForward;

        if (lemming.State.IsActive &&
            (lemmingIsFastForward ||
             gameIsFastForward ||
             isMajorTick))
            return lemmingIsFastForward && gameIsFastForward
                ? EngineConstants.FastForwardSpeedMultiplier
                : 1;

        return 0;
    }

    private void ZombifyLemmings()
    {
        if (_lemmingsToZombify.Count == 0)
            return;

        foreach (var lemming in _lemmingsToZombify)
        {
            lemming.State.IsZombie = true;
        }

        _lemmingsToZombify.Clear();
    }

    private void UpdateLemmingPosition(Lemming lemming)
    {
        if (!lemming.State.IsActive)
            return;

        _lemmingPositionHelper.UpdateItemPosition(lemming);
        lemming.OnUpdatePosition();

        if (lemming.CurrentAction == BlockerAction.Instance)
        {
            BlockerAction.SetBlockerField(lemming, true);
        }

        // If not relevant for this lemming, nothing will happen
        UpdateZombiePosition(lemming);
    }

    private void UpdateZombiePosition(Lemming lemming)
    {
        if (!lemming.State.IsZombie)
            return;

        _zombieSpacialHashGrid.UpdateItemPosition(lemming);
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

        if (removalReason == LemmingRemovalReason.Exit)
        {
            LemmingsSaved++;
        }

        LemmingsRemoved++;
        lemming.OnRemoval(removalReason);
        UpdateControlPanel();
    }

    private void UpdateControlPanel()
    {
        LevelScreen.LevelControlPanel.TextualData.SetLemmingData(LemmingsOut - LemmingsRemoved);
    }

    public static Lemming SimulateLemming(Lemming lemming, bool checkGadgets)
    {
        var simulationLemming = Lemming.SimulationLemming;
        simulationLemming.SetRawDataFromOther(lemming);

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

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AnyZombies() => !_zombieSpacialHashGrid.IsEmpty;

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

    public bool CanCreateNewLemmingClone()
    {
        return _numberOfClonedLemmings < _maxNumberOfClonedLemmings;
    }

    public bool TryGetNextClonedLemming([MaybeNullWhen(false)] out Lemming clonedLemming)
    {
        if (!CanCreateNewLemmingClone())
        {
            clonedLemming = default;
            return false;
        }

        var index = _totalNumberOfHatchLemmings +
                    _numberOfPreplacedLemmings +
                    _numberOfClonedLemmings;

        clonedLemming = _lemmings[index];

        _numberOfClonedLemmings++;
        LemmingsOut++;

        UpdateControlPanel();

        _lemmingPositionHelper.AddItem(clonedLemming);

        return true;
    }

    int IPerfectHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int IPerfectHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming IPerfectHasher<Lemming>.UnHash(int index) => _lemmings[index];

    public void Dispose()
    {
        new Span<Lemming>(_lemmings).Clear();
        new Span<HatchGroup>(_hatchGroups).Clear();
        _lemmingPositionHelper.Clear();
        _lemmingsToZombify.Clear();
        _allBlockers.Clear();
    }
}