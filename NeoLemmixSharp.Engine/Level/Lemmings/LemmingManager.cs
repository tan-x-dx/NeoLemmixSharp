using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager :
    IPerfectHasher<Lemming>,
    IBitBufferCreator<ArrayBitBuffer>,
    IItemManager<Lemming>,
    ISnapshotDataConvertible<LemmingManagerSnapshotData>,
    IInitialisable,
    IDisposable
{
    private readonly HatchGroup[] _hatchGroups;
    private readonly Lemming[] _lemmings;

    private readonly LemmingSpacialHashGrid _lemmingPositionHelper;
    private readonly LemmingSpacialHashGrid _zombieSpacialHashGrid;
    private readonly LemmingSet _lemmingsToZombify;
    private readonly LemmingSet _allBlockers;

    private readonly int _totalNumberOfHatchLemmings;
    private readonly int _numberOfPreplacedLemmings;
    private readonly int _maxNumberOfClonedLemmings;

    private int _numberOfLemmingsReleasedFromHatch;
    private int _numberOfClonedLemmings;

    public int LemmingsToRelease { get; private set; }
    public int LemmingsOut { get; private set; }
    public int LemmingsRemoved { get; private set; }
    public int LemmingsSaved { get; private set; }

    public ReadOnlySpan<HatchGroup> AllHatchGroups => new(_hatchGroups);
    public LemmingEnumerable AllBlockers => _allBlockers.AsEnumerable();
    public ReadOnlySpan<Lemming> AllItems => new(_lemmings);

    public int ScratchSpaceSize => _lemmingPositionHelper.ScratchSpaceSize;

    public LemmingManager(
        HatchGroup[] hatchGroups,
        Lemming[] lemmings,
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

        _lemmings = lemmings;
        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<Lemming>(_lemmings));
        Array.Sort(_lemmings, IdEquatableItemHelperMethods.Compare);

        _lemmingPositionHelper = new LemmingSpacialHashGrid(
            this,
            EngineConstants.LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
        _zombieSpacialHashGrid = new LemmingSpacialHashGrid(
            this,
            EngineConstants.LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        var bitBufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_lemmings.Length);
        var combinedLemmingBitBuffer = new uint[bitBufferLength * 2];
        _lemmingsToZombify = new LemmingSet(
            this,
            new ArrayBitBuffer(combinedLemmingBitBuffer, bitBufferLength * 0, bitBufferLength));
        _allBlockers = new LemmingSet(
            this,
            new ArrayBitBuffer(combinedLemmingBitBuffer, bitBufferLength * 1, bitBufferLength));

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

    public void Tick(bool isMajorTick)
    {
        if (isMajorTick)
        {
            UpdateHatchGroups();
        }

        UpdateLemmings(isMajorTick);

        ZombifyLemmings();
    }

    private void UpdateHatchGroups()
    {
        var hatchLemmingSpan = new ReadOnlySpan<Lemming>(_lemmings, _numberOfPreplacedLemmings, _totalNumberOfHatchLemmings);
        foreach (var hatchGroup in AllHatchGroups)
        {
            var hatchGadget = hatchGroup.Tick();

            if (hatchGadget is null)
                continue;

            hatchGroup.OnSpawnLemming();
            var lemming = hatchLemmingSpan[_numberOfLemmingsReleasedFromHatch++];

            lemming.LevelPosition = hatchGadget.Position + hatchGadget.SpawnPointOffset;
            hatchGadget.HatchSpawnData.InitialiseLemming(lemming);
            InitialiseLemming(lemming);
        }
    }

    private void UpdateLemmings(bool isMajorTick)
    {
        var lemmingSpan = new ReadOnlySpan<Lemming>(_lemmings);
        foreach (var lemming in lemmingSpan)
        {
            if (lemming.State.IsActive && (isMajorTick || lemming.IsFastForward))
            {
                lemming.Tick();
                UpdateLemmingPosition(lemming);
            }
        }
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
        LevelScreen.LevelObjectiveManager.RecheckCriteria();
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

    public void GetAllLemmingsNearRegion(
        Span<uint> scratchSpace,
        LevelRegion levelRegion,
        out LemmingEnumerable result)
    {
        _lemmingPositionHelper.GetAllItemsNearRegion(scratchSpace, levelRegion, out result);
    }

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

    public static bool DoBlockerCheck(Lemming lemming)
    {
        BlockerAction.DoBlockerCheck(lemming);
        return true;
    }

    [Pure]
    public bool CanAssignBlocker(Lemming lemming)
    {
        var firstBounds = BlockerAction.Instance.GetLemmingBounds(lemming);

        foreach (var blocker in _allBlockers)
        {
            var secondBounds = blocker.CurrentBounds;

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

    [SkipLocalsInit]
    public void DoZombieCheck(Lemming lemming)
    {
        Debug.Assert(!lemming.State.IsZombie);

        Span<uint> scratchSpaceSpan = stackalloc uint[_lemmingPositionHelper.ScratchSpaceSize];
        var checkRegion = lemming.CurrentBounds;
        _zombieSpacialHashGrid.GetAllItemsNearRegion(scratchSpaceSpan, checkRegion, out var nearbyZombies);

        if (nearbyZombies.Count == 0)
            return;

        foreach (var zombie in nearbyZombies)
        {
            Debug.Assert(zombie.State.IsZombie);

            var zombieRegion = zombie.CurrentBounds;

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
            clonedLemming = null;
            return false;
        }

        var index = _numberOfPreplacedLemmings +
                    _totalNumberOfHatchLemmings +
                    _numberOfClonedLemmings;

        clonedLemming = _lemmings[index];

        _numberOfClonedLemmings++;
        LemmingsOut++;

        UpdateControlPanel();

        _lemmingPositionHelper.AddItem(clonedLemming);

        return true;
    }

    public int NumberOfItems => _lemmings.Length;

    int IPerfectHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming IPerfectHasher<Lemming>.UnHash(int index) => _lemmings[index];
    void IBitBufferCreator<ArrayBitBuffer>.CreateBitBuffer(out ArrayBitBuffer buffer) => buffer = new(BitArrayHelpers.CreateBitArray(NumberOfItems, false));

    public void Dispose()
    {
        new Span<Lemming>(_lemmings).Clear();
        new Span<HatchGroup>(_hatchGroups).Clear();
        _lemmingPositionHelper.Clear();
        _lemmingsToZombify.Clear();
        _allBlockers.Clear();
    }

    public void WriteToSnapshotData(out LemmingManagerSnapshotData snapshotData)
    {
        snapshotData = new LemmingManagerSnapshotData(
            _numberOfLemmingsReleasedFromHatch,
            _numberOfClonedLemmings,
            LemmingsToRelease,
            LemmingsOut,
            LemmingsRemoved,
            LemmingsSaved);
    }

    public void SetFromSnapshotData(in LemmingManagerSnapshotData snapshotData)
    {
        _numberOfLemmingsReleasedFromHatch = snapshotData.NumberOfLemmingsReleasedFromHatch;
        _numberOfClonedLemmings = snapshotData.NumberOfClonedLemmings;
        LemmingsToRelease = snapshotData.LemmingsToRelease;
        LemmingsOut = snapshotData.LemmingsOut;
        LemmingsRemoved = snapshotData.LemmingsRemoved;
        LemmingsSaved = snapshotData.LemmingsSaved;

        ResetLemmingPositions();
    }

    private void ResetLemmingPositions()
    {
        _lemmingPositionHelper.Clear();
        _zombieSpacialHashGrid.Clear();

        foreach (var lemming in _lemmings)
        {
            if (!lemming.State.IsActive)
                continue;

            _lemmingPositionHelper.AddItem(lemming);

            if (lemming.State.IsZombie)
            {
                _zombieSpacialHashGrid.AddItem(lemming);
            }
        }
    }
}