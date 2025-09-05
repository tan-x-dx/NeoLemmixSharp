using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
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
    private readonly LemmingSpacialHashGrid _lemmingPositionHelper;
    private readonly LemmingSpacialHashGrid _zombieSpacialHashGrid;
    private readonly LemmingSet _lemmingsToZombify;
    private readonly LemmingSet _allBlockers;
    private readonly LemmingSet _fastForwardLemmings;

    private readonly int _totalNumberOfHatchLemmings;
    private readonly int _numberOfPreplacedLemmings;
    private readonly int _maxNumberOfClonedLemmings;

    private int _bitArrayBufferUsageCount;

    private int _numberOfLemmingsReleasedFromHatch;
    private int _numberOfClonedLemmings;

    public int LemmingsToRelease { get; private set; }
    public int LemmingsOut { get; private set; }
    public int LemmingsRemoved { get; private set; }
    public int LemmingsSaved { get; private set; }

    public ReadOnlySpan<HatchGroup> AllHatchGroups => new(_hatchGroups);
    public ReadOnlySpan<Lemming> AllLemmings => new(_lemmings);
    public LemmingEnumerable AllBlockers => _allBlockers.AsEnumerable();

    public int ScratchSpaceSize => _lemmingPositionHelper.ScratchSpaceSize;

    public LemmingManager(
        Lemming[] lemmings,
        int numberOfPreplacedLemmings,
        HatchGroup[] hatchGroups,
        int totalNumberOfHatchLemmings,
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

        _bitArrayBufferUsageCount = RequiredNumberOfLemmingBitSets;

        var bitBufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_lemmings.Length);
        _lemmingByteBuffer = Helpers.AllocateBuffer<uint>(bitBufferLength * RequiredNumberOfLemmingBitSets);

        _lemmingsToZombify = new LemmingSet(this);
        _allBlockers = new LemmingSet(this);
        _fastForwardLemmings = new LemmingSet(this);

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
        UpdateLemmingFastForwardState(lemming);

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
        foreach (var hatchGroup in AllHatchGroups)
        {
            var hatchGadget = hatchGroup.Tick();

            if (hatchGadget is null)
                continue;

            hatchGroup.OnSpawnLemming();
            var lemming = hatchLemmingSpan[_numberOfLemmingsReleasedFromHatch++];

            lemming.Data.AnchorPosition = hatchGadget.Position + hatchGadget.SpawnPointOffset;
            hatchGadget.HatchSpawnData.InitialiseLemming(lemming);
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

    public void GetAllLemmingsNearRegion(
        Span<uint> scratchSpace,
        RectangularRegion levelRegion,
        out LemmingEnumerable result)
    {
        _lemmingPositionHelper.GetAllItemsNearRegion(scratchSpace, levelRegion, out result);
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
            var secondBounds = blocker.Data.CurrentBounds;

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

        Span<uint> scratchSpaceSpan = stackalloc uint[_lemmingPositionHelper.ScratchSpaceSize];
        var checkRegion = lemming.Data.CurrentBounds;
        _zombieSpacialHashGrid.GetAllItemsNearRegion(scratchSpaceSpan, checkRegion, out var nearbyZombies);

        if (nearbyZombies.Count == 0)
            return;

        foreach (var zombie in nearbyZombies)
        {
            Debug.Assert(zombie.State.IsZombie);

            var zombieRegion = zombie.Data.CurrentBounds;

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

    public int NumberOfLemmings => _lemmings.Length;
    ReadOnlySpan<Lemming> IItemManager<Lemming>.AllItems => new(_lemmings);
    int IPerfectHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int IPerfectHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming IPerfectHasher<Lemming>.UnHash(int index) => _lemmings[index];

    int IPerfectHasher<HatchGroup>.NumberOfItems => _hatchGroups.Length;
    int IPerfectHasher<HatchGroup>.Hash(HatchGroup item) => item.Id;
    HatchGroup IPerfectHasher<HatchGroup>.UnHash(int index) => _hatchGroups[index];
    unsafe void IBitBufferCreator<RawBitBuffer, Lemming>.CreateBitBuffer(out RawBitBuffer buffer)
    {
        if (_bitArrayBufferUsageCount == 0)
            throw new InvalidOperationException("Insufficient space for bit buffers!");
        _bitArrayBufferUsageCount--;
        var bitBufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_lemmings.Length);

        uint* pointer = (uint*)_lemmingByteBuffer.Handle + (bitBufferLength * _bitArrayBufferUsageCount);
        buffer = new RawBitBuffer(pointer, bitBufferLength);
    }

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

    public int GetRequiredNumberOfBytesForSnapshotting() => Unsafe.SizeOf<LemmingManagerSnapshotData>();

    public unsafe void WriteToSnapshotData(byte* snapshotDataPointer)
    {
        LemmingManagerSnapshotData* lemmingManagerSnapshotDataPointer = (LemmingManagerSnapshotData*)snapshotDataPointer;

        *lemmingManagerSnapshotDataPointer = new LemmingManagerSnapshotData(
            _numberOfLemmingsReleasedFromHatch,
            _numberOfClonedLemmings,
            LemmingsToRelease,
            LemmingsOut,
            LemmingsRemoved,
            LemmingsSaved);
    }

    public unsafe void SetFromSnapshotData(byte* snapshotDataPointer)
    {
        LemmingManagerSnapshotData* lemmingManagerSnapshotDataPointer = (LemmingManagerSnapshotData*)snapshotDataPointer;

        _numberOfLemmingsReleasedFromHatch = lemmingManagerSnapshotDataPointer->NumberOfLemmingsReleasedFromHatch;
        _numberOfClonedLemmings = lemmingManagerSnapshotDataPointer->NumberOfClonedLemmings;
        LemmingsToRelease = lemmingManagerSnapshotDataPointer->LemmingsToRelease;
        LemmingsOut = lemmingManagerSnapshotDataPointer->LemmingsOut;
        LemmingsRemoved = lemmingManagerSnapshotDataPointer->LemmingsRemoved;
        LemmingsSaved = lemmingManagerSnapshotDataPointer->LemmingsSaved;

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