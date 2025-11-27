using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Rewind;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager :
    IItemManager<Lemming>,
    IBitBufferCreator<RawBitBuffer, Lemming>,
    IPerfectHasher<HatchGroup>,
    ISnapshotDataConvertible,
    IInitialisable,
    IDisposable
{
    private readonly HatchGroup[] _hatchGroups;
    private readonly Lemming[] _lemmings;

    private const int RequiredNumberOfLemmingBitSets =
        2 + // spacial hash grids
        3; // lemming sets
    private readonly RawArray _lemmingByteBuffer;
    private int _bitArrayBufferUsageCount = RequiredNumberOfLemmingBitSets;

    private readonly LemmingSpacialHashGrid _lemmingPositionHelper;
    private readonly LemmingSpacialHashGrid _zombieSpacialHashGrid;
    private readonly LemmingSet _lemmingsToZombify;
    private readonly LemmingSet _allBlockers;
    private readonly LemmingSet _fastForwardLemmings;

    private readonly int _totalNumberOfHatchLemmings;
    private readonly int _numberOfPreplacedLemmings;
    private readonly int _maxNumberOfClonedLemmings;

    private LemmingManagerData _data = new();

    public int LemmingsToRelease => _data.LemmingsToRelease;
    public int LemmingsOut => _data.LemmingsOut;
    public int LemmingsRemoved => _data.LemmingsRemoved;
    public int LemmingsSaved => _data.LemmingsSaved;

    public ReadOnlySpan<HatchGroup> AllHatchGroups => new(_hatchGroups);
    public ReadOnlySpan<Lemming> AllLemmings => new(_lemmings);
    public LemmingEnumerable AllBlockers => _allBlockers.AsEnumerable();
    public Lemming GetLemming(int lemmingId) => _lemmings[lemmingId];

    public LemmingManager(
        Lemming[] lemmings,
        int numberOfPreplacedLemmings,
        HatchGroup[] hatchGroups,
        int maxNumberOfClonedLemmings,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        if (hatchGroups.Length > 0)
        {
            this.AssertUniqueIds(new ReadOnlySpan<HatchGroup>(hatchGroups));
            Array.Sort(hatchGroups, this);
        }
        _hatchGroups = hatchGroups;

        _lemmings = lemmings;
        this.AssertUniqueIds(new ReadOnlySpan<Lemming>(_lemmings));
        Array.Sort(_lemmings, this);

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
        _lemmingByteBuffer = Helpers.AllocateBuffer<uint>(bitBufferLength * RequiredNumberOfLemmingBitSets);

        _lemmingsToZombify = new LemmingSet(this);
        _allBlockers = new LemmingSet(this);
        _fastForwardLemmings = new LemmingSet(this);

        _totalNumberOfHatchLemmings = CalculateTotalNumberOfLemmingsFromHatches(hatchGroups);
        _numberOfPreplacedLemmings = numberOfPreplacedLemmings;
        _maxNumberOfClonedLemmings = maxNumberOfClonedLemmings;
    }

    [Pure]
    private static int CalculateTotalNumberOfLemmingsFromHatches(HatchGroup[] hatchGroups)
    {
        var result = 0;

        foreach (var group in hatchGroups)
        {
            result += group.LemmingsToRelease;
        }

        return result;
    }

    public void Initialise()
    {
        foreach (var lemming in _lemmings)
        {
            InitialiseLemming(lemming);
        }

        //controlPanelTextualData.SetHatchData(0);
        UpdateControlPanelLemmingData();
        //controlPanelTextualData.SetGoalData(0);
    }

    private void InitialiseLemming(Lemming lemming)
    {
        if (!lemming.State.IsActive)
            return;

        lemming.Initialise();

        _lemmingPositionHelper.AddItem(lemming);
        UpdateLemmingFastForwardState(lemming);

        if (lemming.State.IsZombie)
        {
            RegisterZombie(lemming);
        }
        else
        {
            _data.LemmingsOut++;
            UpdateControlPanelLemmingData();
        }
    }

    public void Tick(bool isMajorTick)
    {
        if (isMajorTick)
        {
            UpdateHatchGroups();

            var lemmingSpan = new ReadOnlySpan<Lemming>(_lemmings);
            foreach (var lemming in lemmingSpan)
            {
                if (lemming.State.IsActive)
                {
                    lemming.Tick();
                    UpdateLemmingPosition(lemming);
                }
            }
        }
        else
        {
            foreach (var lemming in _fastForwardLemmings)
            {
                if (lemming.State.IsActive)
                {
                    lemming.Tick();
                    UpdateLemmingPosition(lemming);
                }
            }
        }

        ZombifyLemmings();
    }

    private void UpdateHatchGroups()
    {
        var hatchLemmingSpan = new ReadOnlySpan<Lemming>(_lemmings, _numberOfPreplacedLemmings, _totalNumberOfHatchLemmings);
        foreach (var hatchGroup in _hatchGroups)
        {
            var hatchGadget = hatchGroup.Tick();

            if (hatchGadget is null)
                continue;

            hatchGroup.OnSpawnLemming();
            var lemming = hatchLemmingSpan[_data.NumberOfLemmingsReleasedFromHatch++];

            lemming.AnchorPosition = hatchGadget.Position + hatchGadget.SpawnPointOffset;
            hatchGadget.HatchSpawnData.SpawnLemming(lemming);
            InitialiseLemming(lemming);
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

    public void UpdateLemmingPosition(Lemming lemming)
    {
        if (!lemming.State.IsActive)
            return;

        _lemmingPositionHelper.UpdateItemPosition(lemming);
        lemming.OnUpdatePosition();

        if (lemming.State.IsZombie)
            _zombieSpacialHashGrid.UpdateItemPosition(lemming);
    }

    public void UpdateLemmingFastForwardState(Lemming lemming)
    {
        if (lemming.IsFastForward)
        {
            _fastForwardLemmings.Add(lemming);
        }
        else
        {
            _fastForwardLemmings.Remove(lemming);
        }
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
            _data.LemmingsSaved++;
        }

        _data.LemmingsRemoved++;
        lemming.OnRemoval(removalReason);
        UpdateControlPanelLemmingData();
    }

    private void UpdateControlPanelLemmingData()
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

    public void GetAllLemmingsNearRegion(RectangularRegion levelRegion, out LemmingEnumerable result)
    {
        _lemmingPositionHelper.GetAllItemsNearRegion(levelRegion, out result);
    }

    public void RegisterBlocker(Lemming lemming)
    {
        _allBlockers.Add(lemming);
    }

    public void DeregisterBlocker(Lemming lemming)
    {
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

            if (LevelScreen.RegionsOverlap(firstBounds, secondBounds))
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

        var checkRegion = lemming.CurrentBounds;
        _zombieSpacialHashGrid.GetAllItemsNearRegion(checkRegion, out var nearbyZombies);

        if (nearbyZombies.Count == 0)
            return;

        foreach (var zombie in nearbyZombies)
        {
            Debug.Assert(zombie.State.IsZombie);

            var zombieRegion = zombie.CurrentBounds;

            if (LevelScreen.RegionsOverlap(checkRegion, zombieRegion))
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
        return _data.NumberOfClonedLemmings < _maxNumberOfClonedLemmings;
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
                    _data.NumberOfClonedLemmings;

        clonedLemming = _lemmings[index];

        _data.NumberOfClonedLemmings++;
        _data.LemmingsOut++;

        UpdateControlPanelLemmingData();

        _lemmingPositionHelper.AddItem(clonedLemming);

        return true;
    }

    public int NumberOfLemmings => _lemmings.Length;
    ReadOnlySpan<Lemming> IItemManager<Lemming>.AllItems => new(_lemmings);
    int IPerfectHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int IPerfectHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming IPerfectHasher<Lemming>.UnHash(int index) => _lemmings[index];
    unsafe void IBitBufferCreator<RawBitBuffer, Lemming>.CreateBitBuffer(out RawBitBuffer buffer)
    {
        if (_bitArrayBufferUsageCount == 0)
            throw new InvalidOperationException("Insufficient space for bit buffers!");
        _bitArrayBufferUsageCount--;
        var bitBufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_lemmings.Length);

        uint* pointer = (uint*)_lemmingByteBuffer.Handle + (bitBufferLength * _bitArrayBufferUsageCount);
        buffer = new RawBitBuffer(pointer, bitBufferLength);
    }

    int IPerfectHasher<HatchGroup>.NumberOfItems => _hatchGroups.Length;
    int IPerfectHasher<HatchGroup>.Hash(HatchGroup item) => item.Id;
    HatchGroup IPerfectHasher<HatchGroup>.UnHash(int index) => _hatchGroups[index];

    public void Dispose()
    {
        new Span<Lemming>(_lemmings).Clear();
        new Span<HatchGroup>(_hatchGroups).Clear();
        _lemmingByteBuffer.Dispose();
        _lemmingPositionHelper.Dispose();
        _zombieSpacialHashGrid.Dispose();
        _lemmingsToZombify.Clear();
        _allBlockers.Clear();
    }

    public unsafe int GetRequiredNumberOfBytesForSnapshotting() => sizeof(LemmingManagerData);

    public unsafe void WriteToSnapshotData(byte* snapshotDataPointer)
    {
        LemmingManagerData* lemmingManagerSnapshotDataPointer = (LemmingManagerData*)snapshotDataPointer;

        *lemmingManagerSnapshotDataPointer = _data;
    }

    public unsafe void SetFromSnapshotData(byte* snapshotDataPointer)
    {
        LemmingManagerData* lemmingManagerSnapshotDataPointer = (LemmingManagerData*)snapshotDataPointer;

        _data = *lemmingManagerSnapshotDataPointer;
    }

    public void ResetLemmingPositions()
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
